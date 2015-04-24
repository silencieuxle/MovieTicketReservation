using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTicketReservation.Models {
	public class DateShowtime {
		public DateTime Date { get; set; }
		public Showtime[] Time { get; set; }
	}
}