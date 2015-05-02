using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTicketReservation.ViewModels {
	public class CinemaScheduleModel {
		public string CinemaId { get; set; }
		public string CinemaName { get; set; }
		public List<DateModel> Dates { get; set; }
	}
}