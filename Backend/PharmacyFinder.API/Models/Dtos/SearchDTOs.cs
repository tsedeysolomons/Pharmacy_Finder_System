namespace PharmacyFinder.API.Models.DTOs
{
    public class PharmacySearchDto
    {
        public string? PharmacyName { get; set; }
        public string? City { get; set; }
        public bool? IsOpen { get; set; }
    }

    public class MedicineSearchDto
    {
        public string? MedicineName { get; set; }
        public int? PharmacyId { get; set; }
    }
}
