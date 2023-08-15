using AutoMapper;
using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.Category;
using BookCommerce_WEB.Models.DTO.CoverType;
using BookCommerce_WEB.Models.DTO.Product;
using BookCommerce_WEB.Models.DTO.ProductImage;
using BookCommerce_WEB.Models.ViewModel;
using BookCommerce_WEB.Services.Category;
using BookCommerce_WEB.Services.CoverType;
using BookCommerce_WEB.Services.Product;
using BookCommerce_WEB.Services.ProductImage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BookCommerce_WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;
        private readonly ICategoryService _categoryService;
        private readonly ICoverTypeService _coverTypeService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, 
                                 IProductImageService productImageService,
                                 ICategoryService categoryService,
                                 ICoverTypeService coverTypeService,
                                 IMapper mapper)
        {
            _productService = productService;
            _productImageService = productImageService;
            _categoryService = categoryService;
            _coverTypeService = coverTypeService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? productId)
        {
            ProductViewModel vm = await PopulateDropdown();
            if (productId == null || productId == 0)
                return View(vm);

            var response = await _productService.GetAsync<APIResponse>((int)productId);
            if (response.Result != null && response.IsSuccess)
            {
                vm.ProductModel = JsonConvert.DeserializeObject<ProductModel>(response.Result.ToString());
                return View(_mapper.Map<ProductViewModel>(vm));
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductViewModel vm, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                APIResponse response = new();
                bool isSave;
                if (vm.ProductModel.ProductId == 0)
                {
                    CreateProductDTO dto = _mapper.Map<CreateProductDTO>(vm.ProductModel);
                    dto.Image = files;
                    dto.CategoryId = vm.ProductModel.Category.CategoryId;
                    dto.CoverTypeId = vm.ProductModel.CoverType.CoverTypeId;
                    response = await _productService.CreateAsync<APIResponse>(dto);
                    isSave = true;
                }
                else
                {
                    UpdateProductDTO dto = _mapper.Map<UpdateProductDTO>(vm.ProductModel);
                    dto.Image = files;
                    dto.CategoryId = vm.ProductModel.Category.CategoryId;
                    dto.CoverTypeId = vm.ProductModel.CoverType.CoverTypeId;
                    response = await _productService.UpdateAsync<APIResponse>(dto);
                    isSave = false;
                }

                if (!response.IsSuccess)
                {
                    TempData["error"] = (response.ErrorMessages != null && response.ErrorMessages.Count > 0 ?
                        response.ErrorMessages[0] : "Error encountered.");
                    vm = await PopulateDropdown();
                    return View(vm);
                }

                TempData["success"] = isSave ? "Product created successfully." : "Product updated successfully.";
                return RedirectToAction(nameof(Upsert), new { productId = vm.ProductModel.ProductId });
            }
            else
            {
                await PopulateDropdown();
            }

            return View(vm);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _productService.DeleteAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                return Json(new { success = true, message = "Product deleted successfully." });
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

        public async Task<IActionResult> DeleteImage(int productImageId)
        {
            ProductImageDTO productImageDTO = new();
            var response = await _productImageService.DeleteAsync<APIResponse>(productImageId);
            if(response != null && response.IsSuccess)
            {
                productImageDTO = JsonConvert.DeserializeObject<ProductImageDTO>(response.Result.ToString());
                TempData["success"] = "Image deleted successfully.";
            }
            else
            {
                TempData["error"] = response.ErrorMessages.FirstOrDefault();
            }

            return RedirectToAction(nameof(Upsert), new { productId = productImageDTO.ProductId });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<ProductDTO> productList = new();

            var response = await _productService.GetAllAsync<APIResponse>();

            if (response.Result != null && response.IsSuccess)
                productList = JsonConvert.DeserializeObject<List<ProductDTO>>(response.Result.ToString());

            return Json(new { data = productList });
        }

        private async Task<ProductViewModel> PopulateDropdown()
        {
            ProductViewModel vm = new()
            {
                ProductModel = new()
            };
            var category = await _categoryService.GetAllAsync<APIResponse>();
            var coverType = await _coverTypeService.GetAllAsync<APIResponse>();

            if(category != null)
            {
                var categoryList = JsonConvert.DeserializeObject<IEnumerable<CategoryDTO>>(category.Result.ToString());
                vm.CategoryList = categoryList.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CategoryId.ToString()
                });
            }

            if(coverType != null)
            {
                var coverTypeList = JsonConvert.DeserializeObject<IEnumerable<CoverTypeDTO>>(coverType.Result.ToString());
                vm.CoverTypeList = coverTypeList.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CoverTypeId.ToString()
                });
            }

            return vm;
        }
    }
}
