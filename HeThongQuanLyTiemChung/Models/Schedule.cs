using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        public int? RegimenId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? NextDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Regimen Regimen { get; set; }
    }
}
