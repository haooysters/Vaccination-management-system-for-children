using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class Disease
    {
        public Disease()
        {
            Injections = new HashSet<Injection>();
            Vaccines = new HashSet<Vaccine>();
        }

        public int DiseaseId { get; set; }
        public string DiseaseName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Injection> Injections { get; set; }
        public virtual ICollection<Vaccine> Vaccines { get; set; }
    }
}
