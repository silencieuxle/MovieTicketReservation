using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieTicketReservation.ViewModels {
    public class UserRegisterModel {
        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Bạn phải nhập tên")]
        public string FirstName { get; set; }

        [Display(Name = "Họ")]
        [Required(ErrorMessage = "Bạn phải nhập họ")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Required(ErrorMessage = "Bạn phải nhập địa chỉ email")]
        public string Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        [MinLength(9, ErrorMessage = "Mật khẩu phải dài hơn hoặc bằng 9 kí tự")]
        [Required(ErrorMessage = "Bạn phải nhập mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Nhập lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu 2 không trùng với mật khẩu 1")]
        [Required(ErrorMessage = "Bạn phải nhập mật khẩu lần 2")]
        public string PasswordRetyped { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date, ErrorMessage = "Bạn chỉ được chọn ngày")]
        [Required(ErrorMessage = "Bạn phải chọn ngày sinh")]
        public DateTime BirthDay { get; set; }

        [Display(Name = "Giới tính")]
        [Required(ErrorMessage = "Bạn phải chọn giới tính")]
        public bool Gender { get; set; }

        [Display(Name = "Số điện thoại")]
        [RegularExpression("^[0]\\d{9,10}$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        [Required(ErrorMessage = "Bạn phải nhập số điện thoại.")]
        public string Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Bạn phải nhập địa chỉ.")]
        public string Address { get; set; }

        [Display(Name = "Chứng minh nhân dân")]
        [RegularExpression("\\d{9}", ErrorMessage = "Số CMND không đúng định dạng")]
        [Required(ErrorMessage = "Bạn phải nhập số CMND.")]
        public string IdCardNumber { get; set; }
    }
}