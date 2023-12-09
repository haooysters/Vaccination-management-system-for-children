using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class Regimen
    {
        public int RegimenId { get; set; }
        public int VaccineId { get; set; }
        public int? InjectionId { get; set; }
        public int? MonthAgeId { get; set; }

        public virtual Injection Injection { get; set; }
        public virtual MonthAge MonthAge { get; set; }
        public virtual Vaccine Vaccine { get; set; }
    }
}
