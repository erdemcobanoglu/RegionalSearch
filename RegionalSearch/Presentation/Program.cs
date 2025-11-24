using RegionalSearch.Application.DependencyInjection;      // AddApplicationServices için
using RegionalSearch.Infrastructure.DependencyInjection;  // AddInfrastructure için

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// 📌 Clean Architecture DI Registration
// ----------------------------------------------------

// Application (MediatR + AutoMapper + Validation)
builder.Services.AddApplicationServices();

// Infrastructure (EF Core + Identity + Logging)
builder.Services.AddInfrastructure(builder.Configuration);

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ----------------------------------------------------
// 📌 Middlewares
// ----------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ----------------------------------------------------
// 📌 Default Route
// ----------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
