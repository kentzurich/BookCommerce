using AutoMapper;
using BookCommerce_API.Models;
using BookCommerce_API.Models.DTO.Company;
using BookCommerce_API.Repository.Company;
using BookCommerce_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookCommerce_API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CompanyAPIController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyAPIController> _logger;
        private APIResponse _response;

        public CompanyAPIController(ICompanyRepository companyRepo, 
                                    IMapper mapper,
                                    ILogger<CompanyAPIController> logger)
        {
            _companyRepo = companyRepo;
            _mapper = mapper;
            _logger = logger;
            _logger.LogInformation("CompanyAPI Controller called.");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllCompany()
        {
            try
            {
                _logger.LogInformation("GetAllCompany Method called.");
                IEnumerable<CompanyModel> companyList = await _companyRepo.GetAllAsync();
                _response = Response(HttpStatusCode.OK, true, Result: _mapper.Map<IEnumerable<CompanyDTO>>(companyList));
                _logger.LogInformation("GetAllCompany Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpGet("{id:int}", Name = "GetCompany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetCompany(int id)
        {
            try
            {
                _logger.LogInformation("GetCompany Method called.");

                if (id == 0)
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, isSuccess: false);
                    return BadRequest(_response);
                }

                var company = await _companyRepo.GetAsync(x => x.CompanyId == id);

                if (company == null)
                {
                    _logger.LogError("Company Model is null.");
                    _response = Response(HttpStatusCode.NotFound, isSuccess: false);
                    return NotFound(_response);
                }

                _response = Response(HttpStatusCode.OK, Result: _mapper.Map<CompanyDTO>(company));
                _logger.LogInformation("GetCompany Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<APIResponse>> CreateCompany([FromBody] CreateCompanyDTO model)
        {
            try
            {
                _logger.LogInformation("CreateCompany Method called.");

                if (model == null)
                {
                    _logger.LogError("CreateCompanyDTO model is null.");
                    return NotFound(model);
                }

                CompanyModel companyModel = _mapper.Map<CompanyModel>(model);
                await _companyRepo.CreateAsync(companyModel);
                await _companyRepo.SaveAsync();

                _response = Response(HttpStatusCode.Created, Result: _mapper.Map<CompanyDTO>(companyModel));
                _logger.LogInformation("CreateCompany Method done. Created route to GetCompany method.");
                return CreatedAtRoute(nameof(GetCompany), new { id = companyModel.CompanyId }, _response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [HttpPut("{id:int}", Name = "UpdateCompany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> UpdateCompany(int id, [FromBody] UpdateCompanyDTO model)
        {
            try
            {
                _logger.LogInformation("UpdateCompany Method called.");

                if (id == 0 || id != model.CompanyId)
                {
                    _logger.LogError("Id is equal 0 or id is not equal to category id.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                CompanyModel companyModel = _mapper.Map<CompanyModel>(model);
                await _companyRepo.UpdateAsync(companyModel);
                _response = Response(HttpStatusCode.NoContent);
                _logger.LogInformation("UpdateCompany Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [HttpDelete("{id:int}", Name = "DeleteCompany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> DeleteCompany(int id)
        {
            try
            {
                _logger.LogInformation("DeleteCompany Method called.");

                if (id == 0)
                {
                    _logger.LogError("Id is equal to 0.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                var company = await _companyRepo.GetAsync(x => x.CompanyId == id);
                if (company == null)
                {
                    _logger.LogError("Category model is null.");
                    _response = Response(HttpStatusCode.NotFound, false);
                    return NotFound(_response);
                }

                await _companyRepo.RemoveAsync(company);
                _response = Response(HttpStatusCode.NoContent);
                _logger.LogInformation("DeleteCompany Method done.");
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
