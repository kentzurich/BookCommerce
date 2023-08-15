using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.ApplicationUser;
using BookCommerce_WEB.Models.DTO.Order;
using BookCommerce_WEB.Models.DTO.Order.Details;
using BookCommerce_WEB.Models.DTO.Order.Header;
using BookCommerce_WEB.Models.ViewModel;
using BookCommerce_WEB.Services.Order;
using BookCommerce_WEB.Services.UserAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookCommerce_WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
	{
        private readonly IUserAuthService _userAuthService;
        private readonly IOrderService _orderService;
        [BindProperty]
        public OrderViewModel OrderVM { get; set; }

        public OrderController(IUserAuthService userAuthService,
                               IOrderService orderService)
        {
            _userAuthService = userAuthService;
            _orderService = orderService;
        }
        public IActionResult Index()
		{
			return View();
		}

        public async Task<IActionResult> Details(int orderHeaderId)
        {
            var userId = await GetUserId();
            OrderVM = new();
            APIResponse response = new();

            if ((_userAuthService.IsInRole<APIResponse>(StaticDetails.ROLE_ADMIN).Result.IsSuccess ||
                 _userAuthService.IsInRole<APIResponse>(StaticDetails.ROLE_EMPLOYEE).Result.IsSuccess))
                response = await _orderService.GetOrderHeaderAsync<APIResponse>(orderHeaderId: orderHeaderId);
            else
                response = await _orderService.GetOrderHeaderAsync<APIResponse>(userId, orderHeaderId);
            
            OrderVM.OrderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO>(response.Result.ToString());
            response = await _orderService.GetOrderDetailsAsync<APIResponse>(orderHeaderId);
            OrderVM.OrderDetails = JsonConvert.DeserializeObject<IEnumerable<OrderDetailsDTO>>(response.Result.ToString());

            if (OrderVM.OrderHeader == null)
                return NotFound();
            else
                return View(OrderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Details))]
        public async Task<IActionResult> Details_PayNow()
        {
            APIResponse response = new();

            response = await _orderService.GetOrderHeaderAsync<APIResponse>(orderHeaderId: OrderVM.OrderHeader.OrderHeaderId);
            OrderVM.OrderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO>(response.Result.ToString());
            response = await _orderService.GetOrderDetailsAsync<APIResponse>(OrderVM.OrderHeader.OrderHeaderId);
            OrderVM.OrderDetails = JsonConvert.DeserializeObject<IEnumerable<OrderDetailsDTO>>(response.Result.ToString());

            // Stripe Settings
            var domainUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{domainUrl}Admin/Order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.OrderHeaderId}",
                CancelUrl = $"{domainUrl}Admin/Order/Details?orderHeaderId={OrderVM.OrderHeader.OrderHeaderId}",
            };

            foreach (var item in OrderVM.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            await UpdateStripePayment(OrderVM.OrderHeader.OrderHeaderId, session.Id, session.PaymentIntentId);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);//redirecting new url which is provided by stripe
        }

        public async Task<IActionResult> PaymentConfirmation(int orderHeaderId)
        {
            var responseGetOrderHeader = await _orderService.GetOrderHeaderAsync<APIResponse>(orderHeaderId: orderHeaderId);
            OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(responseGetOrderHeader.Result.ToString());

            if (orderHeaderDTO.PaymentStatus.Equals(StaticDetails.PAYMENTSTATUS_DELAYED) ||
                (orderHeaderDTO.PaymentStatus.Equals(StaticDetails.ORDERSTATUS_PENDING) &&
                 orderHeaderDTO.OrderStatus.Equals(StaticDetails.ORDERSTATUS_PENDING)))
            {
                var service = new SessionService();
                Session session = service.Get(orderHeaderDTO.SessionId);
                //check stripe status
                if (session.PaymentStatus.ToLower().Equals("paid"))
                {
                    await UpdateStripePayment(orderHeaderDTO.OrderHeaderId, session.Id, session.PaymentIntentId);
                    await UpdateOrderStatus(orderHeaderId, StaticDetails.ORDERSTATUS_APPROVED, StaticDetails.PAYMENTSTATUS_APPROVED);
                }
            }
            return View(orderHeaderId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{StaticDetails.ROLE_ADMIN},{StaticDetails.ROLE_EMPLOYEE}")]
        public async Task<IActionResult> UpdateOrder()
        {
            var responseGetOrderHeader = await _orderService.GetOrderHeaderAsync<APIResponse>(orderHeaderId: OrderVM.OrderHeader.OrderHeaderId);
            UpdateOrderHeaderDTO updateOrderHeaderDTO = JsonConvert.DeserializeObject<UpdateOrderHeaderDTO>(responseGetOrderHeader.Result.ToString());
            updateOrderHeaderDTO.Name = OrderVM.OrderHeader.Name;
            updateOrderHeaderDTO.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            updateOrderHeaderDTO.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            updateOrderHeaderDTO.City = OrderVM.OrderHeader.City;
            updateOrderHeaderDTO.State = OrderVM.OrderHeader.State;
            updateOrderHeaderDTO.PostalCode = OrderVM.OrderHeader.PostalCode;
            updateOrderHeaderDTO.ShippingDate = null;
            updateOrderHeaderDTO.PaymentDueDate = null;

            if (OrderVM.OrderHeader.Carrier != null)
                updateOrderHeaderDTO.Carrier = OrderVM.OrderHeader.Carrier;

            if (OrderVM.OrderHeader.TrackingNumber != null)
                updateOrderHeaderDTO.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;

            await _orderService.UpdateOrderHeaderAsync<APIResponse>(updateOrderHeaderDTO);
            TempData["Success"] = "Order details updated successfully.";
            return RedirectToAction(nameof(Details), new { orderHeaderId = updateOrderHeaderDTO.OrderHeaderId });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN + "," + StaticDetails.ROLE_EMPLOYEE)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProcessing()
        {
            await UpdateOrderStatus(OrderVM.OrderHeader.OrderHeaderId, StaticDetails.ORDERSTATUS_INPROCESS);
            TempData["Success"] = "Order start processing.";
            return RedirectToAction(nameof(Details), new { orderHeaderId = OrderVM.OrderHeader.OrderHeaderId });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN + "," + StaticDetails.ROLE_EMPLOYEE)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShipOrder()
        {
            var responseGetOrderHeader = await _orderService.GetOrderHeaderAsync<APIResponse>(orderHeaderId: OrderVM.OrderHeader.OrderHeaderId);
            UpdateOrderHeaderDTO updateOrderHeaderDTO = JsonConvert.DeserializeObject<UpdateOrderHeaderDTO>(responseGetOrderHeader.Result.ToString());
            updateOrderHeaderDTO.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            updateOrderHeaderDTO.Carrier = OrderVM.OrderHeader.Carrier;
            updateOrderHeaderDTO.OrderStatus = StaticDetails.ORDERSTATUS_SHIPPED;
            updateOrderHeaderDTO.ShippingDate = DateTime.Now;
            updateOrderHeaderDTO.PaymentDueDate = null;

            await _orderService.UpdateOrderHeaderAsync<APIResponse>(updateOrderHeaderDTO);
            TempData["Success"] = "Order shipped successfully.";
            return RedirectToAction(nameof(Details), new { orderHeaderId = OrderVM.OrderHeader.OrderHeaderId });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN + "," + StaticDetails.ROLE_EMPLOYEE)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder()
        {
            var responseGetOrderHeader = await _orderService.GetOrderHeaderAsync<APIResponse>(orderHeaderId: OrderVM.OrderHeader.OrderHeaderId);
            OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(responseGetOrderHeader.Result.ToString());
            if (orderHeaderDTO.PaymentStatus.Equals(StaticDetails.PAYMENTSTATUS_APPROVED))
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaderDTO.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);
                await UpdateOrderStatus(orderHeaderDTO.OrderHeaderId, StaticDetails.ORDERSTATUS_CANCELLED, StaticDetails.ORDERSTATUS_REFUNDED);
            }
            else
            {
                await UpdateOrderStatus(orderHeaderDTO.OrderHeaderId, StaticDetails.ORDERSTATUS_CANCELLED, StaticDetails.ORDERSTATUS_CANCELLED);
            }

            TempData["Success"] = "Order cancelled successfully.";
            return RedirectToAction(nameof(Details), new { orderHeaderId = OrderVM.OrderHeader.OrderHeaderId });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
		{
            IEnumerable<OrderHeaderDTO> orderHeader;
            OrderHeaderDTO orderHeaderDTO = new();

            if (_userAuthService.IsInRole<APIResponse>(StaticDetails.ROLE_EMPLOYEE).Result.IsSuccess ||
                _userAuthService.IsInRole<APIResponse>(StaticDetails.ROLE_ADMIN).Result.IsSuccess)
            {
                var responseGetAllOrder = await _orderService.GetOrderHeaderAsync<APIResponse>("all");
                orderHeader = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDTO>>(responseGetAllOrder.Result.ToString());
            }
            else
            {
                var userId = await GetUserId();
                var responseGetAllOrder = await _orderService.GetOrderHeaderAsync<APIResponse>(userId);
                orderHeader = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDTO>>(responseGetAllOrder.Result.ToString());
            }

            switch (status)
            {
                case "paymentpending":
                    orderHeader = orderHeader.Where(x => x.PaymentStatus == StaticDetails.PAYMENTSTATUS_PENDING);
                    break;
                case "inprocess":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == StaticDetails.ORDERSTATUS_INPROCESS);
                    break;
                case "completed":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == StaticDetails.ORDERSTATUS_SHIPPED);
                    break;
                case "approved":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == StaticDetails.ORDERSTATUS_APPROVED);
                    break;
                case "cancelled":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == StaticDetails.ORDERSTATUS_CANCELLED);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeader });

        }

        private async Task UpdateOrderStatus(int orderHeaderId, string orderStatus = null, string paymentStatus = null)
        {
            OrderStatusDTO orderStatusDTO = new()
            {
                OrderHeaderId = orderHeaderId,
                OrderStatus = orderStatus,
                PaymentStatus = paymentStatus
            };
            await _orderService.UpdateStatusAsync<APIResponse>(orderStatusDTO);
        }

        private async Task UpdateStripePayment(int orderHeaderId, string sessionId, string paymentIntentId)
        {
            StripePaymentDTO stripePaymentDTO = new()
            {
                OrderHeaderId = orderHeaderId,
                SessionId = sessionId,
                PaymentIntentId = paymentIntentId
            };
            await _orderService.UpdateStripePaymentAsync<APIRequest>(stripePaymentDTO);
        }

        private async Task<string> GetUserId()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var responseUserId = await _userAuthService.GetUserId<APIResponse>(claimsIdentity.Name);
            if (responseUserId.Result != null && responseUserId.IsSuccess)
            {
                ApplicationUserDTO applicationUserDTO = JsonConvert.DeserializeObject<ApplicationUserDTO>(responseUserId.Result.ToString());
                return applicationUserDTO.Id;
            }

            return string.Empty;
        }
    }
}
