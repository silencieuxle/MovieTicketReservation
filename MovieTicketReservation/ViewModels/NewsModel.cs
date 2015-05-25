using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieTicketReservation.ViewModels {
    public class NewsModel {
        [Required(ErrorMessage = "Bạn phải nhập tựa bài viết")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập mô tả")]
        public string Description { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Bạn phải nhập nội dung bài viết")]
        public string Content { get; set; }

        public string Tags { get; set; }

        [Required(ErrorMessage = "Bạn phải chọn ảnh cover")]
        public HttpPostedFileBase Thumbnail { get; set; }
    }
}