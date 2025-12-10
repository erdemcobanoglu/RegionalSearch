using MediatR;
using Microsoft.EntityFrameworkCore;
using RegionalSearch.Application.Common.Interfaces;
using RegionalSearch.Application.Features.People.Commands;
using RegionalSearch.Domain.Entities;
using System;
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

            var orgDict = await _db.Organizations
                .ToDictionaryAsync(o => o.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);

            var catDict = await _db.Categories
                .ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);

            foreach (var row in rows)
            {
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
            }

            await _db.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
