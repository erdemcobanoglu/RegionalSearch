using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RegionalSearch.Application.Common.Interfaces; 


namespace RegionalSearch.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Build configuration manually (migration sırasında kullanılır)
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connection = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connection);

            // Migration sırasında CurrentUser lazım değil → boş bir service veriyoruz
            return new ApplicationDbContext(optionsBuilder.Options, new DesignTimeCurrentUserService());
        }

        // Boş bir ICurrentUserService implementasyonu
        private class DesignTimeCurrentUserService : ICurrentUserService
        {
            public int? UserId => null;
        }
    }
}
