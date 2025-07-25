using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Sap
{
    public class Quotation
    {
        public string CardCode { get; set; } // Código de cliente
        public DateTime DocDate { get; set; } // Fecha documento
        public DateTime DocDueDate { get; set; } // Fecha vencimiento
        public int SalesPersonCode { get; set; }
        public string Comments { get; set; }
        public int Series { get; set; }
        public List<QuotationLine> Lines { get; set; } = new();
    }
}
