using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Vaccines = new HashSet<Vaccine>();
        }

        public int BrandId { get; set; }
        public string BrandName { get; set; }

        public virtual ICollection<Vaccine> Vaccines { get; set; }
    }
}
