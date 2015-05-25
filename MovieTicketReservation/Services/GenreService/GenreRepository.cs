using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services.GenreService {
    public class GenreRepository : IGenreRepository, IDisposable {
        private DbEntities context;

        public GenreRepository(DbEntities context) {
            this.context = context;
        }

        #region IGenreRepository Members

        public IEnumerable<MovieGenre> GetMovieGenres() {
            return context.MovieGenres.ToList();
        }

        public IEnumerable<MovieGenre> GetMovieGenresByMovieID(int movieId) {
            return context.Movies.Find(movieId).MovieGenres.ToList();
        }

        public MovieGenre GetMovieGenreByID(string genreId) {
            return context.MovieGenres.Find(genreId);
        }

        public bool InsertMovieGenreForMovie(int movieId, List<MovieGenre> genreList) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var movie = context.Movies.Find(movieId);
                    foreach (var item in genreList) {
                        movie.MovieGenres.Add(item);
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

        public bool InsertMovieGenre(MovieGenre movieGenre) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.MovieGenres.Add(movieGenre);

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

        public bool UpdateMovieGenre(MovieGenre movieGenre) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(movieGenre).State = EntityState.Modified;

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

        public bool DeleteMovieGenre(string movieGenreId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var movieGenre = context.MovieGenres.Find(movieGenreId);
                    context.MovieGenres.Remove(movieGenre);

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