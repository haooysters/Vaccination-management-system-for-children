using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class MonthAge
    {
        public MonthAge()
        {
            Regimen = new HashSet<Regimen>();
        }

        public int MonthAgeId { get; set; }
        public int? MonthAgeName { get; set; }

        public virtual ICollection<Regimen> Regimen { get; set; }
    }
}
