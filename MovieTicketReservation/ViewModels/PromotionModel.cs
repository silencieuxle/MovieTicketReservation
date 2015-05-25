using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieTicketReservation.ViewModels {
    public class PromotionModel : NewsModel {
        [Required(ErrorMessage = "Bạn phải nhập % giảm giá")]
        public int PriceOff { get; set; }
    }
}