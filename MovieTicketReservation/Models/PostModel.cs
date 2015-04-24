using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTicketReservation.Models {
    public class PostModel {
        public int NewId { get; set; }
        public string ThumbnailUrl { get; set; }
        public string PostTitle { get; set; }
    }
}