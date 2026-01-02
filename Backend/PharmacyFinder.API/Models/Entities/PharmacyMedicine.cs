

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.API.Models.Entities
{
    [Table("PharmacyMedicines")]
    public class PharmacyMedicine
    {
        [Required]
        public int PharmacyId { get; set; }
        [Required]
        public int MedicineId { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        [ForeignKey("PharmacyId")]
        public Pharmacy Pharmacy { get; set; }
        [ForeignKey("MedicineId")]
        public Medicine Medicine { get; set; }

    }


}