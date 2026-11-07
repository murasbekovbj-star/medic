namespace MedicalApi.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string DoctorName { get; set; } = "";
    public string Specialty { get; set; } = "";   // терапевт, лор, ...
    public DateTime Date { get; set; }
    public string Complaint { get; set; } = "";   // с чем пришёл
    public bool IsDone { get; set; }              // приём уже состоялся
}
