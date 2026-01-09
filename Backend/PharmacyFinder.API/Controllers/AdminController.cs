using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyFinder.API.Data;
using PharmacyFinder.API.Models.DTOs;
using PharmacyFinder.API.Models.Entities;
using System.Security.Claims;

namespace PharmacyFinder.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController(ApplsDbContext context, ILogger<AdminController> logger) : ControllerBase
    {
        private readonly ApplsDbContext _context = context;
        private readonly ILogger<AdminController> _logger = logger;

        // Define logger message delegates for better performance
        private static readonly Action<ILogger, Exception?> _logGetPendingPharmaciesError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(2001, "GetPendingPharmaciesError"),
                "Error getting pending pharmacies");

        private static readonly Action<ILogger, Exception?> _logApprovePharmacyError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(2002, "ApprovePharmacyError"),
                "Error approving pharmacy");

        private static readonly Action<ILogger, Exception?> _logGetApprovalHistoryError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(2003, "GetApprovalHistoryError"),
                "Error getting approval history");

        // GET: api/admin/pharmacies/pending
        [HttpGet("pharmacies/pending")]
        public async Task<ActionResult<IEnumerable<CreatePharmacyDto>>> GetPendingPharmacies()
        {
            try
            {
                List<CreatePharmacyDto> pendingPharmacies = await _context.Pharmacies
                    .Include(p => p.Owner)
                    .Where(p => p.ApprovalStatus == "Pending")
                    .OrderBy(p => p.CreatedAt)
                    .Select(p => new CreatePharmacyDto
                    {
                        PharmacyId = p.PharmacyId,
                        PharmacyName = p.PharmacyName,
                        LicenseNumber = p.LicenseNumber,
                        PhoneNumber = p.PhoneNumber,
                        Email = p.Email,
                        Address = p.Address,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        IsActive = p.IsActive,
                        // Owner = p.Owner != null ? new UserResponseDto
                        // {
                        //     UserId = p.Owner.UserId,
                        //     FullName = p.Owner.FullName,
                        //     Email = p.Owner.Email,
                        //     PhoneNumber = p.Owner.PhoneNumber
                        // } : null
                    })
                    .ToListAsync();

                return Ok(pendingPharmacies);
            }
            catch (Exception ex)
            {
                _logGetPendingPharmaciesError(_logger, ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // POST: api/admin/pharmacies/{id}/approve
        [HttpPost("pharmacies/{id}/approve")]
        public async Task<IActionResult> ApprovePharmacy(int id, [FromBody] PharmacyApprovalDto approvalDto)
        {
            // Model validation will be automatic if you have [ApiController] attribute
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int? adminUserId = GetCurrentUserId();
                if (adminUserId == null)
                {
                    return Unauthorized(new { message = "Admin not authenticated" });
                }

                Pharmacy? pharmacy = await _context.Pharmacies
                    .FirstOrDefaultAsync(p => p.PharmacyId == id);

                if (pharmacy == null)
                {
                    return NotFound(new { message = $"Pharmacy with ID {id} not found" });
                }

                // Validate status is either "Approved" or "Rejected"
                if (approvalDto.Status is not "Approved" and not "Rejected")
                {
                    return BadRequest(new { message = "Status must be either 'Approved' or 'Rejected'" });
                }

                // Update pharmacy status
                pharmacy.ApprovalStatus = approvalDto.Status;
                pharmacy.IsActive = approvalDto.Status == "Approved";

                // Create approval history
                PharmacyApprovalHistory approvalHistory = new()
                {
                    PharmacyId = pharmacy.PharmacyId,
                    ApprovedByUserId = adminUserId.Value,
                    Remarks = approvalDto.Remarks,
                    ApprovedAt = DateTime.UtcNow
                };

                _ = _context.PharmacyApprovals.Add(approvalHistory);
                _ = await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Pharmacy {approvalDto.Status.ToLower(System.Globalization.CultureInfo.CurrentCulture)} successfully",
                    pharmacyId = pharmacy.PharmacyId,
                    status = pharmacy.ApprovalStatus
                });
            }
            catch (Exception ex)
            {
                _logApprovePharmacyError(_logger, ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // POST: api/admin/pharmacies/{id}/reject
        [HttpPost("pharmacies/{id}/reject")]
        public async Task<IActionResult> RejectPharmacy(int id, [FromBody] PharmacyApprovalDto approvalDto)
        {
            // Model validation will be automatic if you have [ApiController] attribute
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Force status to "Rejected" for this endpoint
            approvalDto.Status = "Rejected";

            // Optional: Require remarks for rejection
            return string.IsNullOrWhiteSpace(approvalDto.Remarks)
                ? BadRequest(new { message = "Remarks are required when rejecting a pharmacy" })
                : await ApprovePharmacy(id, approvalDto);
        }

        // Alternative simpler reject endpoint (if you prefer):
        // [HttpPost("pharmacies/{id}/reject")]
        // public async Task<IActionResult> RejectPharmacy(int id, [FromBody] string? remarks = null)
        // {
        //     var approvalDto = new PharmacyApprovalDto
        //     {
        //         Status = "Rejected",
        //         Remarks = remarks
        //     };
        //     return await ApprovePharmacy(id, approvalDto);
        // }

        // GET: api/admin/pharmacies/approval-history/{pharmacyId}
        [HttpGet("pharmacies/approval-history/{pharmacyId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPharmacyApprovalHistory(int pharmacyId)
        {
            try
            {
                var history = await _context.PharmacyApprovals
                    .Include(pa => pa.Approver)
                    .Where(pa => pa.PharmacyId == pharmacyId)
                    .OrderByDescending(pa => pa.CreatedAt)
                    .Select(pa => new
                    {
                        pa.ApprovalId,
                        Status = pa.ApprovalStatus, // Access ApprovalStatus from the related Pharmacy entity
                        pa.Remarks,
                        pa.ApprovedAt,
                        pa.CreatedAt,
                        Approver = pa.Approver != null ? new
                        {
                            pa.Approver.UserId,
                            pa.Approver.FullName,
                            pa.Approver.Email
                        } : null
                    })
                    .ToListAsync();

                return Ok(history);
            }
            catch (Exception ex)
            {
                _logGetApprovalHistoryError(_logger, ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        private int? GetCurrentUserId()
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}