using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        // Kayıt ne zaman oluşturuldu
        public DateTime CreatedDate { get; set; }

        // En son ne zaman güncellendi
        public DateTime? UpdatedDate { get; set; }

        // Kaydı kim oluşturdu (PersonId ile ilişkilendireceksin)
        public int? CreatedPersonId { get; set; }

        // Kaydı en son kim güncelledi
        public int? UpdatedPersonId { get; set; }

        // Soft delete için flag
        public bool IsDeleted { get; set; } = false;
    }
}
