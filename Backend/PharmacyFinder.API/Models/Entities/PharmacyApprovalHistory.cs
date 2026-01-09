using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.API.Models.Entities
{
    [Table("PharmacyApprovalHistories")]
    public class PharmacyApprovalHistory
    {
        [Key]
        public int ApprovalId { get; set; } // REMOVE ? - PK should not be nullable

        [Required]
        public int PharmacyId { get; set; }

        public int? ApprovedByUserId { get; set; } // Keep ? - can be null when pending

        [Required]
        [StringLength(50)]
        public string ApprovalStatus { get; set; } = "Pending"; // REMOVE ? - Required means not nullable

        [StringLength(255)]
        public string? Remarks { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // REMOVE ? - Should have value

        [ForeignKey("PharmacyId")]
        public Pharmacy? Pharmacy { get; set; }

        [ForeignKey("ApprovedByUserId")]
        public User? Approver { get; set; }
    }
}