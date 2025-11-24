using RegionalSearch.Application.DependencyInjection;      // AddApplication için
using RegionalSearch.Infrastructure.DependencyInjection;  // AddInfrastructure için

var builder = WebApplication.CreateBuilder(args);

// ⬇️ katmanların DI kayıtları
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
