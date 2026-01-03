using Microsoft.EntityFrameworkCore;
using PharmacyFinder.API.Models.Entities;

namespace PharmacyFinder.API.Data
{
    public class ApplsDbContext(DbContextOptions<ApplsDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<PharmacyOperatingHour> PharmacyOperatingHours { get; set; }
        public DbSet<PharmacyApprovalHistory> PharmacyApprovals { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<PharmacyMedicine> PharmacyMedicines { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; }

        // confithe table details 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // base.OnModelCreating(modelBuilder);
            // //========User Configration========//
            // modelBuilder.Entity<User>()

        }

    }
}