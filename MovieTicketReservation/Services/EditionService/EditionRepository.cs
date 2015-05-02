using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services.EditionService {
    public class EditionRepository : IEditionRepository, IDisposable {
        private DbEntities context;

        public EditionRepository(DbEntities context) {
            this.context = context;
        }

        #region IEditionRepository Members

        public IEnumerable<MovieEdition> GetMovieEditions() {
            return context.MovieEditions.ToList();
        }

        public MovieEdition GetMovieEditionByID(string editionId) {
            return context.MovieEditions.Find(editionId);
        }

        public bool InsertMovieEdition(MovieEdition movieEdition) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.MovieEditions.Add(movieEdition);

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

        public bool UpdateMovieEdition(MovieEdition movieEdition) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(movieEdition).State = EntityState.Modified;

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

        public bool DeleteMovieEdition(string movieEditionId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var movieEdition = context.MovieEditions.Find(movieEditionId);
                    context.MovieEditions.Remove(movieEdition);

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