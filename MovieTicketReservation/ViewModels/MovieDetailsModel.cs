using System.Collections.Generic;

namespace MovieTicketReservation.Models {
	public class MovieDetailsModel : MovieBasicModel {
		public bool Available { get; set; }
		public int Duration { get; set; }
		public int Length { get; set; }
		public List<GenreModel> Genres { get; set; }
		public List<CinemaModel> Cinemas { get; set; }
		public string Actors { get; set; }
		public string AgeLimit { get; set; }
		public string Description { get; set; }
		public string Director { get; set; }
		public string LongDescription { get; set; }
		public string ReleasedDate { get; set; }
		public string TrailerUrl { get; set; }
		public string WideThumbnail { get; set; }
	}
}