
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.API.Models.Entities
{
    [Table("Medicines")]
    public class Medicine
    {
        [Key]
        public int MedicineId { get; set; }
        [Required]
        [StringLength(100)]
        public string MedicineName { get; set; }
        [StringLength(500)]
        public string Manufacturer { get; set; }
        [Required]
        public bool IsPrescriptionRequired { get; set; }
    }
}