using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.ApplicationUser;
using BookCommerce_WEB.Models.DTO.Product;
using BookCommerce_WEB.Models.DTO.ShoppingCart;
using BookCommerce_WEB.Services.Product;
using BookCommerce_WEB.Services.ShoppingCart;
using BookCommerce_WEB.Services.UserAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BookCommerce_WEB.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserAuthService _userAuthService;

        public HomeController(ILogger<HomeController> logger,
                              IProductService productService,
                              IShoppingCartService shoppingCartService,
                              IUserAuthService userAuthService)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _userAuthService = userAuthService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<ProductDTO> products = null;
            var response = await _productService.GetAllAsync<APIResponse>();
            if(response.Result != null && response.IsSuccess)
                products = JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(response.Result.ToString());

            return View(products);

        }

        public async Task<IActionResult> Details(int productId)
        {
            ShoppingCartDTO cartDTO = new();
            var response = await _productService.GetAsync<APIResponse>(productId);
            if (response.Result != null && response.IsSuccess)
            {
                cartDTO.Count = 1;
                cartDTO.Product = JsonConvert.DeserializeObject<ProductDTO>(response.Result.ToString());
            }
            
            return View(cartDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [ActionName(nameof(Details))]
        public async Task<IActionResult> AddToCart(CreateShoppingCartDTO model)
        {
            APIResponse response = new();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var responseUserId = await _userAuthService.GetUserId<APIResponse>(claimsIdentity.Name);
            ApplicationUserDTO applicationUserDTO = JsonConvert.DeserializeObject<ApplicationUserDTO>(responseUserId.Result.ToString());
            model.ApplicationUserId = applicationUserDTO.Id;

            var responseAddToCart = await _shoppingCartService.AddToCart<APIResponse>(model);

            if (responseAddToCart.Result != null && responseAddToCart.IsSuccess)
            {
                var responseCountCart = await _shoppingCartService.CountShoppingCart<APIResponse>(model.ApplicationUserId);
                model = JsonConvert.DeserializeObject<CreateShoppingCartDTO>(responseCountCart.Result.ToString());
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, model.Count);

                TempData["success"] = "Cart updated successfully.";
            }
            else
            {
                TempData["error"] = responseAddToCart.ErrorMessages.FirstOrDefault();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}