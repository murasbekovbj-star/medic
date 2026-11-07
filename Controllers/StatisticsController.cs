using Microsoft.AspNetCore.Mvc;

// Статистика для главврача
[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var spravki = SpravkiController.Spravki;

        return Ok(new
        {
            patientsCount = PatientsController.Patients.Count,
            spravkiCount = spravki.Count,
            spravkiTotalSum = spravki.Sum(s => s.Price),
            appointmentsCount = AppointmentsController.Appointments.Count,
            appointmentsDone = AppointmentsController.Appointments.Count(a => a.IsDone)
        });
    }
}
