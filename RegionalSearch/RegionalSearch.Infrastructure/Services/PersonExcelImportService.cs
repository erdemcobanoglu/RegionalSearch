using ClosedXML.Excel;
using RegionalSearch.Application.Common.Interfaces;
using RegionalSearch.Application.Features.People.Dtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

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

            // 🔹 Tüm resimleri (row, col) => byte[] olarak map'le
            var pictureMap = BuildPictureMap(sheet);

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
                    // 🔥 Foto artık hücre text’inden değil, Pictures koleksiyonundan geliyor
                    PhotoData = GetPhotoValue(rowIndex, headerMap, pictureMap, "photo")
                };

                result.Add(dto);
            }

            return result;
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

        /// <summary>
        /// Sayfadaki tüm resimleri (Row, Col) -> byte[] olarak map'ler.
        /// </summary>
        private Dictionary<(int Row, int Col), byte[]> BuildPictureMap(IXLWorksheet sheet)
        {
            var pictureMap = new Dictionary<(int Row, int Col), byte[]>();

            foreach (var picture in sheet.Pictures)
            {
                var addr = picture.TopLeftCell.Address;

                using var ms = new MemoryStream();
                picture.ImageStream.Position = 0;
                picture.ImageStream.CopyTo(ms);

                pictureMap[(addr.RowNumber, addr.ColumnNumber)] = ms.ToArray();
            }

            return pictureMap;
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

            string text = row.Cell(col).GetString().Trim();

            if (string.IsNullOrWhiteSpace(text))
                return null;

            // 🔥 Sadece yıl geldiyse (ör: 1998)
            if (int.TryParse(text, out int year) && year is >= 1900 and <= 2100)
                return new DateTime(year, 1, 1);

            // Normal tarih formatı dene
            if (DateTime.TryParse(text, new CultureInfo("tr-TR"), out var dt))
                return dt;

            return null;
        }

        /// <summary>
        /// Verilen satır için, Photo sütunundaki hücreye yapışık resmi döner.
        /// </summary>
        private byte[]? GetPhotoValue(
            int rowIndex,
            Dictionary<string, int> headerMap,
            Dictionary<(int Row, int Col), byte[]> pictureMap,
            string key)
        {
            key = Normalize(key);
            if (!headerMap.TryGetValue(key, out var colIndex))
                return null;

            // (rowIndex, photoColumn) kombinasyonuna bağlı resim varsa al
            if (pictureMap.TryGetValue((rowIndex, colIndex), out var bytes))
                return bytes;

            return null;
        }

        private string Normalize(string text)
        {
            text = text.ToLower().Trim();
            return text.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                       .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");
        }
    }
}
