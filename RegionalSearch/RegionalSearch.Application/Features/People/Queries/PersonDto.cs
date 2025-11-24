using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Features.People.Queries
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Organization { get; set; }
        public string Category { get; set; }
    }
}
