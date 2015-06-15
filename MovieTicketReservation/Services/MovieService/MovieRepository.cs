using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;
using System.Data.Entity;

namespace MovieTicketReservation.Services.MovieService {
    public class MovieRepository : IMovieRepository, IDisposable {
        private DbEntities context;

        public MovieRepository(DbEntities context) {
            this.context = context;
        }

        #region IMovieRepository Members

        public IEnumerable<Movie> GetCanBeReservedMovies() {
            var movies = context.Movies.ToList();
            return movies.Where(m => GetScheduleType(m) == 1 || GetScheduleType(m) == 2);
        }

        public IEnumerable<Movie> GetCanBeScheduledMovies() {
            var movies = context.Movies.ToList();
            return movies.Where(m => GetScheduleType(m) != 0);
        }

        public IEnumerable<Movie> GetCommingMovies() {
            return context.Movies.ToList().Where(m => GetScheduleType(m) == 2);
        }

        public IEnumerable<Movie> GetFutureMovies() {
            return context.Movies.ToList().Where(m => GetScheduleType(m) == 3);
        }

        public IEnumerable<Movie> GetAvailableMovies() {
            return context.Movies.Where(m => (bool)m.Available == true).ToList();
        }

        public IEnumerable<Movie> GetAllMovies() {
            return context.Movies.ToList();
        }

        public IEnumerable<Movie> GetMoviesByScheduleTypes(string[] scheduleTypes, IEnumerable<Movie> data = null) {
            if (data == null) {
                if (scheduleTypes.Count() == 0) return context.Movies.ToList();
                var movies = context.Movies.ToList();
                return movies.Where(m => scheduleTypes.Any(g => GetScheduleType(m) == Convert.ToInt32(g)));
            } else {
                if (scheduleTypes.Count() == 0) return data.ToList();
                var movies = data.ToList();
                return movies.Where(m => scheduleTypes.Any(g => GetScheduleType(m) == Convert.ToInt32(g)));
            }
        }

        public IEnumerable<Movie> GetMoviesByGenres(string[] genreIds, IEnumerable<Movie> data = null) {
            if (data == null) {
                if (genreIds.Count() == 0) return context.Movies.ToList();
                var movies = context.Movies.ToList();
                return context.Movies.Where(m => genreIds.Any(g => m.MovieGenres.Any(me => me.GenreID == g))).ToList();
            } else {
                if (genreIds.Count() == 0) return data.ToList();
                var movies = data.ToList();
                return movies.Where(m => genreIds.Any(g => m.MovieGenres.Any(me => me.GenreID == g))).ToList();
            }
        }

        public IEnumerable<Movie> GetMoviesByEditions(string[] editionIds, IEnumerable<Movie> data = null) {
            if (data == null) {
                if (editionIds.Count() == 0) return context.Movies.ToList();
                var movies = context.Movies.ToList();
                return movies.Where(m => editionIds.Any(e => m.EditionID == e)).ToList();
            } else {
                if (editionIds.Count() == 0) return data.ToList();
                var movies = data.ToList();
                return movies.Where(m => editionIds.Any(e => m.EditionID == e)).ToList();
            }
        }

        public IEnumerable<Movie> GetMoviesByCinemas(string[] cinemaIds, IEnumerable<Movie> data = null) {
            if (data == null) {
                if (cinemaIds.Count() == 0) return context.Movies.ToList();
                var movies = context.Movies.ToList();
                return movies.Where(m => cinemaIds.Any(c => m.Cine_MovieDetails.Any(me => me.CinemaID == c))).ToList();
            } else {
                if (cinemaIds.Count() == 0) return data.ToList();
                var movies = data.ToList();
                return movies.Where(m => cinemaIds.Any(e => m.Cine_MovieDetails.Any(me => me.CinemaID == e))).ToList();
            }
        }

        public IEnumerable<Movie> GetMoviesByCinemaID(string cinemaId) {
            return context.Cine_MovieDetails.Where(d => d.CinemaID == cinemaId).Select(d => d.Movie).ToList();
        }

        public IEnumerable<Movie> GetMoviesByAgeLitmitation(string[] ageLimitationIds, IEnumerable<Movie> data = null) {
            if (data == null) {
                if (ageLimitationIds.Count() == 0) return context.Movies.ToList();
                var movies = context.Movies.ToList();
                return movies.Where(m => ageLimitationIds.Any(c => m.AgeLimitationID == c)).ToList();
            } else {
                if (ageLimitationIds.Count() == 0) return data.ToList();
                var movies = data.ToList();
                return movies.Where(m => ageLimitationIds.Any(e => m.AgeLimitationID == e)).ToList();
            }
        }

        public IEnumerable<Movie> GetMoviesByRate(IEnumerable<Movie> data = null) {
            if (data == null) {
                return context.Movies.Where(m => m.Rate >= 7).ToList();
            } else {
                return data.ToList().Where(m => m.Rate >= 7).ToList();
            }
        }

        public IEnumerable<Movie> GetMoviesByTitle(string title) {
            return context.Movies.Where(m => m.Title.ToLower().Contains(title.ToLower())).ToList();
        }

        public IEnumerable<Movie> GetHotMovies(IEnumerable<Movie> data = null) {
            if (data == null) {
                return context.Movies.Where(m => (bool)m.HotMovie).ToList();
            }
            return data.ToList().Where(m => m.HotMovie == true).ToList();
        }

        public Movie GetMovieByID(int movieId) {
            return context.Movies.Find(movieId);
        }

        public bool InsertCineMovieDetailsByCinemaID(string cinemaID, Movie movie) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Movies.Add(movie);
                    var cinema = context.Cinemas.Find(cinemaID);

                    if (cinema != null) {
                        cinema.Cine_MovieDetails.Add(new Cine_MovieDetails {
                            CinemaID = cinemaID,
                            MovieID = movie.MovieID
                        });
                    } else {
                        return false;
                    }

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool InsertMovie(Movie movie) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Movies.Add(movie);

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool UpdateMovie(Movie movie) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(movie).State = EntityState.Modified;

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool DeleteMovie(int movieId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    Movie movie = context.Movies.Find(movieId);
                    context.Movies.Remove(movie);

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Private methods

        private int GetScheduleType(Movie movie) {
            var beginShowDate = ((DateTime)movie.BeginShowDate).Date;
            var duration = (int)movie.Duration;

            if (beginShowDate.AddDays(duration) >= DateTime.Now) {
                if (beginShowDate <= DateTime.Now.Date && DateTime.Now.Date <= beginShowDate.AddDays(duration))
                    return 1; //Now showing Movie
                else if (beginShowDate <= DateTime.Now.AddDays(14).Date && beginShowDate > DateTime.Now.Date)
                    return 2; //Coming Soon Movies
                else if (beginShowDate > DateTime.Now.AddDays(14).Date)
                    return 3; // Future Movies
            }
            return 0; // Ended Movies
        }

        #endregion

        #region IDisposable Members

        private bool disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (!this.disposed) {
                if (disposing) {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
