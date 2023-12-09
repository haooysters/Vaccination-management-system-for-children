using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.ModelViews
{
    public class CartItem
    {
        [Key]
        public int VaccineID { get; set; }

        public string VaccineName { get; set; }
        public string Thumb { get; set; }
        public int? Price { get; set; }
        public int SoLuong { get; set; }

        public int DiseaseID { get; set; }

        public string DiseaseName { get; set; }

        public int InjectionID { get; set; }
        public string InjectionName { get; set; }
        public double? ThanhTien => SoLuong * Price;


        public string LieuLuong { get; set; }
        public string SoLo { get; set; }
    }
}
