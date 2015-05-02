using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieTicketReservation.ViewModels {
	public class Showtime {
		public int ScheduleId { get; set; }
		public TimeSpan Time { get; set; }
	}
}
