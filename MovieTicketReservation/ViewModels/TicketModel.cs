using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTicketReservation.ViewModels {
	public class TicketModel {
		public int BookingHeaderId { get; set; }
		public string MovieTitle { get; set; }
        public DateTime ReservedDate { get; set; }
        public CinemaModel Cinema { get; set; }
        public string RoomName { get; set; }
		public DateTime ShowDate { get; set; }
		public TimeSpan ShowTime { get; set; }
		public List<String> Seats { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool IsTaken { get; set; }
        public decimal Total { get; set; }
	}
}