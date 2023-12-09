using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.ModelViews
{
    public class MuaHangVM
    {
        [Key]
        public int KhachHangId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
        public string FullName { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Credit card number")]
        [Display(Name = "Credit card number")]
        [DataType(DataType.CreditCard)]
        public int CreditCard { get; set; }

        public int TypePayment { get; set; }
    }
}
