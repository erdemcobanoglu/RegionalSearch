using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Features.People.Commands
{
    public class UpdatePersonCommand : IRequest
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string BirthPlace { get; set; } = null!;
        public DateTime BirthDate { get; set; }

        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }

        // Edit ekranında gelen yeni fotoğraflar
        public List<byte[]>? NewPhotos { get; set; }
    }
}
