using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Features.People.Dtos
{
    public class PersonImportRowDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? OrganizationName { get; set; }
        public string? CategoryName { get; set; }
        public byte[]? PhotoData { get; set; }
    }
}
