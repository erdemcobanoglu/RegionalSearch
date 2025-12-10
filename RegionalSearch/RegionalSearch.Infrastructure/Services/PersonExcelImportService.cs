using ClosedXML.Excel;
using RegionalSearch.Application.Common.Interfaces;
using RegionalSearch.Application.Features.People.Dtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Infrastructure.Services
{
    public class PersonExcelImportService : IPersonExcelImportService
    {
        public List<PersonImportRowDto> Import(Stream excelStream)
        {
            var result = new List<PersonImportRowDto>();

            using var workbook = new XLWorkbook(excelStream);
            var sheet = workbook.Worksheet(1); // ilk sayfa

            var header = sheet.Row(1);
            var headerMap = BuildHeaderMap(header);

            int lastRow = sheet.LastRowUsed().RowNumber();

            for (int rowIndex = 2; rowIndex <= lastRow; rowIndex++)
            {
                var row = sheet.Row(rowIndex);
                if (row.IsEmpty()) continue;

                var dto = new PersonImportRowDto
                {
                    FirstName = GetValue(row, headerMap, "ad"),
                    LastName = GetValue(row, headerMap, "soyad"),
                    BirthPlace = GetValue(row, headerMap, "dogum yeri"),
                    BirthDate = GetDateValue(row, headerMap, "dogum tarihi"),
                    OrganizationName = GetValue(row, headerMap, "organizasyon"),
                    CategoryName = GetValue(row, headerMap, "kategori"),
                    PhotoData = GetPhotoValue(row, headerMap, "photo")
                };

                result.Add(dto);
            }

            return result; // 🟢 Burada artık return var
        }


        // ----------------- Helper Methodlar -----------------

        private Dictionary<string, int> BuildHeaderMap(IXLRow row)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var cell in row.CellsUsed())
            {
                var name = Normalize(cell.GetString());
                map[name] = cell.Address.ColumnNumber;
            }
            return map;
        }

        private string GetValue(IXLRow row, Dictionary<string, int> map, string key)
        {
            key = Normalize(key);
            if (!map.TryGetValue(key, out var col)) return string.Empty;
            return row.Cell(col).GetString().Trim();
        }

        private DateTime? GetDateValue(IXLRow row, Dictionary<string, int> map, string key)
        {
            key = Normalize(key);
            if (!map.TryGetValue(key, out var col)) return null;

            string text = row.Cell(col).GetString();
            if (DateTime.TryParse(text, new CultureInfo("tr-TR"), out var dt)) return dt;
            return null;
        }

        private byte[]? GetPhotoValue(IXLRow row, Dictionary<string, int> map, string key)
        {
            key = Normalize(key);
            if (!map.TryGetValue(key, out var col)) return null;

            var text = row.Cell(col).GetString();
            if (string.IsNullOrWhiteSpace(text)) return null;

            try
            {
                // Base64 ise decode edilir
                return Convert.FromBase64String(text);
            }
            catch { return null; }
        }

        private string Normalize(string text)
        {
            text = text.ToLower().Trim();
            return text.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                       .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");
        }
    }
}
