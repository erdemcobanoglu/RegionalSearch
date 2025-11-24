using Microsoft.Extensions.DependencyInjection;
using RegionalSearch.Application.Common.Mapping;
using RegionalSearch.Application.Features.People.Commands.CreatePerson;

namespace RegionalSearch.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AutoMapper profilleri otomatik bulsun
            services.AddAutoMapper(typeof(PersonMappingProfile).Assembly);

            // MediatR komut ve handler’larını tarasın
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(CreatePersonCommand).Assembly));

            return services;
        }
    }
}
