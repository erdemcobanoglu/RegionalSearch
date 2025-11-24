using Microsoft.EntityFrameworkCore;
using RegionalSearch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            // ---------------------
            // CATEGORY SEED
            // ---------------------
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Kategori A" },
                    new Category { Name = "Kategori B" }
                );

                await context.SaveChangesAsync();
            }

            // ---------------------
            // ORGANIZATION SEED
            // ---------------------
            if (!context.Organizations.Any())
            {
                context.Organizations.AddRange(
                    new Organization { Name = "Org 1" },
                    new Organization { Name = "Org 2" }
                );

                await context.SaveChangesAsync();
            }

            // ---------------------
            // PERSON SEED
            // ---------------------
            if (!context.People.Any())
            {
                var categoryId = context.Categories.First().Id;
                var orgId = context.Organizations.First().Id;

                var person = new Person
                {
                    FirstName = "Ali",
                    LastName = "Yılmaz",
                    BirthPlace = "İstanbul",
                    BirthDate = new DateTime(1990, 1, 1),
                    CategoryId = categoryId,
                    OrganizationId = orgId
                };

                context.People.Add(person);
                await context.SaveChangesAsync();

                // ---------------------
                // PERSON PHOTO SEED
                // ---------------------

                // Örnek Base64 PNG/JPEG (gerçek bir foto base64'si koyabilirsin)
                var sampleBase64 = @"
iVBORw0KGgoAAAANSUhEUgAAAAUA
AAAFCAYAAACNbyblAAAAHElEQVQI12P4
//8/w38GIAXDIBKE0DHxgljNBAAO
9TXL0Y4OHwAAAABJRU5ErkJggg==
";

                // Base64 → Byte[]
                byte[] imageBytes = Convert.FromBase64String(sampleBase64.Replace("\n", "").Replace("\r", ""));

                var photo = new PersonPhoto
                {
                    PersonId = person.Id,
                    PhotoData = imageBytes
                };

                context.PersonPhotos.Add(photo);

                await context.SaveChangesAsync();
            }
        }
    }
}
