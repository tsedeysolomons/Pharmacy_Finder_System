using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyFinder.API.Data;
using PharmacyFinder.API.Models.DTOs;
using PharmacyFinder.API.Models.Entities;

namespace PharmacyFinder.API.Controllers
{
    [ApiController]
    [Route("api/pharmacies")]
    public class PharmaciesController(ApplsDbContext context) : ControllerBase
    {
        private readonly ApplsDbContext _context = context;

        // ================= CREATE PHARMACY =================
        [HttpPost]
        [Authorize(Roles = "Admin,PharmacyOwner")]
        public async Task<IActionResult> Create(CreatePharmacyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Pharmacy pharmacy = new()
            {
                PharmacyName = dto.Name,
                Address = dto.Address,
                LicenseNumber = dto.LicenseNumber,
                IsActive = true
            };

            _ = _context.Pharmacies.Add(pharmacy);
            _ = await _context.SaveChangesAsync();

            return Ok(pharmacy);
        }

        // ================= GET ALL PHARMACIES =================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<PharmacyResponseDto> pharmacies = await _context.Pharmacies
                .Select(p => new PharmacyResponseDto
                {
                    PharmacyId = p.PharmacyId,
                    Name = p.PharmacyName,
                    Address = p.Address,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return Ok(pharmacies);
        }
    }
}
