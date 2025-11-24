Süper, şimdi Infrastructure + EF Core tarafına geçelim.
Amacımız:

* `AuditableEntity` alanlarını **otomatik** doldurmak
* `IsDeleted` ile **soft delete** yapmak
* Hepsini Clean Architecture’a uygun şekilde Infrastructure katmanına koymak

Aşağıdaki kodları kademeli olarak ekleyebilirsin.

---

## 1️⃣ Application katmanında “şu anki kullanıcı” arayüzü

**Proje:** `RegionalSearch.Application`
**Klasör:** `Common/Interfaces`

```csharp
namespace RegionalSearch.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
}
```

> Bu interface’i Infrastructure içinde implement edeceğiz, Presentation’da da DI container’a ekleyeceğiz.

---

## 2️⃣ Infrastructure’da DbContext

**Proje:** `RegionalSearch.Infrastructure`
**Klasör:** `Persistence`
**Sınıf:** `ApplicationDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using RegionalSearch.Application.Common.Interfaces;
using RegionalSearch.Domain.Common;
using RegionalSearch.Domain.Entities;

namespace RegionalSearch.Infrastructure.Persistence;

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
```

> Burada reflection + Expression ile **AuditableEntity’den türeyen tüm entity’lere** otomatik `IsDeleted == false` filter’ı koyduk. Böylece EF sorgularında silinmiş kayıtlar hiç gelmeyecek.

---

## 3️⃣ ICurrentUserService implementasyonu (Infrastructure)

**Proje:** `RegionalSearch.Infrastructure`
**Klasör:** `Identity` veya `Services`

```csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RegionalSearch.Application.Common.Interfaces;

namespace RegionalSearch.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
                return null;

            // Claim Type uygulamana göre değişebilir
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out var id))
                return id;

            return null;
        }
    }
}
```

---

## 4️⃣ Infrastructure için DI extension

**Proje:** `RegionalSearch.Infrastructure`
**Klasör:** `DependencyInjection`
**Sınıf:** `DependencyInjection.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegionalSearch.Application.Common.Interfaces;
using RegionalSearch.Infrastructure.Identity;
using RegionalSearch.Infrastructure.Persistence;

namespace RegionalSearch.Infrastructure;

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

        // Current user
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Buraya repository ve diğer servisleri de ekleyeceksin
        // services.AddScoped<IPersonRepository, PersonRepository>();

        return services;
    }
}
```

---

## 5️⃣ Presentation (MVC) tarafında Program.cs

MVC projenin `Program.cs` dosyasında:

```csharp
using RegionalSearch.Application;
using RegionalSearch.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ...

app.Run();
```

> `AddApplication()` için de Application katmanında benzer bir `DependencyInjection` sınıfı oluşturursun.

---

Şu anda:

* Tüm entity’lerin ortak alanları `AuditableEntity` içinde,
* EF Core, bu alanları CRUD sırasında **otomatik set ediyor**,
* `IsDeleted` ile soft delete çalışıyor,
* Sorgularda silinmiş kayıtlar görünmüyor,
* Kullanıcı bilgisi `ICurrentUserService` üzerinden geliyor (Claims’ten).

İstersen bir sonraki adımda:

* `Person` için bir **Repository + Service + CreatePersonCommand** örneği kurup
* MVC’de bir formdan veri + foto yükleyip nasıl kaydedeceğimizi uçtan uca yazalım.
