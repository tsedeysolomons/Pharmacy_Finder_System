using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Drawing;

namespace PharmacyFinder.API.Models.Entities
{
    [Table("Pharmacies")]
    public class Pharmacy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PharmacyId { get; set; }
        [Required]
        public int OwnerId { get; set; }
        [Required]
        [MaxLength(200)]
        public string? PharmacyName { get; set; }
        [MaxLength(300)]
        public string? LicenseNumber { get; set; }
        [Required]
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        [Required]
        [MaxLength(100)]
        public string? Email { get; set; }
        [MaxLength(500)]
        public string? Address { get; set; }
        // 🌍 SQL Server GEOGRAPHY
        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }
        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        [Required]// Add this inside the Pharmacy class
        public virtual ICollection<PharmacyApprovalHistory>? ApprovalHistory { get; set; }
        [MaxLength(100)]
        public string ApprovalStatus { get; set; } = "Pending";
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? Owner { get; set; }
        // Navigation Properties
        public ICollection<PharmacyMedicine>? PharmacyMedicines { get; set; }
        public ICollection<PharmacyOperatingHour>? OperatingHours { get; set; }
    }
}