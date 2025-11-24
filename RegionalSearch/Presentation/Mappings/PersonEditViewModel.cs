namespace Presentation.Mappings
{
    public class PersonEditViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string BirthPlace { get; set; } = null!;
        public DateTime BirthDate { get; set; }

        public int CategoryId { get; set; }
        public int OrganizationId { get; set; }

        public List<string>? ExistingPhotos { get; set; }

        public List<IFormFile>? NewPhotos { get; set; }
    }

}
