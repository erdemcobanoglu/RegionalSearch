using RegionalSearch.Application.Common.Interfaces;
using RegionalSearch.Application.DependencyInjection;      // AddApplicationServices için
using RegionalSearch.Infrastructure.DependencyInjection;
using RegionalSearch.Infrastructure.Persistence;  // AddInfrastructure için 
using Presentation.Services;  // Interface buradan gelir 


var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// 📌 Clean Architecture DI Registration
// ----------------------------------------------------

// Application (MediatR + AutoMapper + Validation)
builder.Services.AddApplicationServices();

// Infrastructure (EF Core + Identity + Logging)
builder.Services.AddInfrastructure(builder.Configuration);

// ----------------------------------------------------
// 📌 HttpContext & CurrentUserService DI (BUNLAR BURAYA)
// ----------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await ApplicationDbContextSeed.SeedAsync(context);
}


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
