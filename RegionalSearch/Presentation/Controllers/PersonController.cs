using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models; 
using RegionalSearch.Application.Features.People.Commands;
using RegionalSearch.Application.Features.People.Queries;

namespace Presentation.Controllers
{
    public class PersonController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PersonController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        // ----------------------------------------------------
        // LIST  (READ - LIST)
        // ----------------------------------------------------
        public async Task<IActionResult> Index()
        {
            // Application katmanında: GetPersonListQuery -> List<PersonDto>
            var people = await _mediator.Send(new GetPersonListQuery());

            return View(people); // View, model olarak List<PersonDto> alır
        } 

        // ----------------------------------------------------
        // DETAILS  (READ - SINGLE)
        // ----------------------------------------------------
        public async Task<IActionResult> Details(int id)
        {
            var person = await _mediator.Send(new GetPersonDetailQuery { Id = id });

            if (person == null)
                return NotFound();

            return View(person); // View, PersonDetailDto alır
        }

        // ----------------------------------------------------
        // CREATE  (CREATE)
        // ----------------------------------------------------
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PersonCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ViewModel -> Command (AutoMapper)
            var command = _mapper.Map<CreatePersonCommand>(model);

            // Fotoğrafları IFormFile -> byte[] dönüştür
            if (model.Photos != null && model.Photos.Any())
            {
                command.Photos = new List<byte[]>();

                foreach (var file in model.Photos)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    command.Photos.Add(ms.ToArray());
                }
            }

            await _mediator.Send(command);

            return RedirectToAction(nameof(Index));
        }

        // ----------------------------------------------------
        // EDIT  (UPDATE)
        // ----------------------------------------------------
        public async Task<IActionResult> Edit(int id)
        {
            var person = await _mediator.Send(new GetPersonDetailQuery { Id = id });

            if (person == null)
                return NotFound();

            // PersonDetailDto -> PersonEditViewModel (AutoMapper ile)
            var vm = _mapper.Map<PersonEditViewModel>(person);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PersonEditViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ViewModel -> UpdatePersonCommand
            var command = _mapper.Map<UpdatePersonCommand>(model);

            // Foto güncellenecekse yine IFormFile -> byte[] dönüştür
            if (model.NewPhotos != null && model.NewPhotos.Any())
            {
                command.NewPhotos = new List<byte[]>();

                foreach (var file in model.NewPhotos)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    command.NewPhotos.Add(ms.ToArray());
                }
            }

            await _mediator.Send(command);

            return RedirectToAction(nameof(Index));
        }

        // ----------------------------------------------------
        // DELETE  (DELETE)
        // ----------------------------------------------------
        public async Task<IActionResult> Delete(int id)
        {
            var person = await _mediator.Send(new GetPersonDetailQuery { Id = id });

            if (person == null)
                return NotFound();

            return View(person); // Basit bir confirm sayfası
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _mediator.Send(new DeletePersonCommand { Id = id });

            return RedirectToAction(nameof(Index));
        }
    }
}
