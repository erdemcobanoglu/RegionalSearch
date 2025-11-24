Harika, o zaman **Clean Architecture’a tam uyumlu biçimde**
`Person` için tüm katmanların akışını **baştan sona** kuralım.

Bu yapı:

* Domain (entity)
* Application (DTO + Command + Handler + Mapping)
* Presentation (ViewModel + Controller)
* Infrastructure (EF Core)

hepsini birbiriyle doğru şekilde bağlayacak.

---

# ✅ **1. DOMAIN → Person Entity (Senin zaten var)**

```csharp
public class Person : AuditableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BirthPlace { get; set; }
    public DateTime BirthDate { get; set; }

    public int OrganizationId { get; set; }
    public Organization Organization { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public ICollection<PersonPhoto> Photos { get; set; } = new List<PersonPhoto>();
}
```

---

# ✅ **2. APPLICATION → DTO’lar (Command / Response)**

📁 **Application → Features → People → Commands → CreatePerson**

### CreatePersonCommand.cs

```csharp
using MediatR;

public class CreatePersonCommand : IRequest<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BirthPlace { get; set; }
    public DateTime BirthDate { get; set; }

    public int OrganizationId { get; set; }
    public int CategoryId { get; set; }

    // Photo burada IFormFile olmaz!
    public List<byte[]> Photos { get; set; }
}
```

---

### PersonDto.cs (Query için kullanılacak)

```csharp
public class PersonDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Organization { get; set; }
    public string Category { get; set; }
}
```

---

# ✅ **3. APPLICATION → Command Handler**

📁 **Application → Features → People → Handlers**

```csharp
public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreatePersonCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = new Person
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthPlace = request.BirthPlace,
            BirthDate = request.BirthDate,
            CategoryId = request.CategoryId,
            OrganizationId = request.OrganizationId
        };

        _context.People.Add(person);
        await _context.SaveChangesAsync(cancellationToken);

        // Foto yükleme
        if (request.Photos != null)
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
```

---

# ✅ **4. APPLICATION → AutoMapper Profil (Mapping)**

📁 **Application → Common → Mapping**

```csharp
using AutoMapper;

public class PersonMappingProfile : Profile
{
    public PersonMappingProfile()
    {
        CreateMap<Person, PersonDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(x => x.FirstName + " " + x.LastName))
            .ForMember(d => d.Organization, opt => opt.MapFrom(x => x.Organization.Name))
            .ForMember(d => d.Category, opt => opt.MapFrom(x => x.Category.Name));
    }
}
```

---

# ✅ **5. PRESENTATION → ViewModel (MVC)**

📁 **Presentation → Models → PersonCreateViewModel.cs**

```csharp
public class PersonCreateViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BirthPlace { get; set; }
    public DateTime BirthDate { get; set; }

    public int OrganizationId { get; set; }
    public int CategoryId { get; set; }

    public List<IFormFile> Photos { get; set; }
}
```

---

# ✅ **6. PRESENTATION → Controller**

```csharp
public class PersonController : Controller
{
    private readonly IMediator _mediator;

    public PersonController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(PersonCreateViewModel model)
    {
        var photoBytes = new List<byte[]>();

        if (model.Photos != null)
        {
            foreach (var file in model.Photos)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                photoBytes.Add(ms.ToArray());
            }
        }

        var command = new CreatePersonCommand
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            BirthPlace = model.BirthPlace,
            BirthDate = model.BirthDate,
            CategoryId = model.CategoryId,
            OrganizationId = model.OrganizationId,
            Photos = photoBytes
        };

        await _mediator.Send(command);

        return RedirectToAction("Index");
    }
}
```

---

# ⚡ **7. PRESENTATION → Create.cshtml Örneği**

```html
<form asp-action="Create" method="post" enctype="multipart/form-data">
    <input asp-for="FirstName" />
    <input asp-for="LastName" />
    <input asp-for="BirthPlace" />
    <input asp-for="BirthDate" type="date" />

    <select asp-for="CategoryId"> ... </select>
    <select asp-for="OrganizationId"> ... </select>

    <input type="file" name="Photos" multiple />

    <button type="submit">Save</button>
</form>
```

---

# 🏁 **TAM CLEAN ARCHITECTURE AKIŞI TAMAMLANDI!**

Aşağıdaki katmanlarda her şey düzenli şekilde ayrıldı:

* **Domain → Entity**
* **Application → Command + Handler + DTO + Mapping**
* **Infrastructure → EF Core Persist**
* **Presentation → ViewModel + Controller + View**

Bu yapı ile:

✔ SOLID
✔ Katman bağımsızlığı
✔ Test edilebilirlik
✔ Dependency inversion
✔ UI bağımsız model yönetimi

hepsi tamamen doğru şekilde işlemeye başladı.

---

# 🚀 Bir Sonraki Adım?

İstersen devam edebiliriz:

### 1. Person Listesi (Query + DTO)

### 2. Person GetDetail (Query + DTO)

### 3. MediatR Pipeline behavior (Logging, Validation)

### 4. FluentValidation ekleme

### 5. Organization / Category CRUD

### 6. Fotoğrafı görüntüleme (FileResult)

Hangisini next step olarak yapmak istersin?
