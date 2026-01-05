using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyFinder.API.Data;
using PharmacyFinder.API.Models.DTOs;
using PharmacyFinder.API.Models.Entities;

namespace PharmacyFinder.API.Controllers
{
    [ApiController]
    [Route("api/medicines")]
    public class MedicinesController(ApplsDbContext context) : ControllerBase
    {
        private readonly ApplsDbContext _context = context;

        // ================= CREATE MEDICINE =================
        [HttpPost]
        [Authorize(Roles = "Admin,PharmacyOwner")]
        public async Task<IActionResult> Create(CreateMedicineDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Medicine medicine = new()
            {
                MedicineName = dto.Name,
                Description = dto.Description
            };

            _ = _context.Medicines.Add(medicine);
            _ = await _context.SaveChangesAsync();

            return Ok(medicine);
        }

        // ================= GET ALL MEDICINES =================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<MedicineResponseDto> medicines = await _context.Medicines
                .Select(m => new MedicineResponseDto
                {
                    MedicineId = m.MedicineId,
                    Name = m.MedicineName,
                    Description = m.Description
                })
                .ToListAsync();

            return Ok(medicines);
        }
    }
}
