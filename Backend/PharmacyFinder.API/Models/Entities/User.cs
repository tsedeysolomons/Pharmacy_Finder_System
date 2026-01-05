using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.API.Models.Entities

{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string? PasswordHash { get; set; }// Add this inside the User class
        public virtual ICollection<PharmacyApprovalHistory>? ApprovedPharmacies { get; set; }

        [Required]
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserRole>? UserRoles { get; set; }
        public ICollection<Pharmacy>? Pharmacies { get; set; }
        public ICollection<Prescription>? Prescriptions { get; set; }
    }
    // Navigation Properties

}