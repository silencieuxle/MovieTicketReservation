using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface ICinemaMovieRepository : IDisposable {
        IEnumerable<Cine_MovieDetails> GetAllDetails();
        IEnumerable<Cine_MovieDetails> GetDetailsByMovieID(int movieId);
        IEnumerable<Cine_MovieDetails> GetDetailsByCinemaID(string cinemaId);
        IEnumerable<Movie> GetCanBeScheduledMoviesByCinema(string cinemaId);
        Cine_MovieDetails GetDetailsByID(int detailsId);
        Cine_MovieDetails GetDetailsByCinemaIDAndMovieID(string cinemaID, int movieId);
        bool InsertDetails(Cine_MovieDetails cinemaMovieDetails);
        bool UpdateDetails(Cine_MovieDetails cinemaMovieDetails);
        bool DeleteDetails(int detailsId);
    }
}
