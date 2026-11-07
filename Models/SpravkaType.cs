namespace MedicalApi.Models;

public class SpravkaType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int ValidDays { get; set; }            // сколько дней действует
    public bool NeedsDoctorExam { get; set; }     // нужен ли осмотр врача
    public bool ForChildren { get; set; }         // для детей или для взрослых
}
