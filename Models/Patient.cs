namespace MedicalApi.Models;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public int DistrictNumber { get; set; }   // номер участка поликлиники
    public bool HasInsurance { get; set; }
}
