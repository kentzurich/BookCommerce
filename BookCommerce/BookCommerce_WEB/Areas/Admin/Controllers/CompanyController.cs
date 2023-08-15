using AutoMapper;
using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.Company;
using BookCommerce_WEB.Services.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookCommerce_WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;

        public CompanyController(ICompanyService companyService, IMapper mapper)
        {
            _companyService = companyService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? companyId)
        {
            CompanyModel model = new();
            if (companyId == null || companyId == 0)
                return View(model);

            var response = await _companyService.GetAsync<APIResponse>((int)companyId);
            if (response.Result != null && response.IsSuccess)
            {
                CompanyDTO companyDTO = JsonConvert.DeserializeObject<CompanyDTO>(response.Result.ToString());
                return View(_mapper.Map<CompanyModel>(companyDTO));
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CompanyModel model)
        {
            if (ModelState.IsValid)
            {
                APIResponse response = new();
                bool isSave;
                if (model.CompanyId == 0)
                {
                    CreateCompanyDTO dto = _mapper.Map<CreateCompanyDTO>(model);
                    response = await _companyService.CreateAsync<APIResponse>(dto);
                    isSave = true;
                }
                else
                {
                    UpdateCompanyDTO dto = _mapper.Map<UpdateCompanyDTO>(model);
                    response = await _companyService.UpdateAsync<APIResponse>(dto);
                    isSave = false;
                }

                if (!response.IsSuccess)
                {
                    TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0 ?
                        response.ErrorMessages[0] : "Error encountered.");
                    return View(model);
                }

                TempData["success"] = isSave ? "Company created successfully." : "Company updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _companyService.DeleteAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                return Json(new { success = true, message = "Company deleted successfully." });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = (response.ErrorMessages != null && response.ErrorMessages.Count > 0 ?
                   response.ErrorMessages[0] : "Error encountered.")
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<CompanyDTO> companyList = new();

            var response = await _companyService.GetAllAsync<APIResponse>();

            if (response.Result != null && response.IsSuccess)
                companyList = JsonConvert.DeserializeObject<List<CompanyDTO>>(response.Result.ToString());

            return Json(new { data = companyList });
        }
    }
}
