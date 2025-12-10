using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Features.People.Commands
{
    public record ImportPeopleFromExcelCommand(Stream ExcelStream) : IRequest<Unit>;

}
