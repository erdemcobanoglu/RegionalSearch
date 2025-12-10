using MediatR;
using Microsoft.EntityFrameworkCore;
using RegionalSearch.Application.Common.Interfaces;
using RegionalSearch.Application.Features.People.Commands;
using RegionalSearch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Features.People.Handlers
{
    public class ImportPeopleFromExcelCommandHandler
        : IRequestHandler<ImportPeopleFromExcelCommand, Unit>
    {
        private readonly IPersonExcelImportService _importService;
        private readonly IApplicationDbContext _db;

        public ImportPeopleFromExcelCommandHandler(
            IPersonExcelImportService importService,
            IApplicationDbContext db)
        {
            _importService = importService;
            _db = db;
        }

        public async Task<Unit> Handle(
            ImportPeopleFromExcelCommand request,
            CancellationToken cancellationToken)
        {
            var rows = _importService.Import(request.ExcelStream);

            // Mevcut Organization & Category cache
            var orgDict = await _db.Organizations
                .ToDictionaryAsync(o => o.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);

            var catDict = await _db.Categories
                .ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);

            // Mevcut kişiler + fotolar (upsert için)
            var existingPeople = await _db.People
                .Include(p => p.Photos)
                .Include(p => p.Organization)
                .Include(p => p.Category)
                .ToListAsync(cancellationToken);

            var personDict = existingPeople
                .ToDictionary(
                    p => BuildPersonKey(p.FirstName, p.LastName, p.BirthDate),
                    StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                // ---- Organization upsert ----
                Organization? org = null;
                if (!string.IsNullOrWhiteSpace(row.OrganizationName))
                {
                    if (!orgDict.TryGetValue(row.OrganizationName, out org))
                    {
                        org = new Organization { Name = row.OrganizationName! };
                        _db.Organizations.Add(org);
                        orgDict[row.OrganizationName!] = org;
                    }
                }

                // ---- Category upsert ----
                Category? cat = null;
                if (!string.IsNullOrWhiteSpace(row.CategoryName))
                {
                    if (!catDict.TryGetValue(row.CategoryName, out cat))
                    {
                        cat = new Category { Name = row.CategoryName! };
                        _db.Categories.Add(cat);
                        catDict[row.CategoryName!] = cat;
                    }
                }

                // ---- Person upsert ----
                var key = BuildPersonKey(row.FirstName, row.LastName, row.BirthDate);

                if (personDict.TryGetValue(key, out var existingPerson))
                {
                    // ✅ Güncelleme
                    existingPerson.BirthPlace = row.BirthPlace;
                    existingPerson.BirthDate = row.BirthDate;
                    existingPerson.Organization = org;
                    existingPerson.Category = cat;

                    if (row.PhotoData is { Length: > 0 })
                    {
                        var existingPhoto = existingPerson.Photos.FirstOrDefault();
                        if (existingPhoto != null)
                        {
                            // Var olan fotoğrafı güncelle
                            existingPhoto.PhotoData = row.PhotoData;
                        }
                        else
                        {
                            // Hiç foto yoksa ekle
                            existingPerson.Photos.Add(new PersonPhoto
                            {
                                PhotoData = row.PhotoData
                            });
                        }
                    }
                }
                else
                {
                    // ✅ Yeni kayıt
                    var person = new Person
                    {
                        FirstName = row.FirstName,
                        LastName = row.LastName,
                        BirthPlace = row.BirthPlace,
                        BirthDate = row.BirthDate,
                        Organization = org,
                        Category = cat
                    };

                    if (row.PhotoData is { Length: > 0 })
                    {
                        person.Photos.Add(new PersonPhoto
                        {
                            PhotoData = row.PhotoData
                        });
                    }

                    _db.People.Add(person);
                    personDict[key] = person; // sonraki satırlarda aynı kişi gelirse güncelleyelim
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        private static string BuildPersonKey(string firstName, string lastName, DateTime? birthDate)
        {
            var fn = (firstName ?? "").Trim().ToLowerInvariant();
            var ln = (lastName ?? "").Trim().ToLowerInvariant();
            var bd = birthDate?.ToString("yyyy-MM-dd") ?? "";
            return $"{fn}|{ln}|{bd}";
        }
    }
}
