using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    // тестовые данные, чтобы после запуска сразу было что смотреть
    public static List<Patient> Patients = new List<Patient>
    {
        new Patient { Id = 1, Name = "Алия Сатыбалдиева", BirthDate = new DateTime(1998, 3, 12), Phone = "+996700111222", Address = "ул. Абая 10, кв. 5", DistrictNumber = 3, HasInsurance = true },
        new Patient { Id = 2, Name = "Бакыт Жумабеков", BirthDate = new DateTime(1985, 11, 2), Phone = "+996555333444", Address = "ул. Токтогула 45", DistrictNumber = 1, HasInsurance = false },
        new Patient { Id = 3, Name = "Эмир Касымов", BirthDate = new DateTime(2015, 7, 21), Phone = "+996777555666", Address = "мкр. Асанбай 8, кв. 12", DistrictNumber = 2, HasInsurance = true },
    };

    // следующий свободный Id (не Count + 1, иначе после удаления Id повторяются)
    private static int _nextId = 4;

    [HttpGet]
    public List<Patient> GetAll()
    {
        return Patients;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Patient> GetById(int id)
    {
        Patient? patient = Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound();
        }
        return patient;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Patient> Create(Patient patient)
    {
        string? error = Validate(patient);
        if (error != null)
        {
            return BadRequest(error);
        }

        patient.Id = _nextId;
        _nextId++;
        Patients.Add(patient);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Patient> Update([FromRoute] int id, [FromBody] Patient patient)
    {
        Patient? existing = Patients.FirstOrDefault(p => p.Id == id);
        if (existing == null)
    {
        return NotFound();
    }

    string? error = Validate(patient);
    if (error != null)
    {
        return BadRequest(error);
    }

    existing.Name = patient.Name;
    existing.BirthDate = patient.BirthDate;
    existing.Phone = patient.Phone;
    existing.Address = patient.Address;
    existing.DistrictNumber = patient.DistrictNumber;
    existing.HasInsurance = patient.HasInsurance;
    
    return Ok(existing);
    }

    // При удалении пациента удаляем и его справки и приёмы,
    // иначе они останутся "висеть" без владельца.
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Delete(int id)
    {
        Patient? patient = Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound();
        }

        SpravkiController.Spravki.RemoveAll(s => s.PatientId == id);
        AppointmentsController.Appointments.RemoveAll(a => a.PatientId == id);
        Patients.Remove(patient);
        return NoContent();
    }

    [HttpGet("{id}/spravki")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<Spravka>> GetSpravki(int id)
    {
        Patient? patient = Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound();
        }

        return SpravkiController.Spravki.Where(s => s.PatientId == id).ToList();
    }

    [HttpGet("{id}/appointments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<Appointment>> GetAppointments(int id)
    {
        Patient? patient = Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound();
        }

        return AppointmentsController.Appointments.Where(a => a.PatientId == id).ToList();
    }

    // null = данные верные, иначе текст ошибки (его покажет фронтенд)
    private string? Validate(Patient patient)
    {
        if (string.IsNullOrWhiteSpace(patient.Name))
        {
            return "Укажите ФИО пациента";
        }
        if (patient.Name.Length > 100)
        {
            return "ФИО слишком длинное: максимум 100 символов";
        }
        if (patient.BirthDate > DateTime.Now)
        {
            return "Дата рождения не может быть в будущем. Проверьте год";
        }
        if (patient.BirthDate.Year < 1900)
        {
            return "Дата рождения слишком давняя. Проверьте год";
        }
        if (string.IsNullOrWhiteSpace(patient.Phone) || patient.Phone.Length < 7)
        {
            return "Укажите телефон, минимум 7 символов";
        }
        if (patient.DistrictNumber <= 0)
        {
            return "Номер участка должен быть больше 0";
        }
        return null;
    }
}
