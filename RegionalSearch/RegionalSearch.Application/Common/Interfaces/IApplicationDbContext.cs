using Microsoft.EntityFrameworkCore;
using RegionalSearch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Person> People { get; set; }
        DbSet<PersonPhoto> PersonPhotos { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Organization> Organizations { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
