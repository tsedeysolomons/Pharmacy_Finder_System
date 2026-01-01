using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.API.Models.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        [Column(TypeName = "nvarchar(100)")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        [Column(TypeName = "nvarchar(150)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password hash is required")]
        [Column(TypeName = "nvarchar(max)")]
        public string PasswordHash { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Column(TypeName = "nvarchar(20)")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties for relationships
        // public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        // public virtual ICollection<Pharmacy> Pharmacies { get; set; } = new List<Pharmacy>();
        // public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        // public virtual ICollection<PharmacyApproval> ApprovedPharmacies { get; set; } = new List<PharmacyApproval>();
    }
}