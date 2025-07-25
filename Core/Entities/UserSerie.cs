using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Entities
{
    [Table("UserSeries")]
    public class UserSerie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int IdUsuario { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [Required]
        [MaxLength(100)]
        public string IdSerie { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
