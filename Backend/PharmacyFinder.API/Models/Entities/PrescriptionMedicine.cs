
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.API.Models.Entities
{
    [Table("PrescriptionMedicines")]
    public class PrescriptionMedicine
    {
        [Key]
        public int PrescriptionMedicineId { get; set; }
        [Required]
        public int PrescriptionId { get; set; }
        [StringLength(100)]
        public string? MedicineName { get; set; }
        [Required]
        public int Quantity { get; set; }
        [ForeignKey("PrescriptionId")]
        public Prescription? Prescription { get; set; }

    }
}