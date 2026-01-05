using System.ComponentModel.DataAnnotations;

namespace PharmacyFinder.API.Models.DTOs
{
    public class CreatePharmacyDto
    {
        [Required(ErrorMessage = "Pharmacy name is required")]
        [StringLength(150)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(300)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "License number is required")]
        [StringLength(50)]
        public string? LicenseNumber { get; set; }
    }

    public class PharmacyResponseDto
    {
        public int PharmacyId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }
}
