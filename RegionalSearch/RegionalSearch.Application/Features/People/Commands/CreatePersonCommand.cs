using MediatR;

namespace RegionalSearch.Application.Features.People.Commands
{
    public class CreatePersonCommand : IRequest<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthPlace { get; set; }
        public DateTime BirthDate { get; set; }

        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }

        // Photo burada IFormFile olmaz!
        public List<byte[]> Photos { get; set; }
    }
}
