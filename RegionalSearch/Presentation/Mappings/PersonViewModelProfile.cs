using AutoMapper;
using Presentation.Models;
using RegionalSearch.Application.Features.People.Commands.CreatePerson;

namespace Presentation.Mappings
{
    public class PersonViewModelProfile : Profile
    {
        public PersonViewModelProfile()
        {
            CreateMap<PersonCreateViewModel, CreatePersonCommand>()
                // Foto’yu burada da ignore ediyoruz; IFormFile -> byte[] dönüşümünü controller’da yapacağız.
                .ForMember(dest => dest.Photos, opt => opt.Ignore());
        }
    }
}
