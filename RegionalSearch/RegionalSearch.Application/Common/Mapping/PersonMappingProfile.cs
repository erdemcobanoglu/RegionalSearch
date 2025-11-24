using AutoMapper;
using RegionalSearch.Application.Features.People.Commands.CreatePerson;
using RegionalSearch.Application.Features.People.Queries;
using RegionalSearch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RegionalSearch.Application.Common.Mapping
{
    public class PersonMappingProfile : Profile
    {
        public PersonMappingProfile()
        {
            // Entity -> DTO
            CreateMap<Person, PersonDto>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(x => x.FirstName + " " + x.LastName))
                .ForMember(d => d.Organization, opt => opt.MapFrom(x => x.Organization.Name))
                .ForMember(d => d.Category, opt => opt.MapFrom(x => x.Category.Name));

            // Command -> Entity
            CreateMap<CreatePersonCommand, Person>()
                // Fotoğrafları burada AutoMapper’a bırakmıyoruz, handler’da özel işliyoruz
                .ForMember(d => d.Photos, opt => opt.Ignore());
        }
    }
}
