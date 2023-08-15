using AutoMapper;
using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.Category;
using BookCommerce_WEB.Services.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookCommerce_WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? categoryId)
        {
            CategoryModel model = new();
            if (categoryId == null || categoryId == 0)
                return View(model);

            var response = await _categoryService.GetAsync<APIResponse>((int)categoryId);
            if (response.Result != null && response.IsSuccess)
            {
                CategoryDTO categoryDTO = JsonConvert.DeserializeObject<CategoryDTO>(response.Result.ToString());
                return View(_mapper.Map<CategoryModel>(categoryDTO));
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CategoryModel model)
        {
            if(ModelState.IsValid)
            {
                APIResponse response = new();
                bool isSave;
                if(model.CategoryId == 0)
                {
                    CreateCategoryDTO dto = _mapper.Map<CreateCategoryDTO>(model);
                    response = await _categoryService.CreateAsync<APIResponse>(dto);
                    isSave = true;
                }
                else
                {
                    UpdateCategoryDTO dto = _mapper.Map<UpdateCategoryDTO>(model);
                    response = await _categoryService.UpdateAsync<APIResponse>(dto);
                    isSave = false;
                }

                if (!response.IsSuccess)
                {
                    TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0 ?
                        response.ErrorMessages[0] : "Error encountered.");
                    return View(model);
                }

                TempData["success"] = isSave ? "Category created successfully." : "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _categoryService.DeleteAsync<APIResponse>(id);
            if(response != null && response.IsSuccess)
            {
                return Json(new { success = true, message = "Category deleted successfully." });
            } 
            else
            {
                return Json(new { success = false, message = (response.ErrorMessages != null && response.ErrorMessages.Count > 0 ?
                   response.ErrorMessages[0] : "Error encountered.")
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<CategoryDTO> categoryList = new();

            var response = await _categoryService.GetAllAsync<APIResponse>();

            if (response.Result != null && response.IsSuccess)
                categoryList = JsonConvert.DeserializeObject<List<CategoryDTO>>(response.Result.ToString());

            return Json(new { data = categoryList });
        }
    }
}
