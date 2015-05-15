using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IMovieRepository : IDisposable {
        IEnumerable<Movie> GetCanBeReservedMovies();
        IEnumerable<Movie> GetAvailableMovies();
        IEnumerable<Movie> GetAllMovies();
        IEnumerable<Movie> GetMoviesByScheduleTypes(string[] scheduleTypes, IEnumerable<Movie> data = null);
        IEnumerable<Movie> GetMoviesByGenres(string[] genreIds, IEnumerable<Movie> data = null);
        IEnumerable<Movie> GetMoviesByEditions(string[] editionIds, IEnumerable<Movie> data = null);
        IEnumerable<Movie> GetMoviesByCinemas(string[] cinemaIds, IEnumerable<Movie> data = null);
        IEnumerable<Movie> GetMoviesByAgeLitmitation(string[] ageLimitationIds, IEnumerable<Movie> data = null);
        IEnumerable<Movie> GetMoviesByRate(IEnumerable<Movie> data = null);
        IEnumerable<Movie> GetHotMovies(IEnumerable<Movie> data = null);
        IEnumerable<Movie> GetMoviesByTitle(string title);
        IEnumerable<Movie> GetMoviesByCinemaID(string cinemaId);
        Movie GetMovieByID(int movieId);
        bool InsertMovie(Movie movie);
        bool UpdateMovie(Movie movie);
        bool DeleteMovie(int movieId);
    }
}
