using System.ComponentModel.DataAnnotations;

namespace PharmacyFinder.API.Models.DTOs
{
    // Pharmacy Registration Request DTO
    public class CreatePharmacyDto
    {
        [Required(ErrorMessage = "Pharmacy name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Pharmacy name must be between 2 and 200 characters")]
        public string? PharmacyName { get; set; }

        [Required(ErrorMessage = "License number is required")]
        [StringLength(100, ErrorMessage = "License number cannot exceed 100 characters")]
        public string? LicenseNumber { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 500 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; }

        [StringLength(500, ErrorMessage = "Operating hours cannot exceed 500 characters")]
        public string? OperatingHours { get; set; }
        public int PharmacyId { get; internal set; }
        public bool IsActive { get; internal set; }
    }

    public class PharmacyResponseDto
    {
        public int PharmacyId { get; set; }
        public string? PharmacyName { get; set; }
        public string? LicenseNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? OperatingHours { get; set; }
        public bool IsActive { get; set; }
        public string? ApprovalStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserResponseDto? Owner { get; set; }
    }
    // Pharmacy Update Request DTO (Placeholder for future implementation)
    public class PharmacyUpdateDto
    {
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Pharmacy name must be between 2 and 200 characters")]
        public string? PharmacyName { get; set; }
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? PhoneNumber { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        public string? Email { get; set; }
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 500 characters")]
        public string? Address { get; set; }
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal? Latitude { get; set; }
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal? Longitude { get; set; }
        [StringLength(500, ErrorMessage = "Operating hours cannot exceed 500 characters")]
        public string? OperatingHours { get; set; }
        public bool? IsActive { get; set; }

    }
    // Pharmacy Approval Request (Admin)
    public class PharmacyApprovalDto
    {
        [Required(ErrorMessage = "Approval status is required")]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be 'Approved' or 'Rejected'")]
        public string? Status { get; set; }

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters")]
        public string? Remarks { get; set; }
    }
}
