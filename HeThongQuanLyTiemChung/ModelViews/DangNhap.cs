﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongQuanLyTiemChung.ModelViews
{
    public class DangNhap
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage = ("Vui lòng nhập số điện thoại"))]
        [Display(Name = "ố điện thoại")]
      
        public string Phone { get; set; }



        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5, ErrorMessage = "Bạn cần nhập mật khẩu tối thiểu 5 ký tự")]
        public string Password { get; set; }
    }
}