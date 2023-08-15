using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.ApplicationUser;
using BookCommerce_WEB.Services.ShoppingCart;
using BookCommerce_WEB.Services.UserAuth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BookCommerce_WEB.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserAuthService _userAuthService;

        public ShoppingCartViewComponent(IShoppingCartService shoppingCartService,
                                         IUserAuthService userAuthService)
        {
            _shoppingCartService = shoppingCartService;
            _userAuthService = userAuthService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var responseUserId = await _userAuthService.GetUserId<APIResponse>(claimsIdentity.Name);
            if(responseUserId.Result != null && responseUserId.IsSuccess)
            {
                ApplicationUserDTO applicationUserDTO = JsonConvert.DeserializeObject<ApplicationUserDTO>(responseUserId.Result.ToString());

                if (HttpContext.Session.GetInt32(StaticDetails.SessionCart) is null)
                {
                    var responseCountCart = await _shoppingCartService.CountShoppingCart<APIResponse>(applicationUserDTO.Id);
                    if (responseCountCart.Result != null && responseCountCart.IsSuccess)
                    {
                        ShoppingCartModel shoppingCartModel = JsonConvert.DeserializeObject<ShoppingCartModel>(responseCountCart.Result.ToString());
                        HttpContext.Session.SetInt32(StaticDetails.SessionCart, shoppingCartModel.Count);
                    }
                }

                return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
