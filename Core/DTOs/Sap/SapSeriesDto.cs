using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Sap
{
    public class SapSeriesDto
    {
        public int Series { get; set; }
        public string SeriesName { get; set; } = string.Empty;
        public int ObjectCode { get; set; }
        public string Indicator { get; set; } = string.Empty;
        public int NextNumber { get; set; }
        public int LastNum { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
        public bool Locked { get; set; }
        public int PeriodoValidFrom { get; set; }
        public int PeriodoValidTo { get; set; }
        
        public string DisplayName => $"{Series} - {SeriesName}";
        public string DisplayDetails => $"{Prefix}{NextNumber.ToString().PadLeft(6, '0')}{Suffix}";
    }
}
