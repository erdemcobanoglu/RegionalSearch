using RegionalSearch.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Domain.Entities
{
    public class PersonPhoto : AuditableEntity
    {
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        public string FileName { get; set; } = null!;
        public string? FilePath { get; set; }
        public string? Url { get; set; }
    }
}
