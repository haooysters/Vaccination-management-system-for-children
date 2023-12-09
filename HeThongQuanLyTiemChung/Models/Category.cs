using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class Category
    {
        public Category()
        {
            Vaccines = new HashSet<Vaccine>();
        }

        public int CatId { get; set; }
        public string CatName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Vaccine> Vaccines { get; set; }
    }
}
