using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services.CinemaService {
    public class CinemaRepository : ICinemaRepository, IDisposable {
        private DbEntities context;

        public CinemaRepository(DbEntities context) {
            this.context = context;
        }

        #region ICinemaRepository Members

        public IEnumerable<Cinema> GetCinemas() {
            return context.Cinemas.ToList();
        }

        public Cinema GetCinemaByID(string cinemaId) {
            return context.Cinemas.Find(cinemaId);
        }

        public bool InsertCinema(Cinema cinema) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Cinemas.Add(cinema);

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

        public bool UpdateCinema(Cinema cinema) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(cinema).State = EntityState.Modified;

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

        public bool DeleteCinema(string cinemaId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var cinema = context.Cinemas.Find(cinemaId);

                    context.Cinemas.Remove(cinema);

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