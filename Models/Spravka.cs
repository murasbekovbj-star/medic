namespace MedicalApi.Models;

public class Spravka
{
    public int Id { get; set; }
    public string Name { get; set; } = "";      // берётся из каталога
    public int PatientId { get; set; }
    public int SpravkaTypeId { get; set; }
    public decimal Price { get; set; }          // берётся из каталога
    public DateTime IssuedAt { get; set; }      // когда выписана
}
