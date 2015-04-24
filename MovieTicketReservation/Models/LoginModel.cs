using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieTicketReservation.Models {
    public class LoginModel {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage="Sai định dạng địa chỉ email.")]
        [DataType(DataType.EmailAddress, ErrorMessage="Sai định dạng địa chỉ email.")]
        [Required(ErrorMessage = "Bạn cần phải nhập email.")]
        public string Email { get; set; }

        [Display(Name="Mật khẩu")]
        [DataType(DataType.Password, ErrorMessage="Mật khẩu không đúng.")]
        [Required(ErrorMessage = "Bạn cần phải nhập mật khẩu.")]
        public string Password { get; set; }
    }
}