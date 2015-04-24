using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieTicketReservation.Models {
	public class Date {
		public DateTime ShowingDate { get; set; }
		public List<Showtime> Showtimes;
	}
}
