using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;

// Свой модуль: запись на приём к врачу
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    public static List<Appointment> Appointments = new List<Appointment>
    {
        new Appointment { Id = 1, PatientId = 1, DoctorName = "Иванова Т.П.", Specialty = "Терапевт", Date = DateTime.Now.AddDays(1), Complaint = "Кашель, температура", IsDone = false },
        new Appointment { Id = 2, PatientId = 2, DoctorName = "Омаров К.С.", Specialty = "Лор", Date = DateTime.Now.AddDays(2), Complaint = "Болит ухо", IsDone = false },
    };

    private static int _nextId = 3;

    [HttpGet]
    public List<Appointment> GetAll()
    {
        return Appointments;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Appointment> GetById(int id)
    {
        Appointment? appointment = Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
        {
            return NotFound();
        }
        return appointment;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Appointment> Create(Appointment appointment)
    {
        string? error = Validate(appointment);
        if (error != null)
        {
            return BadRequest(error);
        }

        appointment.Id = _nextId;
        _nextId++;
        Appointments.Add(appointment);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Appointment> Update(int id, Appointment appointment)
    {
        Appointment? existing = Appointments.FirstOrDefault(a => a.Id == id);
        if (existing == null)
        {
            return NotFound();
        }

        string? error = Validate(appointment);
        if (error != null)
        {
            return BadRequest(error);
        }

        existing.PatientId = appointment.PatientId;
        existing.DoctorName = appointment.DoctorName;
        existing.Specialty = appointment.Specialty;
        existing.Date = appointment.Date;
        existing.Complaint = appointment.Complaint;
        existing.IsDone = appointment.IsDone;
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Delete(int id)
    {
        Appointment? appointment = Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
        {
            return NotFound();
        }

        Appointments.Remove(appointment);
        return NoContent();
    }

    private string? Validate(Appointment appointment)
    {
        if (!PatientsController.Patients.Any(p => p.Id == appointment.PatientId))
        {
            return "Пациент с таким номером не найден. Сначала создайте пациента в картотеке";
        }
        if (string.IsNullOrWhiteSpace(appointment.DoctorName))
        {
            return "Укажите ФИО врача";
        }
        if (string.IsNullOrWhiteSpace(appointment.Specialty))
        {
            return "Укажите специальность врача, например: Терапевт";
        }
        if (string.IsNullOrWhiteSpace(appointment.Complaint))
        {
            return "Укажите причину обращения";
        }
        if (appointment.Date < DateTime.Now && !appointment.IsDone)
        {
            return "Нельзя записать на прошедшее время. Выберите дату в будущем";
        }
        if (appointment.Date.Hour < 8 || appointment.Date.Hour >= 18)
        {
            return "Поликлиника принимает с 8:00 до 18:00. Выберите другое время";
        }
        // один врач не может принять двоих в одно время
        bool busy = Appointments.Any(a => a.Id != appointment.Id
            && a.DoctorName == appointment.DoctorName
            && a.Date == appointment.Date);
        if (busy)
        {
            return "У этого врача на это время уже есть запись. Выберите другое время";
        }
        return null;
    }
}
