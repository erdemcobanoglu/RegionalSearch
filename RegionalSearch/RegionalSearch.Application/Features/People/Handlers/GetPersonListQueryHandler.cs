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
    public class GetPersonListQueryHandler : IRequestHandler<GetPersonListQuery, List<PersonDto>>
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
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<PersonDto>>(people);
        }
    }
}
