using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieTicketReservation.ViewModels {
	public class DateModel {
		public DateTime ShowingDate { get; set; }
		public List<Showtime> Showtimes;
	}
}
