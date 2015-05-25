using System.Collections.Generic;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.ViewModels {
	public class MovieExtendedModel : Movie {
        public int ScheduleType { get; set; }
	}
}