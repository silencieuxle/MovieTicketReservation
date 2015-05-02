using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IGenreRepository : IDisposable {
        IEnumerable<MovieGenre> GetMovieGenres();
        MovieGenre GetMovieGenreByID(string genreId);
        bool InsertMovieGenre(MovieGenre movieGenre);
        bool UpdateMovieGenre(MovieGenre movieGenre);
        bool DeleteMovieGenre(string movieGenreId);
    }
}
