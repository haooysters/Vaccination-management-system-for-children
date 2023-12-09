using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.ModelViews
{
    public class MuiBenh
    {
        public int DiseaseId { get; set; }
        public string DiseaseName { get; set; }
        public int InjectionId { get; set; }
        
        public string InjectionName { get; set; }
        public int? MonthAgeName { get; set; }
    }
}
