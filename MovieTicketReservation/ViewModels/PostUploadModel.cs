using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieTicketReservation.ViewModels {
    public class PostUploadModel {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public string Tags { get; set; }

        [Required]
        public HttpPostedFileBase ThumbnailURL { get; set; }
    }
}