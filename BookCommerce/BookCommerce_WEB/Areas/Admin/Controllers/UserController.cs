using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.ApplicationUser;
using BookCommerce_WEB.Models.DTO.Company;
using BookCommerce_WEB.Models.ViewModel;
using BookCommerce_WEB.Services.Company;
using BookCommerce_WEB.Services.UserAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BookCommerce_WEB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
    public class UserController : Controller
    {
        private readonly IUserAuthService _userAuthService;
        private readonly ICompanyService _companyService;

        public UserController(IUserAuthService userAuthService, ICompanyService companyService)
        {
            _userAuthService = userAuthService;
            _companyService = companyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RoleManagement(string userId)
        {
            var responseGetUser = await _userAuthService.GetAsync<APIResponse>(userId);
            var getAllRolesResponse = await _userAuthService.GetAllRoles<APIResponse>();
            var getAllCompanyResponse = await _companyService.GetAllAsync<APIResponse>();

            var userRoles = JsonConvert.DeserializeObject<IEnumerable<UserRoleDTO>>(getAllRolesResponse.Result.ToString());
            var companyList = JsonConvert.DeserializeObject<IEnumerable<CompanyDropdownDTO>>(getAllCompanyResponse.Result.ToString());

            RoleManagementViewModel RoleVM = new()
            {
                ApplicationUser = JsonConvert.DeserializeObject<ApplicationUserDTO>(responseGetUser.Result.ToString()),
                RoleList = userRoles.Select(x => x.Name).Select(x => new SelectListItem
                {
                    Text = x,
                    Value = x
                }),
                CompanyList = companyList.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CompanyId.ToString()
                })
            };

            return View(RoleVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleManagement(RoleManagementViewModel roleVM)
        {
            var responseGetUser = await _userAuthService.GetAsync<APIResponse>(roleVM.ApplicationUser.Id);
            ApplicationUserDTO applicationUserDTO = JsonConvert.DeserializeObject<ApplicationUserDTO>(responseGetUser.Result.ToString());
            var oldRole = applicationUserDTO.Role;

            if (roleVM.ApplicationUser.Role != applicationUserDTO.Role)
            {
                var newRole = roleVM.ApplicationUser.Role;
                //a role was updated
                if (newRole == StaticDetails.ROLE_USER_COMPANY)
                {
                    applicationUserDTO.CompanyId = roleVM.ApplicationUser.CompanyId;
                }
                else
                {
                    if (oldRole == StaticDetails.ROLE_USER_COMPANY)
                        applicationUserDTO.CompanyId = null;
                }

                await _userAuthService.UpdateApplicationUserAsync<APIResponse>(applicationUserDTO);
                //error pa dito
                await _userAuthService.UpdateUserRoleAsync<APIResponse>(oldRole, applicationUserDTO);
            }
            else
            {
                if (applicationUserDTO.Role == StaticDetails.ROLE_USER_COMPANY && !applicationUserDTO.CompanyId.Equals(roleVM.ApplicationUser.CompanyId))
                {
                    applicationUserDTO.CompanyId = roleVM.ApplicationUser.CompanyId;
                    await _userAuthService.UpdateApplicationUserAsync<APIResponse>(applicationUserDTO);
                }
            }

            TempData["success"] = "Role updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlockAcc([FromBody] string id)
        {
            var responseGetUser = await _userAuthService.GetAsync<APIResponse>(id);
            ApplicationUserDTO applicationUserDTO = JsonConvert.DeserializeObject<ApplicationUserDTO>(responseGetUser.Result.ToString());

            if (applicationUserDTO == null)
                return Json(new { success = false, message = "Error while locking/unlocking." });

            if (applicationUserDTO.LockoutEnd is not null && applicationUserDTO.LockoutEnd > DateTime.Now)
            {
                applicationUserDTO.LockoutEnd = null;
                applicationUserDTO.LockoutEnabled = false;
            }
            else
            {
                applicationUserDTO.LockoutEnabled = true;
                applicationUserDTO.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            await _userAuthService.UpdateApplicationUserAsync<APIResponse>(applicationUserDTO);

            return Json(new { success = true, message = "Operation successful." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var responseUserList = await _userAuthService.GetAllAsync<APIResponse>();
            var userList = JsonConvert.DeserializeObject<List<ApplicationUserDTO>>(responseUserList.Result.ToString());
            return Json(new { data = userList });
        }
    }
}
