

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.API.Models.Entities
{
    [Table("Prescriptions")]
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(255)]
        public string? ImagePath { get; set; }
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}