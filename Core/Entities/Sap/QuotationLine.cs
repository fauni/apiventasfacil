using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Sap
{
    public class QuotationLine
    {
        public string ItemCode { get; set; } // Código artículo
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
