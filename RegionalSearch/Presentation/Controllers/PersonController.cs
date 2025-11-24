using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using RegionalSearch.Application.Features.People.Commands.CreatePerson;

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

            return RedirectToAction("Index");
        }
    }
}
