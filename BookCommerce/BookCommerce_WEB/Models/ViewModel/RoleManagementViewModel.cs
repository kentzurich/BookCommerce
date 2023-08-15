using BookCommerce_WEB.Models.DTO.ApplicationUser;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookCommerce_WEB.Models.ViewModel
{
    public class RoleManagementViewModel
    {
        public ApplicationUserDTO ApplicationUser { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
        public IEnumerable<SelectListItem> CompanyList { get; set; }
    }
}
