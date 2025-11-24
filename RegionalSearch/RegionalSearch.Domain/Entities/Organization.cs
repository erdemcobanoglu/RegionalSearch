using RegionalSearch.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Domain.Entities
{
    public class Organization : AuditableEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<Person> People { get; set; } = new List<Person>();
    }
}
