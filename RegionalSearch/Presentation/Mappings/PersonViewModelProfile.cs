using AutoMapper;
using Presentation.Models;
using RegionalSearch.Application.Features.People.Commands; 
using RegionalSearch.Application.Features.People.Queries; 

namespace Presentation.Mappings
{
    public class PersonViewModelProfile : Profile
    {
        public PersonViewModelProfile()
        {
            // -----------------------------
            // CREATE
            // -----------------------------
            CreateMap<PersonCreateViewModel, CreatePersonCommand>()
                .ForMember(dest => dest.Photos, opt => opt.Ignore());

            // -----------------------------
            // LIST (Index)
            // -----------------------------
            // PersonDto -> PersonListViewModel (UI istediğinde)
            CreateMap<PersonDto, PersonListViewModel>();

            // -----------------------------
            // DETAILS
            // -----------------------------
            CreateMap<PersonDetailDto, PersonDetailViewModel>();

            // -----------------------------
            // EDIT (GET)
            // -----------------------------
            // PersonDetailDto → PersonEditViewModel
            CreateMap<PersonDetailDto, PersonEditViewModel>()
                .ForMember(dest => dest.NewPhotos, opt => opt.Ignore());   // Yeni foto alanı UI'da gelecek

            // -----------------------------
            // EDIT (POST)
            // -----------------------------
            // PersonEditViewModel → UpdatePersonCommand
            CreateMap<PersonEditViewModel, UpdatePersonCommand>()
                .ForMember(dest => dest.NewPhotos, opt => opt.Ignore()); // Foto dönüştürme Controller’da olacak

            // -----------------------------
            // DELETE (GET)
            // -----------------------------
            CreateMap<PersonDetailDto, PersonDeleteViewModel>();

            // -----------------------------
            // DELETE (POST)
            // -----------------------------
            CreateMap<PersonDeleteViewModel, DeletePersonCommand>();
        }
    }
}
