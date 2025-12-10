using RegionalSearch.Application.Features.People.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Common.Interfaces
{
    public interface IPersonExcelImportService
    {
        List<PersonImportRowDto> Import(Stream excelStream);
    }
}
