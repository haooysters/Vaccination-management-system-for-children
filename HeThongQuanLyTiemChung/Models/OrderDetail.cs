using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? VaccineId { get; set; }
        public int? InjectionId { get; set; }
        public string LotNumber { get; set; }
        public string Dosage { get; set; }
        public int? Quantity { get; set; }
        public int? Total { get; set; }

        public virtual Injection Injection { get; set; }
        public virtual Order Order { get; set; }
        public virtual Vaccine Vaccine { get; set; }
    }
}
