using BookCommerce_Utility;
using BookCommerce_WEB.Extensions;
using BookCommerce_WEB.Hubs;
using BookCommerce_WEB.Models.AutoMapper;
using BookCommerce_WEB.Services.APIMessageRequestBuilder;
using BookCommerce_WEB.Services.Base;
using BookCommerce_WEB.Services.Category;
using BookCommerce_WEB.Services.Company;
using BookCommerce_WEB.Services.CoverType;
using BookCommerce_WEB.Services.Order;
using BookCommerce_WEB.Services.Product;
using BookCommerce_WEB.Services.ProductImage;
using BookCommerce_WEB.Services.ShoppingCart;
using BookCommerce_WEB.Services.TokenProvider;
using BookCommerce_WEB.Services.UserAuth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using ProductService = BookCommerce_WEB.Services.Product.ProductService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(x => x.Filters.Add(new AuthExceptionRedirection()));
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddSignalR();

builder.Services.AddHttpClient<IUserAuthService, UserAuthService>();
builder.Services.AddHttpClient<ICategoryService, CategoryService>();
builder.Services.AddHttpClient<ICoverTypeService, CoverTypeService>();
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddHttpClient<IProductImageService, ProductImageService>();
builder.Services.AddHttpClient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddHttpClient<IOrderService, OrderService>();

builder.Services.AddScoped<IUserAuthService, UserAuthService>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICoverTypeService, CoverTypeService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IAPIMessageRequestBuilder, APIMessageRequestBuilder>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.LoginPath = "/Identity/Auth/Login";
        options.AccessDeniedPath = "/Identity/Auth/AccessDenied";
        options.SlidingExpiration = true;
    });
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/hubs/notification");

app.Run();
