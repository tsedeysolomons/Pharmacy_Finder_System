

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.API.Models.Entities
{
    [Table("PharmacyApprovalHistorys")]
    public class PharmacyApprovalHistory
    {
        [Key]
        public int ApprovalId { get; set; }
        [Required]
        public int PharmacyId { get; set; }
        [Required]
        public int ApprovedByUserId { get; set; }
        [StringLength(250)]
        public string Remarks { get; set; }
        public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("PharmacyId")]
        public Pharmacy Pharmacy { get; set; }
        [ForeignKey("ApprovedByUserId")]
        public User Approver { get; set; }

    }
}