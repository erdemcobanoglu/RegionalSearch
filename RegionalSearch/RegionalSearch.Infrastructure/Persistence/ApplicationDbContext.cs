using RegionalSearch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;                // 👈 ÖNEMLİ
using RegionalSearch.Application.Common.Interfaces;  // ICurrentUserService için
using RegionalSearch.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace RegionalSearch.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Person> People => Set<Person>();
        public DbSet<PersonPhoto> PersonPhotos => Set<PersonPhoto>();
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AuditableEntity için global query filter (soft delete)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // IsDeleted == false filtresi
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var prop = Expression.Property(parameter, nameof(AuditableEntity.IsDeleted));
                    var compare = Expression.Equal(prop, Expression.Constant(false));
                    var lambda = Expression.Lambda(compare, parameter);

                    modelBuilder
                        .Entity(entityType.ClrType)
                        .HasQueryFilter(lambda);
                }
            }

            // İstersen burada Fluent API config'lerini çağırırsın
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyAuditInformation();
            return base.SaveChanges();
        }

        private void ApplyAuditInformation()
        {
            var now = DateTime.UtcNow;
            var userId = _currentUserService.UserId;

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = now;
                        entry.Entity.CreatedPersonId = userId;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = now;
                        entry.Entity.UpdatedPersonId = userId;
                        // IsDeleted'i burada elleme; soft delete için ayrı senaryo
                        break;

                    case EntityState.Deleted:
                        // Soft delete
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.UpdatedDate = now;
                        entry.Entity.UpdatedPersonId = userId;
                        break;
                }
            }
        }
    }
}
