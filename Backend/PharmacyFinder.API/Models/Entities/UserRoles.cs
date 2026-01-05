

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyFinder.API.Models.Entities
{
    [Table("UserRoles")]
    public class UserRole
    {
        [Key] // Add this
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Add this
        public int UserRoleId { get; set; } // Add this primary key
        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Navigation Properties
        /* */
        public User? User { get; set; }
        public Role? Role { get; set; }
    }
}
