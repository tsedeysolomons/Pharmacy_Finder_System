using System.ComponentModel.DataAnnotations;

namespace PharmacyFinder.API.Models.DTOs
{
    /// <summary>
    /// DTO for pharmacy approval/rejection requests
    /// </summary>
    public class AdminPharmacyApprovalDto
    {
        /// <summary>
        /// Approval status: "Approved" or "Rejected"
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be either 'Approved' or 'Rejected'")]
        public string? Status { get; set; } = "Pending";

        /// <summary>
        /// Optional remarks for approval/rejection
        /// </summary>
        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters")]
        public string? Remarks { get; set; }
    }
}