using AutoMapper;
using BookCommerce_API.Models;
using BookCommerce_API.Models.DTO.ShoppingCart;
using BookCommerce_API.Models.ViewModel;
using BookCommerce_API.Repository.Product;
using BookCommerce_API.Repository.ShoppingCart;
using BookCommerce_API.Repository.User;
using BookCommerce_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookCommerce_API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly IShoppingCartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ShoppingCartAPIController> _logger;
        private APIResponse _response;
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public ShoppingCartAPIController(IShoppingCartRepository cartRepo,
                                         IProductRepository productRepo,
                                         IMapper mapper,
                                         ILogger<ShoppingCartAPIController> logger)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _mapper = mapper;
            _logger = logger;
            _logger.LogInformation("ShoppingCartAPI Controller called.");
        }

        [HttpGet("GetAllShoppingCart/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllShoppingCart(string userName)
        {
            try
            {
                _logger.LogInformation("GetAllShoppingCart Method called.");
                IEnumerable<ShoppingCartModel> cartList = await _cartRepo.GetAllAsync(x => x.ApplicationUserId == userName, includeProperties: "Product,ApplicationUser");
                _response = Response(HttpStatusCode.OK, true, Result: _mapper.Map<IEnumerable<ShoppingCartDTO>>(cartList));
                _logger.LogInformation("GetAllShoppingCart Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpGet("GetShoppingCart/{id:int}", Name = "GetShoppingCart")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetShoppingCart(int id)
        {
            try
            {
                _logger.LogInformation("GetShoppingCart Method called.");

                if (id == 0)
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, isSuccess: false);
                    return BadRequest(_response);
                }

                var cart = await _cartRepo.GetAsync(x => x.CartId == id);

                if (cart == null)
                {
                    _logger.LogError("ShoppingCart Model is null.");
                    _response = Response(HttpStatusCode.NotFound, isSuccess: false);
                    return NotFound(_response);
                }

                _response = Response(HttpStatusCode.OK, Result: _mapper.Map<ShoppingCartDTO>(cart));
                _logger.LogInformation("GetAllCategory Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<APIResponse>> AddToCart([FromBody] CreateShoppingCartDTO model)
        {
            try
            {
                _logger.LogInformation("AddToCart Method called.");

                if (model == null)
                {
                    _logger.LogError("CreateShoppingCartDTO model is null.");
                    return NotFound(model);
                }

                if (model.ProductId == 0)
                {
                    _logger.LogError("ProductId is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, isSuccess: false);
                    return BadRequest(_response);
                }

                var product = await _productRepo.GetAsync(x => x.ProductId == model.ProductId);

                if(product == null)
                {
                    _logger.LogError("Product is not exist.");
                    _response = Response(HttpStatusCode.NotFound, isSuccess: false, new List<string> { "Product is not exist." });
                    return NotFound(_response);
                }

                if(model.Count < 1)
                {
                    _logger.LogError("Count must greater than 1.");
                    _response = Response(HttpStatusCode.BadRequest, isSuccess: false, new List<string> { "Count must greater than 1." });
                    return BadRequest(_response);
                }

                ShoppingCartModel shoppingCartModel = _mapper.Map<ShoppingCartModel>(model);
                var cart = await _cartRepo.GetAsync(x => x.ApplicationUserId == model.ApplicationUserId && x.ProductId == model.ProductId);

                if (cart is null)
                    await _cartRepo.CreateAsync(shoppingCartModel);
                else
                    await _cartRepo.IncrementCount(cart, model.Count);

                await _cartRepo.SaveAsync();

                _response = Response(HttpStatusCode.Created, Result: _mapper.Map<ShoppingCartDTO>(shoppingCartModel));
                _logger.LogInformation("AddToCart Method done. Created route to GetShoppingCart method.");
                return CreatedAtRoute(nameof(GetShoppingCart), new { id = shoppingCartModel.CartId }, _response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

		[HttpPut("DecrementCount/{id:int}", Name = "DecrementCount")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<APIResponse>> DecrementCount(int id)
		{
			try
			{
				_logger.LogInformation("DecrementCount Method called.");

				if (id == 0)
				{
					_logger.LogError("Id is equal 0.");
					_response = Response(HttpStatusCode.BadRequest, false);
					return BadRequest(_response);
				}

				var cart = await _cartRepo.GetAsync(x => x.CartId == id);
                await _cartRepo.DecrementCount(cart, 1);
				await _cartRepo.SaveAsync();
				_response = Response(HttpStatusCode.NoContent);
				_logger.LogInformation("DecrementCount Method done.");
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message.ToString());
				throw;
			}
		}

		[HttpPut("IncrementCount/{id:int}", Name = "IncrementCount")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<APIResponse>> IncrementCount(int id)
		{
			try
			{
				_logger.LogInformation("IncrementCount Method called.");

				if (id == 0)
				{
					_logger.LogError("Id is equal 0.");
					_response = Response(HttpStatusCode.BadRequest, false);
					return BadRequest(_response);
				}

				var cart = await _cartRepo.GetAsync(x => x.CartId == id);
				await _cartRepo.IncrementCount(cart, 1);
				await _cartRepo.SaveAsync();
				_response = Response(HttpStatusCode.NoContent);
				_logger.LogInformation("IncrementCount Method done.");
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message.ToString());
				throw;
			}
		}

		[HttpGet("CountShoppingCart/{id}", Name = "CountShoppingCart")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> CountShoppingCart(string id)
        {
            try
            {
                _logger.LogInformation("CountShoppingCart Method called.");

                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, isSuccess: false);
                    return BadRequest(_response);
                }

                var countCart = _cartRepo.GetAllAsync(x => x.ApplicationUserId == id).GetAwaiter().GetResult().Count;
                ShoppingCartDTO shoppingCartDTO = new()
                {
                    Count = countCart,
                };

                _response = Response(HttpStatusCode.OK, Result: _mapper.Map<ShoppingCartDTO>(shoppingCartDTO));
                _logger.LogInformation("CountShoppingCart Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

		[HttpDelete("DeleteCartItem/{id:int}", Name = "DeleteCartItem")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<APIResponse>> DeleteCartItem(int id)
		{
			try
			{
				_logger.LogInformation("DeleteCartItem Method called.");

				if (id == 0)
				{
					_logger.LogError("Id is equal to 0.");
					_response = Response(HttpStatusCode.BadRequest, false);
					return BadRequest(_response);
				}

				var cart = await _cartRepo.GetAsync(x => x.CartId == id);
				if (cart == null)
				{
					_logger.LogError("Cart model is null.");
					_response = Response(HttpStatusCode.NotFound, false);
					return NotFound(_response);
				}

				await _cartRepo.RemoveAsync(cart);
				_response = Response(HttpStatusCode.NoContent);
				_logger.LogInformation("DeleteCartItem Method done.");
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message.ToString());
				throw;
			}
		}

		[HttpDelete("DeleteAllCart/{id}", Name = "DeleteAllCart")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<APIResponse>> DeleteAllCart(string id)
		{
            try
            {
				_logger.LogInformation("DeleteAllCart Method called.");

				if (id == null)
				{
					_logger.LogError("Id is equal to 0.");
					_response = Response(HttpStatusCode.BadRequest, false);
					return BadRequest(_response);
				}
                List<ShoppingCartModel> cartList = await _cartRepo.GetAllAsync(x => x.ApplicationUserId == id);
                if(cartList == null || cartList.Count < 1)
                {
					_logger.LogError("Cart model is empty.");
					_response = Response(HttpStatusCode.NotFound, false);
					return NotFound(_response);
				}

                await _cartRepo.RemoveRangeAsync(cartList);
				_response = Response(HttpStatusCode.NoContent);
				_logger.LogInformation("DeleteAllCart Method done.");
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
