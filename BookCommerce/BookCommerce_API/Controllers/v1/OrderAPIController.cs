using AutoMapper;
using BookCommerce_API.Models;
using BookCommerce_API.Models.DTO.Order;
using BookCommerce_API.Models.DTO.Order.Details;
using BookCommerce_API.Models.DTO.Order.Header;
using BookCommerce_API.Repository.Order;
using BookCommerce_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookCommerce_API.Controllers.v1
{
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	[ApiVersion("1.0")]
	public class OrderAPIController : ControllerBase
	{
		private readonly IOrderHeaderRepository _orderHeaderRepo;
		private readonly IOrderDetailsRepository _orderDetailsRepo;
		private readonly ILogger<OrderAPIController> _logger;
		private readonly IMapper _mapper;
		private APIResponse _response;

		public OrderAPIController(IOrderHeaderRepository orderHeaderRepo,
								  IOrderDetailsRepository orderDetailsRepo,
								  ILogger<OrderAPIController> logger,
								  IMapper mapper)
		{
			_orderHeaderRepo = orderHeaderRepo;
			_orderDetailsRepo = orderDetailsRepo;
			_logger = logger;
			_mapper = mapper;
			_logger.LogInformation("OrderAPI Controller called.");
		}

		[HttpGet("{applicationUserId}/{orderHeaderId:int}", Name = "GetOrderHeader")]
		[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetOrderHeader(string applicationUserId, int orderHeaderId)
        {
            try
            {
                _logger.LogInformation("GetAllOrderHeader Method called.");

				if(applicationUserId != "all" && orderHeaderId != 0)
				{
                    OrderHeaderModel orderList = await _orderHeaderRepo.GetAsync(x =>
                        x.OrderHeaderId == orderHeaderId &&
                        x.ApplicationUserId == applicationUserId, includeProperties: "ApplicationUser");

                    _response = Response(HttpStatusCode.OK, true, Result: _mapper.Map<OrderHeaderDTO>(orderList));
                }
				else
				{
                    
                    if (orderHeaderId != 0)
                    {
                        OrderHeaderModel orderList = await _orderHeaderRepo.GetAsync(x => 
							x.OrderHeaderId == orderHeaderId, includeProperties: "ApplicationUser");

                        _response = Response(HttpStatusCode.OK, true, Result: _mapper.Map<OrderHeaderDTO>(orderList));
                    }
                    else
                    {
                        IEnumerable<OrderHeaderModel> orderList;

                        if (applicationUserId == "all")
                            orderList = await _orderHeaderRepo.GetAllAsync(includeProperties: "ApplicationUser");
                        else
                            orderList = await _orderHeaderRepo.GetAllAsync(x => x.ApplicationUserId == applicationUserId, includeProperties: "ApplicationUser");

                        _response = Response(HttpStatusCode.OK, true, Result: _mapper.Map<IEnumerable<OrderHeaderDTO>>(orderList));
                    }
                }

                _logger.LogInformation("GetAllOrderHeader Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpGet("{orderHeaderId:int}", Name = "GetAllOrderDetails")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetOrderDetails(int orderHeaderId)
        {
            try
            {
                _logger.LogInformation("GetAllOrderDetails Method called.");
                if (orderHeaderId == 0)
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, isSuccess: false);
                    return BadRequest(_response);
                }

				IEnumerable<OrderDetailsModel> orderDetailList = await _orderDetailsRepo.GetAllAsync(x => x.OrderHeaderId == orderHeaderId, includeProperties: "Product");
                _response = Response(HttpStatusCode.OK, true, Result: _mapper.Map<IEnumerable<OrderDetailsDTO>>(orderDetailList));
                _logger.LogInformation("GetAllOrderDetails Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpPost("CreateOrderHeader")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<APIResponse>> CreateOrderHeader([FromBody] CreateOrderHeaderDTO model)
		{
			try
			{
				_logger.LogInformation("CreateOrderHeader Method called.");

				if (model == null)
				{
					_logger.LogError("CreateOrderHeaderDTO model is null.");
					return NotFound(model);
				}

				OrderHeaderModel orderHeaderModel = _mapper.Map<OrderHeaderModel>(model);
				await _orderHeaderRepo.CreateAsync(orderHeaderModel);
				await _orderHeaderRepo.SaveAsync();

				_response = Response(HttpStatusCode.Created, Result: _mapper.Map<OrderHeaderDTO>(orderHeaderModel));
				_logger.LogInformation("CreateOrderHeader Method done. Created route to GetCoverType method.");
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message.ToString());
				throw;
			}
		}

		[HttpPost("CreateOrderDetails")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<APIResponse>> CreateOrderDetails([FromBody] CreateOrderDetailsDTO model)
		{
			try
			{
				_logger.LogInformation("CreateOrderDetails Method called.");

				if (model == null)
				{
					_logger.LogError("CreateOrderDetailsDTO model is null.");
					return NotFound(model);
				}

				OrderDetailsModel orderDetailsModel = _mapper.Map<OrderDetailsModel>(model);
				await _orderDetailsRepo.CreateAsync(orderDetailsModel);
				await _orderDetailsRepo.SaveAsync();

				_response = Response(HttpStatusCode.Created, Result: _mapper.Map<OrderDetailsModel>(orderDetailsModel));
				_logger.LogInformation("CreateOrderDetails Method done.");
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message.ToString());
				throw;
			}
		}

		[HttpPut("UpdateStripePayment")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<APIResponse>> UpdateStripePayment([FromBody] StripePaymentDTO model)
		{
			try
			{
				_logger.LogInformation("UpdateStripePayment Method called.");

				if (model == null)
				{
					_logger.LogError("StripePaymentDTO model is null.");
					return NotFound(model);
				}
				await _orderHeaderRepo.UpdateStripePayment(model.OrderHeaderId, model.SessionId, model.PaymentIntentId);
				await _orderHeaderRepo.SaveAsync();

				_response = Response(HttpStatusCode.NoContent);
				_logger.LogInformation("UpdateStripePayment Method done.");
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message.ToString());
				throw;
			}
		}

		[HttpPut("UpdateStatus")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<APIResponse>> UpdateStatus([FromBody] OrderStatusDTO model)
		{
			try
			{
				_logger.LogInformation("UpdateStatus Method called.");

				if (model == null)
				{
					_logger.LogError("OrderStatusDTO model is null.");
					return NotFound(model);
				}
				await _orderHeaderRepo.UpdateStatus(model.OrderHeaderId, model.OrderStatus, model.PaymentStatus);
				await _orderHeaderRepo.SaveAsync();

				_response = Response(HttpStatusCode.NoContent);
				_logger.LogInformation("UpdateStatus Method done.");
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message.ToString());
				throw;
			}
		}

        [HttpPut("UpdateOrderHeader")]
        [Authorize(Roles = $"{StaticDetails.ROLE_ADMIN},{StaticDetails.ROLE_EMPLOYEE}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> UpdateOrderHeader([FromBody] UpdateOrderHeaderDTO model)
        {
            try
            {
                _logger.LogInformation("UpdateOrderHeader Method called.");

                if (model == null)
                {
                    _logger.LogError("UpdateOrderHeaderDTO model is null.");
                    return NotFound(model);
                }

				OrderHeaderModel orderHeaderModel = _mapper.Map<OrderHeaderModel>(model);
                await _orderHeaderRepo.UpdateAsync(orderHeaderModel);
                _response = Response(HttpStatusCode.NoContent);
                _logger.LogInformation("UpdateOrderHeaderDTO Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        private APIResponse Response(HttpStatusCode statusCode,
									 bool isSuccess = true,
									 List<string> ErrorMessages = null,
									 object Result = null)
		{
			var response = new APIResponse();
			response.IsSuccess = isSuccess;
			response.StatusCode = statusCode;
			response.ErrorMessages = ErrorMessages;
			response.Result = Result;

			return response;
		}
	}
}
