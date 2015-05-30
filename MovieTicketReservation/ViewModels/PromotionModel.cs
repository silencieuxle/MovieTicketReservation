using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieTicketReservation.ViewModels {
    public class PromotionModel : NewsModel {
        public int PromotionID { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập % giảm giá")]
        public int PriceOff { get; set; }

        [Required(ErrorMessage="Bạn phải chọn rạp")]
        public List<String> Cinemas { get; set; }

        public int? FixedDayOfWeek { get; set; }

        public string PromotionTitle { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BeginDay { get; set; }

        [Required]
        public int PromotionType { get; set; }

        public int? Duration { get; set; }
    }
}