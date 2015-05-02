using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services.CinemaImageService {
    public class CinemaImageRepository : ICinemaImagesRepositorycs, IDisposable {
        private DbEntities context;

        public CinemaImageRepository(DbEntities context) {
            this.context = context;
        }

        #region ICinemaImagesRepositorycs Members

        public IEnumerable<CinemaImage> GetCinemaImages() {
            return context.CinemaImages.ToList();
        }

        public IEnumerable<CinemaImage> GetCinemaImagesByCinemaID(string cinemaId) {
            return context.CinemaImages.Where(i => i.CinemaID == cinemaId).ToList();
        }

        public CinemaImage GetCinemaImageByID(int imageId) {
            return context.CinemaImages.Find(imageId);
        }

        public bool InsertCinemaImage(CinemaImage cinemaImage) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.CinemaImages.Add(cinemaImage);

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

        public bool UpdateCinemaImage(CinemaImage cinemaImage) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(cinemaImage).State = EntityState.Modified;

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

        public bool DeleteCinemaImage(int cinemaImageId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var cinemaImage = context.CinemaImages.Find(cinemaImageId);
                    context.CinemaImages.Remove(cinemaImage);

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