using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyFinder.API.Data;
using PharmacyFinder.API.Models.DTOs;
using PharmacyFinder.API.Models.Entities;
using System.Security.Claims;

namespace PharmacyFinder.API.Controllers
{
    [Route("api/pharmacies")]
    [ApiController]
    [Authorize]
    public class PharmaciesController(ApplsDbContext context, ILogger<PharmaciesController> logger) : ControllerBase
    {
        private readonly ApplsDbContext _context = context;
        private readonly ILogger<PharmaciesController> _logger = logger;

        // Define logger message delegates for better performance
        private static readonly Action<ILogger, Exception?> _logRegisterPharmacyError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(1001, "RegisterPharmacyError"),
                "Error registering pharmacy");

        private static readonly Action<ILogger, Exception?> _logGetPharmacyError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(1002, "GetPharmacyError"),
                "Error fetching pharmacy");

        private static readonly Action<ILogger, Exception?> _logUpdatePharmacyError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(1003, "UpdatePharmacyError"),
                "Error updating pharmacy");

        // POST: api/pharmacies/register
        [HttpPost("register")]
        [Authorize(Roles = "PharmacyOwner")]
        public async Task<ActionResult<PharmacyResponseDto>> RegisterPharmacy(CreatePharmacyDto registerDto)
        {
            try
            {
                // Get current user ID from JWT token
                int? userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                // Check if user already has a pharmacy
                Pharmacy? existingPharmacy = await _context.Pharmacies
                    .FirstOrDefaultAsync(p => p.OwnerId == userId.Value);

                if (existingPharmacy != null)
                {
                    return BadRequest(new { message = "User already has a registered pharmacy" });
                }

                // Check if license number already exists
                bool licenseExists = await _context.Pharmacies
                    .AnyAsync(p => p.LicenseNumber == registerDto.LicenseNumber);

                if (licenseExists)
                {
                    return Conflict(new { message = "License number already registered" });
                }

                // Create new pharmacy
                Pharmacy pharmacy = new()
                {
                    OwnerId = userId.Value,
                    PharmacyName = registerDto.PharmacyName,
                    LicenseNumber = registerDto.LicenseNumber,
                    PhoneNumber = registerDto.PhoneNumber,
                    Email = registerDto.Email,
                    Address = registerDto.Address,
                    Latitude = registerDto.Latitude,
                    Longitude = registerDto.Longitude,
                    IsActive = true,
                    ApprovalStatus = "Pending", // Needs admin approval
                    CreatedAt = DateTime.UtcNow
                };

                _ = _context.Pharmacies.Add(pharmacy);
                _ = await _context.SaveChangesAsync();

                // Convert to DTO for response
                PharmacyResponseDto pharmacyDto = new()
                {
                    PharmacyId = pharmacy.PharmacyId,
                    PharmacyName = pharmacy.PharmacyName,
                    LicenseNumber = pharmacy.LicenseNumber,
                    PhoneNumber = pharmacy.PhoneNumber,
                    Email = pharmacy.Email,
                    Address = pharmacy.Address,
                    Latitude = pharmacy.Latitude,
                    Longitude = pharmacy.Longitude,
                    IsActive = pharmacy.IsActive,
                    ApprovalStatus = pharmacy.ApprovalStatus,
                    CreatedAt = pharmacy.CreatedAt
                };

                return CreatedAtAction(nameof(GetMyPharmacy), new { id = pharmacy.PharmacyId }, pharmacyDto);
            }
            catch (Exception ex)
            {
                _logRegisterPharmacyError(_logger, ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // GET: api/pharmacies/my-pharmacy
        [HttpGet("my-pharmacy")]
        [Authorize(Roles = "PharmacyOwner")]
        public async Task<ActionResult<PharmacyResponseDto>> GetMyPharmacy()
        {
            try
            {
                int? userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                Pharmacy? pharmacy = await _context.Pharmacies
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.OwnerId == userId.Value);

                if (pharmacy == null)
                {
                    return NotFound(new { message = "No pharmacy registered for this user" });
                }

                PharmacyResponseDto pharmacyDto = new()
                {
                    PharmacyId = pharmacy.PharmacyId,
                    PharmacyName = pharmacy.PharmacyName,
                    LicenseNumber = pharmacy.LicenseNumber,
                    PhoneNumber = pharmacy.PhoneNumber,
                    Email = pharmacy.Email,
                    Address = pharmacy.Address,
                    Latitude = pharmacy.Latitude,
                    Longitude = pharmacy.Longitude,
                    IsActive = pharmacy.IsActive,
                    ApprovalStatus = pharmacy.ApprovalStatus,
                    CreatedAt = pharmacy.CreatedAt,
                    Owner = pharmacy.Owner != null ? new UserResponseDto
                    {
                        UserId = pharmacy.Owner.UserId,
                        FullName = pharmacy.Owner.FullName,
                        Email = pharmacy.Owner.Email,
                        PhoneNumber = pharmacy.Owner.PhoneNumber
                    } : null
                };

                return Ok(pharmacyDto);
            }
            catch (Exception ex)
            {
                _logGetPharmacyError(_logger, ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // PUT: api/pharmacies/update
        [HttpPut("update")]
        [Authorize(Roles = "PharmacyOwner")]
        public async Task<IActionResult> UpdatePharmacy(PharmacyUpdateDto updateDto)
        {
            try
            {
                int? userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                Pharmacy? pharmacy = await _context.Pharmacies
                    .FirstOrDefaultAsync(p => p.OwnerId == userId.Value);

                if (pharmacy == null)
                {
                    return NotFound(new { message = "Pharmacy not found" });
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(updateDto.PharmacyName))
                {
                    pharmacy.PharmacyName = updateDto.PharmacyName;
                }

                if (!string.IsNullOrEmpty(updateDto.PhoneNumber))
                {
                    pharmacy.PhoneNumber = updateDto.PhoneNumber;
                }

                if (!string.IsNullOrEmpty(updateDto.Email))
                {
                    pharmacy.Email = updateDto.Email;
                }

                if (!string.IsNullOrEmpty(updateDto.Address))
                {
                    pharmacy.Address = updateDto.Address;
                }

                if (updateDto.Latitude.HasValue)
                {
                    pharmacy.Latitude = updateDto.Latitude.Value;
                }

                if (updateDto.Longitude.HasValue)
                {
                    pharmacy.Longitude = updateDto.Longitude.Value;
                }

                if (updateDto.OperatingHours != null)
                {
                    //pharmacy.OperatingHours = updateDto.OperatingHours;
                }

                _ = await _context.SaveChangesAsync();

                return Ok(new { message = "Pharmacy updated successfully" });
            }
            catch (Exception ex)
            {
                _logUpdatePharmacyError(_logger, ex);
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