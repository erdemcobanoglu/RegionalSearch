using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Features.People.Queries
{
    public class GetPersonDetailQuery : IRequest<PersonDetailDto?>
    {
        public int Id { get; set; }
    }
}
