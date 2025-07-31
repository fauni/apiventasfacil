using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.UnitOfMeasure
{
    public class UnitOfMeasureDto
    {
        public int UomEntry { get; set; }
        public string UomCode { get; set; }
        public string UomName { get; set; }
        public decimal BaseQty { get; set; }
        public decimal AltQty { get; set; }
        public bool IsDefault { get; set; }
        public string ItemCode { get; set; }
    }


}
