﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTicketReservation.ViewModels {
	public class MovieBasicModel {
		public int MovieId { get; set; }
		public string Title { get; set; }
		public string ThumbnailUrl { get; set; }
		public DateTime BeginShowDate { get; set; }
		public int ScheduleType { get; set; } //Type 1 = Now showing, Type 2 = Comming Soon, Type 3 = Future movies
		public List<EditionModel> Editions { get; set; }
	}
}