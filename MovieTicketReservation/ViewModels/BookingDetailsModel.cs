using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTicketReservation.ViewModels {
	public class BookingDetailsModel {
        public Int32 ScheduleId { get; set; }
        public string MovieTitle { get; set; }
        public string Cinema { get; set; }
        public string Room { get; set; }
		public TimeSpan Showtime { get; set; }
		public DateTime ReservedDate { get; set; }
		public List<String> Seats { get; set; }
        public decimal Total { get; set; }
	}
}