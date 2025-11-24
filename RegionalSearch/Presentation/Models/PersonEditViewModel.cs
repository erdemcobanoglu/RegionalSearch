namespace Presentation.Models
{
    public class PersonEditViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string BirthPlace { get; set; } = null!;
        public DateTime BirthDate { get; set; }

        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }

        // Var olan fotoğraflar (readonly gösterim için)
        public List<string>? ExistingPhotoUrls { get; set; }

        // Yeni eklenecek fotoğraflar
        public List<IFormFile>? NewPhotos { get; set; }
    }
}
