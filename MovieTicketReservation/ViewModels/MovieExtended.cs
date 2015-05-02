using System.Collections.Generic;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.ViewModels {
	public class MovieExtended : Movie {
        public int ScheduleType { get; set; }
	}
}