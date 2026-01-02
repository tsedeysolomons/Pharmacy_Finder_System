

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.API.Models.Entities
{
    [Table("PharmacyOperatingHours")]
    public class PharmacyOperatingHour
    {
        [Required]
        public int PharmacyId { get; set; }
        [Required]
        public int DayOfWeek { get; set; } // 0 = Sunday, 6 = Saturday
        [Required]
        public TimeSpan OpenTime { get; set; }
        [Required]
        public TimeSpan CloseTime { get; set; }
        [ForeignKey("PharmacyId")]
        public Pharmacy Pharmacy { get; set; }



    }

}