using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        [Required]
        [MaxLength(100)]
        public string Status { get; set; }
        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        // Nuevos campos SAP
        public int? EmployeeCodeSap { get; set; }
        [MaxLength(50)]
        public string? AlmacenCode { get; set; }
        [MaxLength(100)]
        public string? UserSap { get; set; }
        [MaxLength(255)]
        public string? PasswordSap { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


        public ICollection<UserSerie> UserSeries { get; set; } = new List<UserSerie>();
    }
}
