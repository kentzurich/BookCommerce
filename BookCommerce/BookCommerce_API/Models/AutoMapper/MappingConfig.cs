using AutoMapper;
using BookCommerce_API.Models.DTO.ApplicationUser;
using BookCommerce_API.Models.DTO.Category;
using BookCommerce_API.Models.DTO.Company;
using BookCommerce_API.Models.DTO.CoverType;
using BookCommerce_API.Models.DTO.Order.Details;
using BookCommerce_API.Models.DTO.Order.Header;
using BookCommerce_API.Models.DTO.Product;
using BookCommerce_API.Models.DTO.ProductImage;
using BookCommerce_API.Models.DTO.ShoppingCart;

namespace BookCommerce_API.Models.AutoMapper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            MapCategory();
            MapCoverType();
            MapCompany();
            MapProduct();
            MapUser();
            MapOrder();
            ShoppingCart();
        }

        private void MapCategory()
        {
            CreateMap<CategoryModel, CategoryDTO>().ReverseMap();
            CreateMap<CategoryModel, CreateCategoryDTO>().ReverseMap();
            CreateMap<CategoryModel, UpdateCategoryDTO>().ReverseMap();
        }

        private void MapCoverType()
        {
            CreateMap<CoverTypeModel, CoverTypeDTO>().ReverseMap();
            CreateMap<CoverTypeModel, CreateCoverTypeDTO>().ReverseMap();
            CreateMap<CoverTypeModel, UpdateCoverTypeDTO>().ReverseMap();
        }

        private void MapCompany()
        {
            CreateMap<CompanyModel, CompanyDTO>().ReverseMap();
            CreateMap<CompanyModel, CreateCompanyDTO>().ReverseMap();
            CreateMap<CompanyModel, UpdateCompanyDTO>().ReverseMap();
        }

        private void MapProduct()
        {
            CreateMap<ProductModel, ProductDTO>().ReverseMap();
            CreateMap<ProductModel, CreateProductDTO>().ReverseMap();
            CreateMap<ProductModel, UpdateProductDTO>().ReverseMap();

            CreateMap<ProductImageModel, ProductImageDTO>().ReverseMap();
            CreateMap<ProductImageModel, CreateProductImageDTO>().ReverseMap();
        }

        private void MapUser()
        {
            CreateMap<ApplicationUserModel, UserDTO>().ReverseMap();
            CreateMap<ApplicationUserModel, ApplicationUserDTO>().ReverseMap();
        }

        private void MapOrder()
        {
            CreateMap<OrderHeaderModel, OrderHeaderDTO>().ReverseMap();
            CreateMap<OrderHeaderModel, CreateOrderHeaderDTO>().ReverseMap();
            CreateMap<OrderHeaderModel, UpdateOrderHeaderDTO>().ReverseMap();

            CreateMap<OrderDetailsModel, OrderDetailsDTO>().ReverseMap();
            CreateMap<OrderDetailsModel, CreateOrderDetailsDTO>().ReverseMap();
            CreateMap<OrderDetailsModel, UpdateOrderDetailsDTO>().ReverseMap();
        }

        private void ShoppingCart()
        {
            CreateMap<ShoppingCartModel, ShoppingCartDTO>().ReverseMap();
            CreateMap<ShoppingCartModel, CreateShoppingCartDTO>().ReverseMap();
        }
    }
}
