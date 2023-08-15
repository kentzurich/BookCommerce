using AutoMapper;
using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.CoverType;
using BookCommerce_WEB.Services.CoverType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookCommerce_WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
    public class CoverTypeController : Controller
    {
        private readonly ICoverTypeService _coverTypeService;
        private readonly IMapper _mapper;

        public CoverTypeController(ICoverTypeService coverTypeService, IMapper mapper)
        {
            _coverTypeService = coverTypeService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? coverTypeId)
        {
            CoverTypeModel model = new();
            if (coverTypeId == null || coverTypeId == 0)
                return View(model);

            var response = await _coverTypeService.GetAsync<APIResponse>((int)coverTypeId);
            if (response.Result != null && response.IsSuccess)
            {
                CoverTypeDTO coverTypeDTO = JsonConvert.DeserializeObject<CoverTypeDTO>(response.Result.ToString());
                return View(_mapper.Map<CoverTypeModel>(coverTypeDTO));
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CoverTypeModel model)
        {
            if (ModelState.IsValid)
            {
                APIResponse response = new();
                bool isSave;
                if (model.CoverTypeId == 0)
                {
                    CreateCoverTypeDTO dto = _mapper.Map<CreateCoverTypeDTO>(model);
                    response = await _coverTypeService.CreateAsync<APIResponse>(dto);
                    isSave = true;
                }
                else
                {
                    UpdateCoverTypeDTO dto = _mapper.Map<UpdateCoverTypeDTO>(model);
                    response = await _coverTypeService.UpdateAsync<APIResponse>(dto);
                    isSave = false;
                }

                if (!response.IsSuccess)
                {
                    TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0 ?
                        response.ErrorMessages[0] : "Error encountered.");
                    return View(model);
                }

                TempData["success"] = isSave ? "Cover Type created successfully." : "Cover Type updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _coverTypeService.DeleteAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                return Json(new { success = true, message = "Cover Type deleted successfully." });
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
            List<CoverTypeDTO> coverTypeList = new();

            var response = await _coverTypeService.GetAllAsync<APIResponse>();

            if (response.Result != null && response.IsSuccess)
                coverTypeList = JsonConvert.DeserializeObject<List<CoverTypeDTO>>(response.Result.ToString());

            return Json(new { data = coverTypeList });
        }
    }
}
