using AutoMapper;
using BookCommerce_API.Models;
using BookCommerce_API.Models.DTO.ProductImage;
using BookCommerce_API.Repository.ProductImage;
using BookCommerce_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookCommerce_API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ProductImageAPIController : ControllerBase
    {
        private readonly IProductImageRepository _productImageRepo;
        private readonly ILogger<ProductImageAPIController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private APIResponse _response;

        public ProductImageAPIController(IProductImageRepository productImageRepo,
                                         ILogger<ProductImageAPIController> logger,
                                         IWebHostEnvironment hostEnvironment,
                                         IMapper mapper)
        {
            _productImageRepo = productImageRepo;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
            _logger.LogInformation("ProductImageAPI Controller called.");
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllProductImage()
        {
            try
            {
                _logger.LogInformation("GetAllProductImage Method called.");
                var productImageList = await _productImageRepo.GetAllAsync();
                _response = Response(HttpStatusCode.OK, true, Result: _mapper.Map<List<ProductImageDTO>>(productImageList));
                _logger.LogInformation("GetAllProductImage Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteProductImage")]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        //[Authorize(Roles = $"{StaticDetails.ROLE_ADMIN},{StaticDetails.ROLE_EMPLOYEE},{StaticDetails.ROLE_USER_COMPANY}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> DeleteProductImage(int id)
        {
            try
            {
                _logger.LogInformation("DeleteProductImage Method called.");

                if (id == 0)
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                var productImage = await _productImageRepo.GetAsync(x => x.ProductImageId == id);
                if (productImage == null)
                {
                    _logger.LogError("Product Image model is null.");
                    _response = Response(HttpStatusCode.NotFound, false);
                    return NotFound(_response);
                }

                if (!string.IsNullOrEmpty(productImage.ImgLocalPath))
                {
                    var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, productImage.ImgLocalPath.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        _logger.LogInformation("Deleting image.");
                        System.IO.File.Delete(oldImagePath);
                    }

                    await _productImageRepo.RemoveAsync(productImage);
                }

                _response = Response(HttpStatusCode.OK, Result: productImage);
                _logger.LogInformation("DeleteProductImage Method done.");
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
