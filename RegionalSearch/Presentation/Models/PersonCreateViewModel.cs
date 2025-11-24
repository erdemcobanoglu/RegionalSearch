namespace Presentation.Models
{
    public class PersonCreateViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthPlace { get; set; }
        public DateTime BirthDate { get; set; }

        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }

        public List<IFormFile> Photos { get; set; }
    }
}
