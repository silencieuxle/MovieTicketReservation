using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieTicketReservation.ViewModels {
    public class MemberModel {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Định dạng email không đúng.")]
        [Required]
        public string Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password, ErrorMessage = "Sai mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Tên")]
        [Required]
        public string Firstname { get; set; }

        [Display(Name = "Họ")]
        [Required]
        public string Lastname { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required]
        public string Address { get; set; }

        [Display(Name = "Số CMND")]
        [Required]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Số chứng minh nhân dân phải có 9 chữ số")]
        public string IdCardNumber { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required]
        [RegularExpression("^[0]d{9,10}$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string Phone { get; set; }

        [Display(Name = "Giới tính")]
        [Required]
        public bool? Gender { get; set; }

        [Display(Name = "Ngày sinh")]
        [Required]
        [DataType(DataType.DateTime)]
        [Range(typeof(DateTime), "1/1/1920", "1/1/2005")]
        public DateTime? Birthday { get; set; }

        public string AvatarUrl { get; set; }
    }
}