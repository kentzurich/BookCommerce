using AutoMapper;
using BookCommerce_API.Models;
using BookCommerce_API.Models.DTO.CoverType;
using BookCommerce_API.Repository.CoverType;
using BookCommerce_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookCommerce_API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CoverTypeAPIController : ControllerBase
    {
        private readonly ICoverTypeRepository _coverTypeRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryAPIController> _logger;
        private APIResponse _response;

        public CoverTypeAPIController(ICoverTypeRepository coverTypeRepo, 
                                      IMapper mapper,
                                      ILogger<CategoryAPIController> logger)
        {
            _coverTypeRepo = coverTypeRepo;
            _mapper = mapper;
            _logger = logger;
            _logger.LogInformation("CoverTypeAPI Controller called.");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllCoverType()
        {
            try
            {
                _logger.LogInformation("GetAllCoverType Method called.");
                IEnumerable<CoverTypeModel> coverTypeList = await _coverTypeRepo.GetAllAsync();
                _response = Response(HttpStatusCode.OK, true, Result: _mapper.Map<IEnumerable<CoverTypeDTO>>(coverTypeList));
                _logger.LogInformation("GetAllCoverType Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpGet("{id:int}", Name = "GetCoverType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetCoverType(int id)
        {
            try
            {
                _logger.LogInformation("GetCoverType Method called.");

                if (id == 0)
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, isSuccess: false);
                    return BadRequest(_response);
                }

                var coverTypes = await _coverTypeRepo.GetAsync(x => x.CoverTypeId == id);

                if (coverTypes == null)
                {
                    _logger.LogError("CoverType Model is null.");
                    _response = Response(HttpStatusCode.NotFound, isSuccess: false);
                    return NotFound(_response);
                }

                _response = Response(HttpStatusCode.OK, Result: _mapper.Map<CoverTypeDTO>(coverTypes));
                _logger.LogInformation("GetCoverType Method done.");
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
		public async Task<ActionResult<APIResponse>> CreateCategory([FromBody] CreateCoverTypeDTO model)
        {
            try
            {
                _logger.LogInformation("CreateCategory Method called.");

                if (model == null)
                {
                    _logger.LogError("CreateCoverTypeDTO model is null.");
                    return NotFound(model);
                }

                CoverTypeModel coverTypeModel = _mapper.Map<CoverTypeModel>(model);
                await _coverTypeRepo.CreateAsync(coverTypeModel);
                await _coverTypeRepo.SaveAsync();

                _response = Response(HttpStatusCode.Created, Result: _mapper.Map<CoverTypeDTO>(coverTypeModel));
                _logger.LogInformation("CreateCategory Method done. Created route to GetCoverType method.");
                return CreatedAtRoute(nameof(GetCoverType), new { id = coverTypeModel.CoverTypeId }, _response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpPut("{id:int}", Name = "UpdateCoverType")]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> UpdateCoverType(int id, [FromBody] UpdateCoverTypeDTO model)
        {
            try
            {
                _logger.LogInformation("UpdateCoverType Method called.");

                if (id == 0 || id != model.CoverTypeId)
                {
                    _logger.LogError("Id is equal 0 or id is not equal to category id.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                CoverTypeModel coverTypeModel = _mapper.Map<CoverTypeModel>(model);
                await _coverTypeRepo.UpdateAsync(coverTypeModel);
                _response = Response(HttpStatusCode.NoContent);
                _logger.LogInformation("UpdateCoverType Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteCoverType")]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> DeleteCoverType(int id)
        {
            try
            {
                _logger.LogInformation("DeleteCoverType Method called.");

                if (id == 0)
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                var coverType = await _coverTypeRepo.GetAsync(x => x.CoverTypeId == id);
                if (coverType == null)
                {
                    _logger.LogError("CoverType model is null.");
                    _response = Response(HttpStatusCode.NotFound, false);
                    return NotFound(_response);
                }

                await _coverTypeRepo.RemoveAsync(coverType);
                _response = Response(HttpStatusCode.NoContent);
                _logger.LogInformation("DeleteCoverType Method done.");
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
