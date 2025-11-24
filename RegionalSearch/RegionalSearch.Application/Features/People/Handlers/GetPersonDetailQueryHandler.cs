using AutoMapper;
using MediatR;
using RegionalSearch.Application.Common.Interfaces;
using RegionalSearch.Application.Features.People.Queries;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Features.People.Handlers
{
    public class GetPersonDetailQueryHandler : IRequestHandler<GetPersonDetailQuery, PersonDetailDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPersonDetailQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PersonDetailDto?> Handle(GetPersonDetailQuery request, CancellationToken cancellationToken)
        {
            var person = await _context.People
                .Include(p => p.Organization)
                .Include(p => p.Category)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (person == null)
                return null;

            var dto = _mapper.Map<PersonDetailDto>(person);
            dto.Photos = person.Photos.Select(x => x.PhotoData).ToList();

            return dto;
        }
    }
}
