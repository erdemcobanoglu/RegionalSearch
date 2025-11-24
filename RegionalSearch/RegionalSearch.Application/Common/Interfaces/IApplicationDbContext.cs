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
        DbSet<Person> People { get; }
        DbSet<PersonPhoto> PersonPhotos { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
