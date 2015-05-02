using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTicketReservation.ViewModels {
	public class SeatModel {
		public int SeatId { get; set; }
		public string Name { get; set; }
		public bool Reserved { get; set; }
	}
}