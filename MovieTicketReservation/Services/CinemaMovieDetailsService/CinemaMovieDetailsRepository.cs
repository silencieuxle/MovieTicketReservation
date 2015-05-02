using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;
using System.Data.Entity;

namespace MovieTicketReservation.Services.CinemaMovieDetailsService {
    public class CinemaMovieDetailsRepository : ICinemaMovieRepository, IDisposable {
        private DbEntities context;

        public CinemaMovieDetailsRepository(DbEntities context) {
            this.context = context;
        }

        #region ICinemaMovieRepository Members

        public IEnumerable<Cine_MovieDetails> GetAllDetails() {
            return context.Cine_MovieDetails.ToList();
        }

        public IEnumerable<Cine_MovieDetails> GetDetailsByMovieID(int movieId) {
            return context.Cine_MovieDetails.Where(d => d.MovieID == movieId).ToList();
        }

        public Cine_MovieDetails GetDetailsByID(int detailsId) {
            return context.Cine_MovieDetails.Find(detailsId);
        }

        public Cine_MovieDetails GetDetailsByCinemaIDAndMovieID(string cinemaID, int movieId) {
            return context.Cine_MovieDetails.FirstOrDefault(d => d.CinemaID == cinemaID && d.MovieID == movieId);
        }

        public bool InsertDetails(Cine_MovieDetails cinemaMovieDetails) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Cine_MovieDetails.Add(cinemaMovieDetails);

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return false;
        }

        public bool UpdateDetails(Cine_MovieDetails cinemaMovieDetails) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(cinemaMovieDetails).State = EntityState.Modified;

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return false;
        }

        public bool DeleteDetails(int detailsId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var cinemaMovieDetails = context.Cine_MovieDetails.Find(detailsId);
                    context.Cine_MovieDetails.Remove(cinemaMovieDetails);

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return false;
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