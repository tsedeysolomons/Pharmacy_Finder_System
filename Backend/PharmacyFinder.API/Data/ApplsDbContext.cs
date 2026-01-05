using Microsoft.EntityFrameworkCore;
using PharmacyFinder.API.Models.Entities;

namespace PharmacyFinder.API.Data
{
    public class ApplsDbContext : DbContext
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

        public ApplsDbContext(DbContextOptions<ApplsDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========= User =========
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt)
                      .ValueGeneratedOnAdd();
            });

            // ========= Role =========
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.RoleName).IsUnique();

                entity.HasData(
                    new Role { RoleId = 1, RoleName = "Admin" },
                    new Role { RoleId = 2, RoleName = "PharmacyOwner" },
                    new Role { RoleId = 3, RoleName = "Customer" }
                );
            });

            // ========= UserRole =========
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId);
            });

            // ========= Pharmacy =========
            modelBuilder.Entity<Pharmacy>(entity =>
            {
                entity.HasKey(e => e.PharmacyId);
                entity.Property(e => e.PharmacyName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.LicenseNumber).IsUnique();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Latitude).HasColumnType("decimal(9,6)");
                entity.Property(e => e.Longitude).HasColumnType("decimal(9,6)");
                entity.Property(e => e.ApprovalStatus)
                      .IsRequired()
                      .HasDefaultValue("Pending");
                entity.Property(e => e.CreatedAt)
                      .ValueGeneratedOnAdd();

                entity.HasOne(p => p.Owner)
                      .WithMany(u => u.Pharmacies)
                      .HasForeignKey(p => p.OwnerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========= PharmacyOperatingHour =========
            modelBuilder.Entity<PharmacyOperatingHour>(entity =>
            {
                // Composite Primary Key
                entity.HasKey(e => new { e.PharmacyId, e.DayOfWeek });

                // Ensure one record per day per pharmacy
                entity.HasIndex(e => new { e.PharmacyId, e.DayOfWeek })
                      .IsUnique();

                entity.Property(e => e.DayOfWeek)
                      .IsRequired();

                entity.Property(e => e.OpenTime)
                      .IsRequired();

                entity.Property(e => e.CloseTime)
                      .IsRequired();

                // Relationship
                entity.HasOne(oh => oh.Pharmacy)
                      .WithMany(p => p.OperatingHours)
                      .HasForeignKey(oh => oh.PharmacyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ========= PharmacyApprovalHistory =========
            modelBuilder.Entity<PharmacyApprovalHistory>(entity =>
            {
                entity.HasKey(e => e.ApprovalId);
                entity.Property(e => e.Remarks).HasMaxLength(255);

                entity.HasOne(pa => pa.Pharmacy)
                      .WithMany(p => p.ApprovalHistory)
                      .HasForeignKey(pa => pa.PharmacyId);

                entity.HasOne(pa => pa.Approver)
                      .WithMany(u => u.ApprovedPharmacies)
                      .HasForeignKey(pa => pa.ApprovedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========= Medicine =========
            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.HasKey(e => e.MedicineId);
                entity.Property(e => e.MedicineName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Manufacturer).HasMaxLength(150);
                entity.Property(e => e.IsPrescriptionRequired).HasDefaultValue(false);

                entity.HasData(
                    new Medicine { MedicineId = 1, MedicineName = "Paracetamol", Manufacturer = "Generic", IsPrescriptionRequired = false },
                    new Medicine { MedicineId = 2, MedicineName = "Amoxicillin", Manufacturer = "HealthCorp", IsPrescriptionRequired = true },
                    new Medicine { MedicineId = 3, MedicineName = "Ibuprofen", Manufacturer = "MediHealth", IsPrescriptionRequired = false }
                );
            });

            // ========= PharmacyMedicine =========
            modelBuilder.Entity<PharmacyMedicine>(entity =>
            {
                entity.HasKey(e => e.PharmacyMedicineId);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Quantity).HasDefaultValue(0);
                entity.Property(e => e.LastUpdated)
                      .ValueGeneratedOnAdd();
            });

            // ========= Prescription =========
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(e => e.PrescriptionId);
                entity.Property(e => e.ImagePath).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Status).HasDefaultValue("Pending");
                entity.Property(e => e.UploadedAt)
                      .ValueGeneratedOnAdd();
            });

            // ========= PrescriptionMedicine =========
            modelBuilder.Entity<PrescriptionMedicine>(entity =>
            {
                entity.HasKey(e => e.PrescriptionMedicineId);
                entity.Property(e => e.MedicineName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Quantity).HasDefaultValue(1);
            });

            // ========= Seed Admin =========
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "System Administrator",
                    Email = "admin@pharmacyfinder.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEHeQH5j8F+qwo0hHywIkMHqq7gBLq45c/2E7FJX8C0nNJgTqj6nE8nXhU6JxNcZ7UA==",
                    PhoneNumber = "+1234567890",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 }
            );
        }
    }
}
