using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IGenreRepository : IDisposable {
        IEnumerable<MovieGenre> GetMovieGenres();
        IEnumerable<MovieGenre> GetMovieGenresByMovieID(int movieId);
        MovieGenre GetMovieGenreByID(string genreId);
        bool InsertMovieGenreForMovie(int movieId, List<MovieGenre> genreList);
        bool InsertMovieGenre(MovieGenre movieGenre);
        bool UpdateMovieGenre(MovieGenre movieGenre);
        bool DeleteMovieGenre(string movieGenreId);
    }
}
