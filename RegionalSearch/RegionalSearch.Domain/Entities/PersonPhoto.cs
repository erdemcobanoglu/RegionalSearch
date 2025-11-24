using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Domain.Entities
{
    public class PersonPhoto
    {
        public int Id { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        // Fotoğrafın dosya adı, yolu veya URL’si
        public string FileName { get; set; } = null!;
        public string? FilePath { get; set; }     // "images/people/xxx.jpg"
        public string? Url { get; set; }          // Eğer CDN veya cloud kullanırsan

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
