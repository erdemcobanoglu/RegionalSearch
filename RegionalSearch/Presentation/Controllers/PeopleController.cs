using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegionalSearch.Application.Features.People.Commands;
using System.IO;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class PeopleController : Controller
    {
        private readonly IMediator _mediator;

        public PeopleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /People/Import
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        // POST: /People/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Lütfen bir Excel dosyası seçin.");
                return View();
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".xlsx" && ext != ".xls")
            {
                ModelState.AddModelError(string.Empty, "Sadece .xlsx veya .xls uzantılı Excel dosyaları yükleyebilirsiniz.");
                return View();
            }

            using (var stream = file.OpenReadStream())
            {
                await _mediator.Send(new ImportPeopleFromExcelCommand(stream));
            }

            TempData["SuccessMessage"] = "Excel dosyası başarıyla içeri aktarıldı.";
            return RedirectToAction(nameof(Import));
        }
    }
}
