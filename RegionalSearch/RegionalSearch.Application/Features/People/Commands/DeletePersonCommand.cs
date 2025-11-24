using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Features.People.Commands
{
    public class DeletePersonCommand : IRequest   // 👈 public
    {
        public int Id { get; set; }
    }
}
