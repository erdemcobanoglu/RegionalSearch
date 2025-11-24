namespace Presentation.Mappings
{
    public class PersonDetailViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string BirthPlace { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string Category { get; set; } = null!;
        public string Organization { get; set; } = null!;

        public List<string>? PhotoUrls { get; set; }
    }

}
