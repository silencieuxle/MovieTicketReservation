using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.EditionService;
using MovieTicketReservation.Services.GenreService;

namespace MovieTicketReservation.ViewModels {
    public class MovieAddModel {
        private DbEntities context = new DbEntities();

        [Required(ErrorMessage = "Bạn phải nhập tên film")]
        [Display(Name = "Tên film")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập tên đạo diễn")]
        [Display(Name = "Đạo diễn")]
        public string Director { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập diễn viên")]
        [Display(Name = "Diễn viên")]
        public string Actors { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập mô tả ngắn")]
        [Display(Name = "Mô tả ngắn")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập mô tả đầy đủ")]
        [Display(Name = "Mô tả đầy đủ")]
        public string LongDescription { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập độ dài film")]
        [Display(Name = "Độ dài film")]
        [Range(0, 500, ErrorMessage="Độ dài film phải từ 0 đến 500 phút")]
        [RegularExpression("^\\d*$", ErrorMessage = "Độ dài film phải là số")]
        public int MovieLength { get; set; }

        [Display(Name = "Trailer film")]
        [Required(ErrorMessage = "Bạn phải nhập đường dẫn trailer")]
        public string TrailerURL { get; set; }

        [Display(Name = "Ngày ra mắt")]
        [DataType(DataType.Date, ErrorMessage = "Bạn chỉ được chọn ngày")]
        [Required(ErrorMessage = "Bạn phải chọn ngày ra mắt")]
        public DateTime ReleasedDate { get; set; }

        [Display(Name = "Ngày khởi chiếu")]
        [DataType(DataType.Date, ErrorMessage = "Bạn chỉ được chọn ngày")]
        public DateTime BeginShowDate { get; set; }

        [Display(Name = "Thời gian chiếu (ngày)")]
        [Required(ErrorMessage = "Bạn phải nhập thời gian chiếu")]
        [RegularExpression("^\\d*$", ErrorMessage = "Độ dài film phải là số")]
        [Range(0, 45, ErrorMessage="Thời gian chiếu từ 0 - 10")]
        public int Duration { get; set; }

        [Display(Name = "Giới hạn độ tuổi")]
        [Required(ErrorMessage = "Bạn phải chọn giới hạn")]
        public string AgeLimitation { get; set; }

        [Display(Name = "Điểm số")]
        [Range(0, 10, ErrorMessage = "Điểm phải nằm trong khoảng từ 0 đến 10")]
        public double? Rate { get; set; }

        [Display(Name = "Film bom tấn")]
        public bool HotMovie { get; set; }

        [Display(Name = "Sẵn sàng")]
        public bool Available { get; set; }

        [Required(ErrorMessage = "Bạn cần chọn file")]
        public HttpPostedFileBase Thumbnail { get; set; }

        [Required(ErrorMessage = "Bạn cần chọn file")]
        public HttpPostedFileBase WideThumbnail { get; set; }

        [Required(ErrorMessage = "Bạn phải chọn phiên bản")]
        public string Edition { get; set; }

        [Required(ErrorMessage = "Bạn phải chọn dạng phụ đề")]
        public int Subtitle { get; set; }

        [Required(ErrorMessage = "Bạn phải chọn thể loại")]
        public List<string> Genres { get; set; }

        public List<string> Cinemas { get; set; }
    }
}