using AutoMapper;
using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.ApplicationUser;
using BookCommerce_WEB.Models.DTO.Order;
using BookCommerce_WEB.Models.DTO.Order.Details;
using BookCommerce_WEB.Models.DTO.Order.Header;
using BookCommerce_WEB.Models.DTO.ProductImage;
using BookCommerce_WEB.Models.DTO.ShoppingCart;
using BookCommerce_WEB.Models.ViewModel;
using BookCommerce_WEB.Services.Order;
using BookCommerce_WEB.Services.ProductImage;
using BookCommerce_WEB.Services.ShoppingCart;
using BookCommerce_WEB.Services.UserAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookCommerce_WEB.Areas.Customer.Controllers
{
	[Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductImageService _productImageService;
        private readonly IUserAuthService _userAuthService;
		private readonly IOrderService _orderService;
		private readonly IEmailSender _emailSender;
		private readonly IMapper _mapper;

		[BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }
        public double OrderTotal { get; set; }

        public CartController(IShoppingCartService shoppingCartService,
                              IProductImageService productImageService,
                              IUserAuthService userAuthService,
							  IOrderService orderService,
							  IEmailSender emailSender,
							  IMapper mapper)
        {
            _shoppingCartService = shoppingCartService;
            _productImageService = productImageService;
            _userAuthService = userAuthService;
			_orderService = orderService;
			_emailSender = emailSender;
			_mapper = mapper;
		}

        public async Task<IActionResult> Index()
        {
            var userId = await GetUserId();
            var responseGetCart = await _shoppingCartService.GetAllAsync<APIResponse>(userId);

            ShoppingCartVM = new ShoppingCartViewModel()
            {
                ListCart = JsonConvert.DeserializeObject<IEnumerable<ShoppingCartDTO>>(responseGetCart.Result.ToString()),
                OrderHeader = new()
            };

            var responseProductImages = await _productImageService.GetAllAsync<APIResponse>();
            IEnumerable<ProductImageDTO> productImages = JsonConvert.DeserializeObject<IEnumerable<ProductImageDTO>>(responseProductImages.Result.ToString());

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Product.ProductImage = productImages.Where(x => x.ProductId == cart.Product.ProductId).ToList();
                cart.Price = GetPriceBasedQty(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

		public async Task<IActionResult> Summary()
		{
			var userId = await GetUserId();
			var responseGetCart = await _shoppingCartService.GetAllAsync<APIResponse>(userId);

			ShoppingCartVM = new ShoppingCartViewModel()
			{
				ListCart = JsonConvert.DeserializeObject<IEnumerable<ShoppingCartDTO>>(responseGetCart.Result.ToString()),
				OrderHeader = new()
			};

            var responseApplicationUser = await _userAuthService.GetAsync<APIResponse>(userId);
            ShoppingCartVM
                .OrderHeader
                .ApplicationUser = JsonConvert
                .DeserializeObject<ApplicationUserDTO>(responseApplicationUser.Result.ToString());

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

			foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedQty(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName(nameof(Summary))]
		public async Task<IActionResult> SummaryPOST()
		{
			var userId = await GetUserId();
			var responseGetCart = await _shoppingCartService.GetAllAsync<APIResponse>(userId);

			ShoppingCartVM.ListCart = JsonConvert.DeserializeObject<IEnumerable<ShoppingCartDTO>>(responseGetCart.Result.ToString());
			ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = GetPriceBasedQty(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			var responseUser = await _userAuthService.GetAsync<APIResponse>(userId);
			ApplicationUserDTO applicationUserDTO = JsonConvert.DeserializeObject<ApplicationUserDTO>(responseUser.Result.ToString());

			ShoppingCartVM.OrderHeader.PaymentStatus =
				applicationUserDTO.CompanyId == 0 ?
					StaticDetails.PAYMENTSTATUS_PENDING : StaticDetails.PAYMENTSTATUS_DELAYED;
			ShoppingCartVM.OrderHeader.OrderStatus =
				applicationUserDTO.CompanyId == 0 ?
					StaticDetails.ORDERSTATUS_PENDING : StaticDetails.ORDERSTATUS_APPROVED;

			if (applicationUserDTO.CompanyId != 0)
				ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);

			var createOrderHeader = _mapper.Map<CreateOrderHeaderDTO>(ShoppingCartVM.OrderHeader);
			var orderHeader = await _orderService.CreateOrderHeaderAsync<APIResponse>(createOrderHeader);
			ShoppingCartVM.OrderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO>(orderHeader.Result.ToString());

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				CreateOrderDetailsDTO orderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.OrderHeaderId,
					Price = cart.Price,
					Count = cart.Count
				};
				await _orderService.CreateOrderDetailsAsync<APIResponse>(orderDetail);
			}

			if (applicationUserDTO.CompanyId == 0 || applicationUserDTO.CompanyId == null)
			{
				// Stripe Settings
				var domainUrl = $"{Request.Scheme}://{Request.Host.Value}/";
				var options = new SessionCreateOptions
				{
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
					SuccessUrl = $"{domainUrl}Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.OrderHeaderId}",
					CancelUrl = $"{domainUrl}Customer/Cart/Index",
				};

				foreach (var item in ShoppingCartVM.ListCart)
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
				await UpdateStripePayment(ShoppingCartVM.OrderHeader.OrderHeaderId, session.Id, session.PaymentIntentId);
				Response.Headers.Add("Location", session.Url);

                await _shoppingCartService.DeleteRangeAsync<APIResponse>(userId);
                HttpContext.Session.Clear();
                return new StatusCodeResult(303);//redirecting new url which is provided by stripe
			}
			else
			{
				return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.OrderHeaderId });
			}
		}

		public async Task<IActionResult> OrderConfirmation(int id)
		{
            var responseGetOrderHeader = await _orderService.GetOrderHeaderAsync<APIResponse>(orderHeaderId: id);
            OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(responseGetOrderHeader.Result.ToString());

			if (!orderHeaderDTO.PaymentStatus.Equals(StaticDetails.PAYMENTSTATUS_DELAYED))
			{
				//this is an order of a customer
				var service = new SessionService();
				Session session = service.Get(orderHeaderDTO.SessionId);
				//check stripe status
				if (session.PaymentStatus.ToLower().Equals("paid"))
				{
					await UpdateStripePayment(orderHeaderDTO.OrderHeaderId, session.Id, session.PaymentIntentId);
					OrderStatusDTO orderStatusDTO = new()
					{
						OrderHeaderId = id,
						OrderStatus = StaticDetails.ORDERSTATUS_APPROVED,
						PaymentStatus = StaticDetails.PAYMENTSTATUS_APPROVED
					};
					await _orderService.UpdateStatusAsync<APIResponse>(orderStatusDTO);
				}
			}

			await _emailSender.SendEmailAsync(
				orderHeaderDTO.ApplicationUser.Email, 
				"New Order - BookCommerce", $"<p>New order created. Order Id No. {orderHeaderDTO.OrderHeaderId}</p>");
			return View(id);
		}

		public async Task<IActionResult> Plus(int cartId)
        {
			var responseCart = await _shoppingCartService.GetAsync<APIResponse>(cartId);
			if (responseCart.Result != null && responseCart.IsSuccess)
                await _shoppingCartService.IncrementCount<APIResponse>(cartId);
			else
				TempData["error"] = responseCart.ErrorMessages.FirstOrDefault();

			return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var responseCart = await _shoppingCartService.GetAsync<APIResponse>(cartId);
            if(responseCart.Result != null && responseCart.IsSuccess)
            {
				ShoppingCartDTO shoppingCartDTO = JsonConvert.DeserializeObject<ShoppingCartDTO>(responseCart.Result.ToString());
                if(shoppingCartDTO.Count <= 1)
                {
					await _shoppingCartService.DeleteAsync<APIResponse>(cartId);
					await CountCartItem();
				}
                else
                {
                    await _shoppingCartService.DecrementCount<APIResponse>(cartId);
				}
			}
			else
			{
				TempData["error"] = responseCart.ErrorMessages.FirstOrDefault();
			}

			return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
		{
            var responseCart = await _shoppingCartService.DeleteAsync<APIResponse>(cartId);
            if(responseCart != null && responseCart.IsSuccess)
                await CountCartItem();
            else
				TempData["error"] = responseCart.ErrorMessages.FirstOrDefault();

			return RedirectToAction(nameof(Index));
		}

        private async Task<int> CountCartItem()
        {
			var userId = await GetUserId();
			var responseCountCart = await _shoppingCartService.CountShoppingCart<APIResponse>(userId);
            if(responseCountCart.Result != null && responseCountCart.IsSuccess)
            {
				ShoppingCartDTO shoppingCartDTO = JsonConvert.DeserializeObject<ShoppingCartDTO>(responseCountCart.Result.ToString());
				HttpContext.Session.SetInt32(StaticDetails.SessionCart, shoppingCartDTO.Count);
                return shoppingCartDTO.Count;
			}
            else
            {
				TempData["error"] = responseCountCart.ErrorMessages.FirstOrDefault();
			}

            return 0;
		}

        private async Task<string> GetUserId()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var responseUserId = await _userAuthService.GetUserId<APIResponse>(claimsIdentity.Name);
            if(responseUserId.Result != null && responseUserId.IsSuccess)
            {
				ApplicationUserDTO applicationUserDTO = JsonConvert.DeserializeObject<ApplicationUserDTO>(responseUserId.Result.ToString());
				return applicationUserDTO.Id;
			}

            return string.Empty;
		}

		private double GetPriceBasedQty(ShoppingCartDTO cart)
        {
            if (cart.Count <= 50)
                return cart.Product.Price;
            else
            {
                if (cart.Count <= 100)
                    return cart.Product.Price_50;
                else
                    return cart.Product.Price_100;
            }
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
    }
}
