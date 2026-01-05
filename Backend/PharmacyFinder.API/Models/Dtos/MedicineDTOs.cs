using System.ComponentModel.DataAnnotations;

namespace PharmacyFinder.API.Models.DTOs
{
    public class CreateMedicineDto
    {
        [Required(ErrorMessage = "Medicine name is required")]
        [StringLength(150)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class MedicineResponseDto
    {
        public int MedicineId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
