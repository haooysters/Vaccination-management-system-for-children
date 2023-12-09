using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class Injection
    {
        public Injection()
        {
            AppointDates = new HashSet<AppointDate>();
            OrderDetails = new HashSet<OrderDetail>();
            Regimen = new HashSet<Regimen>();
        }

        public int InjectionId { get; set; }
        public int? DiseaseId { get; set; }
        public string InjectionName { get; set; }
        public int MonthAgeName { get; set; }

        public virtual Disease Disease { get; set; }
        public virtual ICollection<AppointDate> AppointDates { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Regimen> Regimen { get; set; }
    }
}
