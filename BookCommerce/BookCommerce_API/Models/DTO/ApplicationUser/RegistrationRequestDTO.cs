namespace BookCommerce_API.Models.DTO.ApplicationUser
{
    public class RegistrationRequestDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public int CompanyId { get; set; }
        public string Role { get; set; }
    }
}
