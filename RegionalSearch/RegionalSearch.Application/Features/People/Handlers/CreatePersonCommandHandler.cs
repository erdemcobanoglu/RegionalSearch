using AutoMapper;
using MediatR;
using RegionalSearch.Application.Common.Interfaces;
using RegionalSearch.Application.Features.People.Commands.CreatePerson;
using RegionalSearch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Features.People.Handlers
{
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreatePersonCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            // Command -> Person
            var person = _mapper.Map<Person>(request);

            _context.People.Add(person);
            await _context.SaveChangesAsync(cancellationToken);

            // Fotoğraf kaydetme (AutoMapper dışında, özel işlem)
            if (request.Photos != null && request.Photos.Any())
            {
                foreach (var bytes in request.Photos)
                {
                    var photo = new PersonPhoto
                    {
                        PersonId = person.Id,
                        PhotoData = bytes
                    };

                    _context.PersonPhotos.Add(photo);
                }

                await _context.SaveChangesAsync(cancellationToken);
            }

            return person.Id;
        }
    }
}
