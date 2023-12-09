using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.ModelViews
{
    public class PhacDo
    {
        [Key]
        public int? DiseaseId { get; set; }
        public string DiseaseName { get; set; }
        public int? InjectionId { get; set; }

        public string InjectionName { get; set; }
        public string  MonthAgeName { get; set; }
        public int ThanhTuoi { get; set; }

        public string VaccineName { get; set; }
        public string LieuLuong { get; set; }
        public string SoLo { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? NgayHen { get; set; }
    }
}
