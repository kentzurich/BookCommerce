using AutoMapper;
using BookCommerce_API.Models;
using BookCommerce_API.Models.DTO.Product;
using BookCommerce_API.Models.DTO.ProductImage;
using BookCommerce_API.Repository.Category;
using BookCommerce_API.Repository.CoverType;
using BookCommerce_API.Repository.Product;
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
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ICoverTypeRepository _coverTypeRepo;
        private readonly IProductImageRepository _productImageRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<ProductAPIController> _logger;
        private APIResponse _response;

        public ProductAPIController(IProductRepository productRepo, 
                                    ICategoryRepository categoryRepo,
                                    ICoverTypeRepository coverTypeRepo,
                                    IProductImageRepository productImageRepo,
                                    IMapper mapper,
                                    IWebHostEnvironment hostEnvironment,
                                    ILogger<ProductAPIController> logger)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _coverTypeRepo = coverTypeRepo;
            _productImageRepo = productImageRepo;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _logger.LogInformation("ProductAPI Controller called.");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllProduct()
        {
            try
            {
                _logger.LogInformation("GetAllProduct Method called.");
                IEnumerable<ProductModel> productList = await _productRepo.GetAllAsync(includeProperties: "Category,CoverType,ProductImage");
                _response = Response(HttpStatusCode.OK, true, Result: _mapper.Map<IEnumerable<ProductDTO>>(productList));
                _logger.LogInformation("GetAllProduct Method done.");
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpGet("{id:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetProduct(int id)
        {
            try
            {
                _logger.LogInformation("GetProduct Method called.");

                if (id == 0)
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, isSuccess: false);
                    return BadRequest(_response);
                }

                var product = await _productRepo.GetAsync(x => x.ProductId == id, includeProperties: "Category,CoverType,ProductImage");

                if (product == null)
                {
                    _logger.LogError("Product Model is null.");
                    _response = Response(HttpStatusCode.NotFound, isSuccess: false);
                    return NotFound(_response);
                }

                _response = Response(HttpStatusCode.OK, Result: _mapper.Map<ProductDTO>(product));
                _logger.LogInformation("GetProduct Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<APIResponse>> CreateProduct([FromForm] CreateProductDTO model)
        {
            try
            {
                _logger.LogInformation("CreateProduct Method called.");

                if (model == null)
                {
                    _logger.LogError("CreateProductDTO model is null.");
                    return NotFound(model);
                }

                ProductModel productModel = _mapper.Map<ProductModel>(model);
                productModel.Category = await _categoryRepo.GetAsync(x => x.CategoryId == productModel.CategoryId);
                productModel.CoverType = await _coverTypeRepo.GetAsync(x => x.CoverTypeId == productModel.CoverTypeId);

                if(productModel.Category == null || productModel.CoverType == null)
                {
                    _logger.LogError("Invalid Category Id or Cover Type Id.");
                    _response = Response(HttpStatusCode.NotFound, false, new List<string> { "Invalid Category Id or Cover Type Id." });
                    return NotFound(_response);
                }

                await _productRepo.CreateAsync(productModel);
                await _productRepo.SaveAsync();
                _response = Response(HttpStatusCode.Created);

                if (model.Image != null)
                {
                    _logger.LogInformation("CreateImage Method called.");
                    var imageResult = await CreateImage(model.Image, productModel.ProductId);
                    _logger.LogInformation("CreateImage Method done.");
                    if (imageResult.Count > 0)
                        _response = Response(HttpStatusCode.Created, ErrorMessages: imageResult, Result: _mapper.Map<ProductDTO>(productModel));
                    else
                        _response = Response(HttpStatusCode.Created, Result: _mapper.Map<ProductDTO>(productModel));
                }

                _logger.LogInformation("CreateProduct Method done. Created route to GetProduct method.");
                return CreatedAtRoute(nameof(GetProduct), new { id = productModel.ProductId }, _response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpPut("{id:int}", Name = "UpdateProduct")]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        public async Task<ActionResult<APIResponse>> UpdateProduct(int id, [FromForm] UpdateProductDTO model)
        {
            try
            {
                _logger.LogInformation("UpdateProduct Method called.");

                if (id == 0 || id != model.ProductId)
                {
                    _logger.LogError("Id is equal 0 or id is not equal to category id.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                ProductModel productModel = _mapper.Map<ProductModel>(model);
                productModel.Category = await _categoryRepo.GetAsync(x => x.CategoryId == productModel.CategoryId);
                productModel.CoverType = await _coverTypeRepo.GetAsync(x => x.CoverTypeId == productModel.CoverTypeId);

                if (productModel.Category == null || productModel.CoverType == null)
                {
                    _logger.LogError("Invalid Category Id or Cover Type Id.");
                    _response = Response(HttpStatusCode.NotFound, false, new List<string> { "Invalid Category Id or Cover Type Id." });
                    return NotFound(_response);
                }

                await _productRepo.UpdateAsync(productModel);

                if (model.Image != null)
                {
                    _logger.LogInformation("CreateImage Method called.");
                    var imageResult = await CreateImage(model.Image, productModel.ProductId);
                    _logger.LogInformation("CreateImage Method done.");
                    if (imageResult.Count > 0)
                        _response = Response(HttpStatusCode.NoContent, ErrorMessages: imageResult);
                    else
                        _response = Response(HttpStatusCode.NoContent);
                }

                _response = Response(HttpStatusCode.NoContent);
                _logger.LogInformation("UpdateProduct Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteProduct")]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation("DeleteProduct Method called.");

                if (id == 0)
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                var product = await _productRepo.GetAsync(x => x.ProductId == id);
                if (product == null)
                {
                    _logger.LogError("Product model is null.");
                    _response = Response(HttpStatusCode.NotFound, false);
                    return NotFound(_response);
                }

                var productPath = @"ProductImage\product-" + id;
                var finalPath = Path.Combine(_hostEnvironment.WebRootPath, productPath);

                if (Directory.Exists(finalPath))
                {
                    string[] filePaths = Directory.GetFiles(finalPath);

                    foreach (string filePath in filePaths)
                        System.IO.File.Delete(filePath);

                    _logger.LogError("Deleting images.");
                    Directory.Delete(finalPath);
                }

                await _productRepo.RemoveAsync(product);
                _response = Response(HttpStatusCode.NoContent);
                _logger.LogInformation("DeleteProduct Method done.");
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

        private async Task<List<string>> CreateImage(List<IFormFile> images, int id)
        {
            List<string> errorResults = new();
            var supportedTypes = new[] { "jpg", "jpeg", "png" };
            
            foreach (var file in images)
            {
                var fileExt = Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    _logger.LogError($"File extension of {file.FileName} is invalid - Only Upload jpg/jpeg/png image file.");
                    errorResults.Add($"File extension of {file.FileName} is invalid - Only Upload jpg/jpeg/png image file.");
                    continue;
                }

                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = $"{wwwRootPath}\\ProductImage\\product-{id}";

                var directoryLocation = Path.Combine(filePath, fileName);

                if (!Directory.Exists(filePath))
                {
                    _logger.LogInformation($"CreateDirectory in {filePath}.");
                    Directory.CreateDirectory(filePath);
                }

                using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                {
                    _logger.LogInformation($"Copy files to {directoryLocation}.");
                    file.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

                CreateProductImageDTO imageModel = new();
                ProductImageModel productImageModel = _mapper.Map<ProductImageModel>(imageModel);
                productImageModel.ImgUrl = $"{baseUrl}/ProductImage/product-{id}/{fileName}";
                productImageModel.ImgLocalPath = directoryLocation;
                productImageModel.ProductId = id;

                await _productImageRepo.CreateAsync(productImageModel);
                await _productImageRepo.SaveAsync();
            }

            return errorResults;
        }
    }
}
