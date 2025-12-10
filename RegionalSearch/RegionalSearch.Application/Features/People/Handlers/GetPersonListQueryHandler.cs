using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegionalSearch.Application.Common.Interfaces;
using RegionalSearch.Application.Features.People.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RegionalSearch.Application.Features.People.Handlers
{
    public class GetPersonListQueryHandler
        : IRequestHandler<GetPersonListQuery, List<PersonDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPersonListQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PersonDto>> Handle(GetPersonListQuery request, CancellationToken cancellationToken)
        {
            var people = await _context.People
                .Include(p => p.Organization)
                .Include(p => p.Category)
                .Include(p => p.Photos)
                .ToListAsync(cancellationToken);

            var list = _mapper.Map<List<PersonDto>>(people);

            // 🔥 Fotoğraf Base64 mapping burada tamamlanıyor
            foreach (var dto in list)
            {
                var person = people.First(x => x.Id == dto.Id);
                var photo = person.Photos.FirstOrDefault();

                dto.PhotoBase64 = photo != null
                    ? Convert.ToBase64String(photo.PhotoData)
                    : null;
            }

            return list;
        }
    }
}
