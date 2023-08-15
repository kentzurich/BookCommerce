using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.ApplicationUser;
using BookCommerce_WEB.Models.DTO.Company;
using BookCommerce_WEB.Services.Company;
using BookCommerce_WEB.Services.TokenProvider;
using BookCommerce_WEB.Services.UserAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace BookCommerce_WEB.Areas.Identity.Controllers
{
	[Area("Identity")]
    public class AuthController : Controller
    {
        private readonly IUserAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        private readonly ICompanyService _companyService;

        public AuthController(IUserAuthService authService,
                              ITokenProvider tokenProvider,
                              ICompanyService companyService)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
            _companyService = companyService;
        }
        public async Task<IActionResult> Login()
        {
            LoginRequestDTO loginRequestDTO = new();
            return View(loginRequestDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            APIResponse response = await _authService.LoginAsync<APIResponse>(model);
            if (response.Result != null)
            {
                TokenDTO tokenDTO = JsonConvert.DeserializeObject<TokenDTO>(response.Result.ToString());

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(tokenDTO.AccessToken);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(x => x.Type == "unique_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

				HttpContext.Response.Cookies.Append(StaticDetails.CookieRole, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value);

                _tokenProvider.SetToken(tokenDTO);
                return LocalRedirect("~/Customer/Home/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessages.FirstOrDefault());
                return View(model);
            }
        }

        public async Task<IActionResult> Register()
        {
            RegistrationRequestDTO model = new();
            await RoleList(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationRequestDTO model)
        {
            if (model == null)
                return BadRequest();

            if (string.IsNullOrEmpty(model.Role))
                model.Role = StaticDetails.ROLE_USER_INDIVIDUAL;

            APIResponse response = await _authService.RegisterAsync<APIResponse>(model);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                await RoleList(model);
                ModelState.AddModelError(string.Empty, response.ErrorMessages.FirstOrDefault());
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            var token = _tokenProvider.GetToken();
            await _authService.LogoutAsync<APIResponse>(token);
            _tokenProvider.ClearToken();
            HttpContext.Response.Cookies.Delete(StaticDetails.CookieRole);
			return LocalRedirect("~/Customer/Home/Index");
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        private async Task RoleList(RegistrationRequestDTO model)
        {
            var getAllRolesResponse = await _authService.GetAllRoles<APIResponse>();
            var getAllCompanyResponse = await _companyService.GetAllAsync<APIResponse>();

            if (getAllRolesResponse.Result != null && getAllCompanyResponse.Result != null)
            {
                var userRoles = JsonConvert.DeserializeObject<IEnumerable<UserRoleDTO>>(getAllRolesResponse.Result.ToString());
                var companyList = JsonConvert.DeserializeObject<IEnumerable<CompanyDropdownDTO>>(getAllCompanyResponse.Result.ToString());

                model.RoleList = userRoles.Select(x => x.Name).Select(x => new SelectListItem
                {
                    Text = x,
                    Value = x
                });

                model.CompanyList = companyList.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CompanyId.ToString()
                });
            }
        }
    }
}
