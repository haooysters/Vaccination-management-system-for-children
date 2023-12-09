using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class Vaccine
    {
        public Vaccine()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Regimen = new HashSet<Regimen>();
        }

        public int VaccineId { get; set; }
        public string VaccineName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? AgeId { get; set; }
        public int? BrandId { get; set; }
        public int? DiseaseId { get; set; }
        public bool? Active { get; set; }
        public int? Price { get; set; }
        public string Thumb { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Age Age { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual Disease Disease { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Regimen> Regimen { get; set; }
    }
}
