using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyFinder.API.Data;
using PharmacyFinder.API.Models.DTOs;
using PharmacyFinder.API.Models.Entities;
//using PharmacyFinder.API.Services.Implementations;
using PharmacyFinder.API.Services.Interfaces;

namespace PharmacyFinder.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(ApplsDbContext context, IJwtService jwtService) : ControllerBase
    {
        private readonly ApplsDbContext _context = context;
        private readonly IJwtService _jwtService = jwtService;

        // ================= REGISTER =================
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUser dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("Email already exists");
            }

            User user = new()
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _ = await _context.Users.AddAsync(user);
            _ = await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login(Login dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User? user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password");
            }

            string token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token,
                expiresIn = 3600
            });
        }
    }
}
