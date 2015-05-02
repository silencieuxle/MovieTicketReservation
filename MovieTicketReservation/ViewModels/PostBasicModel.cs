using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTicketReservation.ViewModels {
    public class PostBasicModel {
        public int NewId { get; set; }
        public string ThumbnailUrl { get; set; }
        public string PostTitle { get; set; }
        public int ViewCount { get; set; }
        public DateTime PostedDate { get; set; }
    }
}