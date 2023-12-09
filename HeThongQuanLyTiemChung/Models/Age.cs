using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class Age
    {
        public Age()
        {
            Vaccines = new HashSet<Vaccine>();
        }

        public int AgeId { get; set; }
        public string AgeName { get; set; }

        public virtual ICollection<Vaccine> Vaccines { get; set; }
    }
}
