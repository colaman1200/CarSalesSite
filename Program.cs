using CarSalesSite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies; 

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Home/Error";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

builder.Services.AddControllersWithViews();



var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.UseAuthentication();  
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "account_login",
    pattern: "Account/Login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "account_profile",
    pattern: "Account/Profile",
    defaults: new { controller = "Account", action = "Profile" });





var additionalRoutes = new Dictionary<string, string>
{
    { "brands", "Brands/Index" },
    { "about", "Home/About" },
    { "importabout", "Home/ImportAbout" },
    { "clearanceofcars", "Home/ClearanceOfCars" },
    { "headpay", "Home/HeadPay" },
    { "delivery", "Home/Delivery" },
    { "choose", "Home/Choose" },
    { "peculiarities", "Home/Peculiarities" },
    { "faq", "Home/Faq" },
    { "docs", "Home/Docs" },
    { "importrules", "Home/ImportRules" },
    { "calculator", "Home/Calculator" },
    { "admin/cars", "Admin/Cars" },
    {"installcharger","Home/InstallCharger" },
    {"services","Home/Services" },
    {"privacy","Home/Privacy" }
};

// Для метода Filter (форма фильтрации)
app.MapControllerRoute(
    name: "car_filter_form",
    pattern: "cars/filter/form",
    defaults: new { controller = "Cars", action = "Filter" });

// Для метода FilterByType (кнопки нижнего хедера)
app.MapControllerRoute(
    name: "car_filter",
    pattern: "cars/filter/{category}/{value}",
    defaults: new { controller = "Cars", action = "FilterByCategory" });

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "images")),
    RequestPath = "/images"
});

app.MapControllerRoute(
    name: "add_favourite",
    pattern: "Account/AddToFavourites",
    defaults: new { controller = "Account", action = "AddToFavourites" });

app.MapControllerRoute(
    name: "remove_favourite",
    pattern: "Account/RemoveFromFavourites",
    defaults: new { controller = "Account", action = "RemoveFromFavourites" });

foreach (var route in additionalRoutes)
{
    app.MapControllerRoute(
        name: route.Key,
        pattern: route.Key,
        defaults: new { controller = route.Value.Split('/')[0], action = route.Value.Split('/')[1] });
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    
    if (!context.Users.Any(u => u.Role == "admin"))
    {
        var admin = new User
        {
            Name = "Kirik",
            Email = "kirillvasilevich1337@gmail.com", 
            Phone = "+375298247787",
            Role = "admin"
        };

        admin.SetPassword("admin123"); 
        context.Users.Add(admin);
        context.SaveChanges();
    }
}

app.Run();