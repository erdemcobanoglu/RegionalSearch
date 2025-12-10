using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegionalSearch.Application.Common.Interfaces;
using RegionalSearch.Infrastructure.Persistence;
using RegionalSearch.Infrastructure.Services; 
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RegionalSearch.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // 👇 IApplicationDbContext -> ApplicationDbContext binding
            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());

            // Current user
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); 
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Buraya repository ve diğer servisleri de ekleyeceksin
            // services.AddScoped<IPersonRepository, PersonRepository>();

            // 🔹 Excel import servisi
            services.AddTransient<IPersonExcelImportService, PersonExcelImportService>(); 

            return services;
        }
    }
}
