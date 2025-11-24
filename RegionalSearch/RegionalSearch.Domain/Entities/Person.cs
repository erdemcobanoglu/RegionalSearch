using RegionalSearch.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Domain.Entities
{
    public class Person : AuditableEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string? BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }

        public int? OrganizationId { get; set; }
        public int? CategoryId { get; set; }

        public Organization? Organization { get; set; }
        public Category? Category { get; set; }

        public ICollection<PersonPhoto> Photos { get; set; } = new List<PersonPhoto>();
    }
}
