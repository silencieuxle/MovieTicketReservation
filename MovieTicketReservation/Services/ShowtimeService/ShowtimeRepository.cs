using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;
using System.Data.Entity;

namespace MovieTicketReservation.Services.ShowtimeService {
    public class ShowtimeRepository : IShowtimeRepository, IDisposable {
        private DbEntities context;

        public ShowtimeRepository(DbEntities context) {
            this.context = context;
        }

        #region IShowtimeRepository Members

        public IEnumerable<ShowTime> GetShowtimes() {
            return context.ShowTimes.ToList();
        }

        public IEnumerable<ShowTime> GetAvailableShowtimes() {
            return context.ShowTimes.Where(s => ((TimeSpan)s.StartTime).Hours >= DateTime.Now.Hour).ToList();
        }

        public ShowTime GetShowtimeByID(int showtimeId) {
            return context.ShowTimes.Find(showtimeId);
        }

        public bool InsertShowtime(ShowTime showtime) {
            using (var tracsaction = context.Database.BeginTransaction()) {
                try {
                    context.ShowTimes.Add(showtime);

                    context.SaveChanges();

                    tracsaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    tracsaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool UpdateShowtime(ShowTime showtime) {
            using (var tracsaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(showtime).State = EntityState.Modified;

                    context.SaveChanges();

                    tracsaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    tracsaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool DeleteShowtime(int showtimeId) {
            using (var tracsaction = context.Database.BeginTransaction()) {
                try {
                    var showtime = context.ShowTimes.Find(showtimeId);
                    context.ShowTimes.Remove(showtime);

                    context.SaveChanges();

                    tracsaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    tracsaction.Rollback();
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