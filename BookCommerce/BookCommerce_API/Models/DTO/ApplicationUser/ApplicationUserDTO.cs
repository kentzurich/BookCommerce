using BookCommerce_API.Models.DTO.Company;
using Microsoft.AspNetCore.Identity;

namespace BookCommerce_API.Models.DTO.ApplicationUser
{
    public class ApplicationUserDTO : IdentityUser
    {
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public CompanyDTO Company { get; set; }
        public string Role { get; set; }
    }
}
