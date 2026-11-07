using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    // тестовые данные
    public static List<Spravka> Spravki = new List<Spravka>
    {
        new Spravka { Id = 1, Name = "Справка 083", PatientId = 1, SpravkaTypeId = 1, Price = 100, IssuedAt = DateTime.Now.AddDays(-10) },
        new Spravka { Id = 2, Name = "Справка в бассейн", PatientId = 1, SpravkaTypeId = 3, Price = 300, IssuedAt = DateTime.Now.AddDays(-3) },
        new Spravka { Id = 3, Name = "Санитарная книжка", PatientId = 2, SpravkaTypeId = 2, Price = 750, IssuedAt = DateTime.Now.AddDays(-1) },
    };

    private static int _nextId = 4;

    // GET /api/spravki                      все справки
    // GET /api/spravki?patientId=1          справки одного пациента
    // GET /api/spravki?sort=price           по цене (sort=date: по дате, новые сверху)
    [HttpGet]
    public List<Spravka> GetAll(int? patientId, string? sort)
    {
        List<Spravka> result = Spravki;

        if (patientId != null)
        {
            result = result.Where(s => s.PatientId == patientId).ToList();
        }

        if (sort == "price")
        {
            result = result.OrderBy(s => s.Price).ToList();
        }
        else if (sort == "date")
        {
            result = result.OrderByDescending(s => s.IssuedAt).ToList();
        }

        return result;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Spravka> GetById(int id)
    {
        Spravka? spravka = Spravki.FirstOrDefault(s => s.Id == id);
        if (spravka == null)
        {
            return NotFound();
        }
        return spravka;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Spravka> Create(Spravka spravka)
    {
        string? error = CheckAndFill(spravka);
        if (error != null)
        {
            return BadRequest(error);
        }

        spravka.Id = _nextId;
        _nextId++;
        spravka.IssuedAt = DateTime.Now;
        Spravki.Add(spravka);
        return CreatedAtAction(nameof(GetById), new { id = spravka.Id }, spravka);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Spravka> Update(int id, Spravka spravka)
    {
        Spravka? existing = Spravki.FirstOrDefault(s => s.Id == id);
        if (existing == null)
        {
            return NotFound();
        }

        string? error = CheckAndFill(spravka);
        if (error != null)
        {
            return BadRequest(error);
        }

        existing.PatientId = spravka.PatientId;
        existing.SpravkaTypeId = spravka.SpravkaTypeId;
        existing.Name = spravka.Name;
        existing.Price = spravka.Price;
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Delete(int id)
    {
        Spravka? spravka = Spravki.FirstOrDefault(s => s.Id == id);
        if (spravka == null)
        {
            return NotFound();
        }

        Spravki.Remove(spravka);
        return NoContent();
    }

    // Проверяет пациента и вид справки, и берёт название и цену из каталога.
    // Возвращает текст ошибки или null, если всё хорошо.
    private string? CheckAndFill(Spravka spravka)
    {
        Patient? patient = PatientsController.Patients.FirstOrDefault(p => p.Id == spravka.PatientId);
        if (patient == null)
        {
            return "Пациент с таким номером не найден. Сначала создайте пациента в картотеке";
        }

        SpravkaType? type = SpravkaTypesController.Types.FirstOrDefault(t => t.Id == spravka.SpravkaTypeId);
        if (type == null)
        {
            return "Такого вида справки нет в каталоге. Посмотрите список в /api/SpravkaTypes";
        }

        // справка для детей только детям (до 18 лет)
        int age = DateTime.Now.Year - patient.BirthDate.Year;
        if (type.ForChildren && age >= 18)
        {
            return "Эта справка только для детей, а пациенту уже есть 18 лет. Выберите другой вид справки";
        }

        spravka.Name = type.Name;
        spravka.Price = type.Price;
        return null;
    }
}
