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

        // Ad - Soyad
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // İstersen FullName otomatik hesaplatılabilir
        public string FullName => $"{FirstName} {LastName}".Trim();

        // Doğum
        public string? BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }

        // Kategori & Organization
        public string? Organization { get; set; }
        public string? Category { get; set; }

        // Foto
        public string? PhotoBase64 { get; set; } // byte[] depoluyorsan dönüşüm ile UI'de gösterebilirsin.
    }

}
