using PriceComparisonMVC.Filter;
using PriceComparisonMVC.Infrastructure;
using PriceComparisonMVC.Services;

var builder = WebApplication.CreateBuilder(args);

ConfigurationService.ConfigureServices(builder);

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IApiService, ApiService>();
builder.Services.AddSingleton<TokenManager>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

//Додайте цей рядок перед викликом builder.Build()
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<UserInfoActionFilter>();
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

//app.UseCookiePolicy(new CookiePolicyOptions
//{
//    MinimumSameSitePolicy = SameSiteMode.Strict,
//    Secure = CookieSecurePolicy.Always
//});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


// Маршрути для статичних сторінок з футера
app.MapControllerRoute(
    name: "about",
    pattern: "about",
    defaults: new { controller = "StaticPages", action = "About" });

app.MapControllerRoute(
    name: "howWeWork",
    pattern: "how-we-work",
    defaults: new { controller = "StaticPages", action = "HowWeWork" });

app.MapControllerRoute(
    name: "partners",
    pattern: "partners",
    defaults: new { controller = "StaticPages", action = "Partners" });

app.MapControllerRoute(
    name: "termsOfUse",
    pattern: "terms-of-use",
    defaults: new { controller = "StaticPages", action = "TermsOfUse" });

app.MapControllerRoute(
    name: "privacyPolicy",
    pattern: "privacy-policy",
    defaults: new { controller = "StaticPages", action = "PrivacyPolicy" });

app.MapControllerRoute(
    name: "productReviews",
    pattern: "product-reviews",
    defaults: new { controller = "StaticPages", action = "ProductReviews" });

app.MapControllerRoute(
    name: "storeReviews",
    pattern: "store-reviews",
    defaults: new { controller = "StaticPages", action = "StoreReviews" });

app.MapControllerRoute(
    name: "howToChooseProduct",
    pattern: "how-to-choose-product",
    defaults: new { controller = "StaticPages", action = "HowToChooseProduct" });

app.MapControllerRoute(
    name: "faq",
    pattern: "faq",
    defaults: new { controller = "StaticPages", action = "FAQ" });

app.MapControllerRoute(
    name: "contactUs",
    pattern: "contact-us",
    defaults: new { controller = "StaticPages", action = "ContactUs" });

app.MapControllerRoute(
    name: "siteMap",
    pattern: "site-map",
    defaults: new { controller = "StaticPages", action = "SiteMap" });

// Дефолтний маршрут повинен бути останнім
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
