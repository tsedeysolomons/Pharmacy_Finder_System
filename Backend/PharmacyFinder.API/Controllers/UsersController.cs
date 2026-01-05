using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyFinder.API.Data;
using PharmacyFinder.API.Models.DTOs;
using PharmacyFinder.API.Models.Entities;

namespace PharmacyFinder.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController(ApplsDbContext context) : ControllerBase
    {
        private readonly ApplsDbContext _context = context;

        // ================= GET ALL USERS (ADMIN) =================
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            List<UserResponseDto> users = await _context.Users
                .Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    Roles = _context.UserRoles
                            .Where(ur => ur.UserId == u.UserId)
                        .Select(r => r.Role.RoleName)
                        .ToList()
                })
                .ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

            return Ok(users);
        }

        // ================= UPDATE USER =================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (dto.FullName != null)
            {
                user.FullName = dto.FullName;
            }

            if (dto.PhoneNumber != null)
            {
                user.PhoneNumber = dto.PhoneNumber;
            }

            if (dto.IsActive.HasValue)
            {
                user.IsActive = dto.IsActive.Value;
            }

            _ = await _context.SaveChangesAsync();
            return Ok("User updated successfully");
        }

        // ================= CHANGE PASSWORD =================
        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordDto dto)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                return BadRequest("Current password is incorrect");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _ = await _context.SaveChangesAsync();

            return Ok("Password updated successfully");
        }
    }
}
