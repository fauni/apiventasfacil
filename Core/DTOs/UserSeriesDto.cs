using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class UserSeriesDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string IdSerie { get; set; }
        public int Series { get; set; }
        public string SeriesName { get; set; }

        // Propiedades calculadas
        public string DisplayName => !string.IsNullOrEmpty(SeriesName) ? $"{IdSerie} - {SeriesName}" : IdSerie;
    }
}
