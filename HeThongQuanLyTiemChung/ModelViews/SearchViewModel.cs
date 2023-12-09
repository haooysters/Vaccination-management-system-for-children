using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeThongQuanLyTiemChung.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeThongQuanLyTiemChung.ModelViews
{
    public class SearchViewModel
    {
        public List<Vaccine>? Vaccines { get; set; }
        public SelectList? Cats { get; set; }
        public int? VaccineCat { get; set; }
        public string? SearchString { get; set; }
    }
}
