using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SpravkaTypesController : ControllerBase
{
    // каталог справок, заполнен сразу
    public static List<SpravkaType> Types = new List<SpravkaType>
    {
        new SpravkaType { Id = 1, Name = "Справка 083", Price = 100, ValidDays = 30, NeedsDoctorExam = true, ForChildren = false },
        new SpravkaType { Id = 2, Name = "Санитарная книжка", Price = 750, ValidDays = 365, NeedsDoctorExam = true, ForChildren = false },
        new SpravkaType { Id = 3, Name = "Справка в бассейн", Price = 300, ValidDays = 14, NeedsDoctorExam = true, ForChildren = false },
        new SpravkaType { Id = 4, Name = "Справка в детский лагерь", Price = 250, ValidDays = 30, NeedsDoctorExam = true, ForChildren = true },
        new SpravkaType { Id = 5, Name = "Справка для водительских прав", Price = 500, ValidDays = 365, NeedsDoctorExam = true, ForChildren = false },
    };

    private static int _nextId = 6;

    [HttpGet]
    public List<SpravkaType> GetAll()
    {
        return Types;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<SpravkaType> GetById(int id)
    {
        SpravkaType? type = Types.FirstOrDefault(t => t.Id == id);
        if (type == null)
        {
            return NotFound();
        }
        return type;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<SpravkaType> Create(SpravkaType type)
    {
        string? error = Validate(type);
        if (error != null)
        {
            return BadRequest(error);
        }

        type.Id = _nextId;
        _nextId++;
        Types.Add(type);
        return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<SpravkaType> Update(int id, SpravkaType type)
    {
        SpravkaType? existing = Types.FirstOrDefault(t => t.Id == id);
        if (existing == null)
        {
            return NotFound();
        }

        string? error = Validate(type);
        if (error != null)
        {
            return BadRequest(error);
        }

        existing.Name = type.Name;
        existing.Price = type.Price;
        existing.ValidDays = type.ValidDays;
        existing.NeedsDoctorExam = type.NeedsDoctorExam;
        existing.ForChildren = type.ForChildren;
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Delete(int id)
    {
        SpravkaType? type = Types.FirstOrDefault(t => t.Id == id);
        if (type == null)
        {
            return NotFound();
        }

        // нельзя удалить вид, по которому уже выписаны справки
        if (SpravkiController.Spravki.Any(s => s.SpravkaTypeId == id))
        {
            return BadRequest("Нельзя удалить этот вид справки: по нему уже выписаны справки");
        }

        Types.Remove(type);
        return NoContent();
    }

    private string? Validate(SpravkaType type)
    {
        if (string.IsNullOrWhiteSpace(type.Name))
        {
            return "Укажите название вида справки";
        }
        if (type.Name.Length > 100)
        {
            return "Название слишком длинное: максимум 100 символов";
        }
        if (type.Price <= 0)
        {
            return "Цена должна быть больше 0";
        }
        if (type.ValidDays <= 0)
        {
            return "Срок действия должен быть хотя бы 1 день";
        }
        return null;
    }
}
