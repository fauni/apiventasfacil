using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class SalesPersonDto
    {
        public int SlpCode { get; set; }
        public string SlpName { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public string Active { get; set; } = string.Empty;

        // Propiedades de conveniencia
        public bool IsActive => Active == "Y";
        public string DisplayName => $"{SlpCode} - {SlpName}";
    }
}
