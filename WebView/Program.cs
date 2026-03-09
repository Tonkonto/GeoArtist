using GeoComponent;
using Microsoft.AspNetCore.Mvc;
using WebView.Middleware;
using WebView.Models.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Geo сервис
builder.Services.AddGeoComponent();

// Exception handling middleware
builder.Services
    .AddControllersWithViews()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var firstError = context.ModelState
                .Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Validation failed.";

            var response = new ApiErrorResponse
            {
                Error = "Validation failed",
                Details = firstError
            };

            return new BadRequestObjectResult(response);
        };
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
//app.UseAuthorization();
app.UseMiddleware<ApiExceptionMiddleware>();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
