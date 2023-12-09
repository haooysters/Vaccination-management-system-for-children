using System;
using System.Collections.Generic;

#nullable disable

namespace HeThongQuanLyTiemChung.Models
{
    public partial class Customer
    {
        public Customer()
        {
            AppointDates = new HashSet<AppointDate>();
            Orders = new HashSet<Order>();
        }

        public int CustomerId { get; set; }
        public int? GenderId { get; set; }
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string Phone { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual Gender Gender { get; set; }
        public virtual ICollection<AppointDate> AppointDates { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
