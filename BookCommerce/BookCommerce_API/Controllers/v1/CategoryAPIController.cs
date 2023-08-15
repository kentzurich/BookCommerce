using AutoMapper;
using BookCommerce_API.Models;
using BookCommerce_API.Models.DTO.Category;
using BookCommerce_API.Repository.Category;
using BookCommerce_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookCommerce_API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoryAPIController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryAPIController> _logger;
        private APIResponse _response;

        public CategoryAPIController(ICategoryRepository categoryRepo, 
                                     IMapper mapper, 
                                     ILogger<CategoryAPIController> logger)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
            _logger = logger;
            _logger.LogInformation("CategoryAPI Controller called.");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllCategory()
        {
            try
            {
                _logger.LogInformation("GetAllCategory Method called.");
                IEnumerable<CategoryModel> categoryList = await _categoryRepo.GetAllAsync();
                _response = Response(HttpStatusCode.OK, true, Result: _mapper.Map<IEnumerable<CategoryDTO>>(categoryList));
                _logger.LogInformation("GetAllCategory Method done.");
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetCategory(int id)
        {
            try
            {
                _logger.LogInformation("GetCategory Method called.");

                if (id == 0)
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, isSuccess: false);
                    return BadRequest(_response);
                }

                var categories = await _categoryRepo.GetAsync(x => x.CategoryId == id);

                if (categories == null)
                {
                    _logger.LogError("Categories Model is null.");
                    _response = Response(HttpStatusCode.NotFound, isSuccess: false);
                    return NotFound(_response);
                }

                _response = Response(HttpStatusCode.OK, Result: _mapper.Map<CategoryDTO>(categories));
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
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<APIResponse>> CreateCategory([FromBody] CreateCategoryDTO model)
        {
            try
            {
                _logger.LogInformation("CreateCategory Method called.");

                if (model == null)
                {
                    _logger.LogError("CreateCategoryDTO model is null.");
                    return NotFound(model);
                }

                CategoryModel categoryModel = _mapper.Map<CategoryModel>(model);
                await _categoryRepo.CreateAsync(categoryModel);
                await _categoryRepo.SaveAsync();

                _response = Response(HttpStatusCode.Created, Result: _mapper.Map<CategoryDTO>(categoryModel));
                _logger.LogInformation("CreateCategory Method done. Created route to GetCategory method.");
                return CreatedAtRoute(nameof(GetCategory), new { id = categoryModel.CategoryId }, _response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpPut("{id:int}", Name = "UpdateCategory")]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> UpdateCategory(int id, [FromBody] UpdateCategoryDTO model)
        {
            try
            {
                _logger.LogInformation("UpdateCategory Method called.");

                if (id == 0 || id != model.CategoryId)
                {
                    _logger.LogError("Id is equal 0 or id is not equal to category id.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                CategoryModel categoryModel = _mapper.Map<CategoryModel>(model);
                await _categoryRepo.UpdateAsync(categoryModel);
                _response = Response(HttpStatusCode.NoContent);
                _logger.LogInformation("UpdateCategory Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> DeleteCategory(int id)
        {
            try
            {
                _logger.LogInformation("DeleteCategory Method called.");

                if (id == 0)
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                var category = await _categoryRepo.GetAsync(x => x.CategoryId == id);
                if (category == null)
                {
                    _logger.LogError("Category model is null.");
                    _response = Response(HttpStatusCode.NotFound, false);
                    return NotFound(_response);
                }

                await _categoryRepo.RemoveAsync(category);
                _response = Response(HttpStatusCode.NoContent);
                _logger.LogInformation("DeleteCategory Method done.");
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
