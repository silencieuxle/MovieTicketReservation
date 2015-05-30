using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;
using System.Data.Entity;

namespace MovieTicketReservation.Services.PromotionService {
    public class PromotionRepository : IPromotionService, IDisposable {
        private DbEntities context;

        public PromotionRepository(DbEntities context) {
            this.context = context;
        }

        #region IPromotionService Members

        public IEnumerable<Promote> GetPromotions() {
            return context.Promotes.ToList();
        }

        public IEnumerable<Promote> GetPromotionsByCinema(string cinemaId) {
            return context.Cinemas.Find(cinemaId).Promotes.ToList();
        }

        public IEnumerable<Promote> GetFixedDayOfWeekPromotions() {
            return context.Promotes.Where(p => p.FixedDayOfWeek != null).ToList();
        }

        public IEnumerable<Promote> GetDuratedPromotions() {
            return context.Promotes.Where(p => p.BeginDay != null).ToList();
        }

        public IEnumerable<Promote> GetActiveDuratedPromotions() {
            return context.Promotes.ToList().Where(p => p.BeginDay != null && ((DateTime)p.BeginDay).AddDays((int)p.Duration) >= DateTime.Now);
        }

        public Promote GetPromotionByID(int promotionId) {
            return context.Promotes.Find(promotionId);
        }

        public Promote GetFixedDayOfWeekPromotionByDay(int dayOfWeek) {
            return context.Promotes.FirstOrDefault(p => p.FixedDayOfWeek == dayOfWeek);
        }

        public Promote GetPromotionByScheduleID(int scheduleId) {
            return context.Schedules.Find(scheduleId).Promote;
        }

        public bool Insert(Promote promotion) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Promotes.Add(promotion);

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

        public bool Delete(int promotionId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var promotion = context.Promotes.Find(promotionId);

                    context.Promotes.Remove(promotion);

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