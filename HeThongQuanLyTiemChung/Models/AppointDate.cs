using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class AppointDate
    {
        public int AppointDateId { get; set; }
        public int? InjectionId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? AppointmentDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Injection Injection { get; set; }
    }
}
