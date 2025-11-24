using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Features.People.Queries
{
    public class PersonDetailDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get; set; } = null!;

        public string BirthPlace { get; set; } = null!;
        public DateTime BirthDate { get; set; }

        public string Organization { get; set; } = null!;
        public string Category { get; set; } = null!;

        public List<byte[]> Photos { get; set; } = new();
    }
}
