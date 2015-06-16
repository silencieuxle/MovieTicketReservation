using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;
using System.Data.Entity;

namespace MovieTicketReservation.Services.NewsRepository {
    public class NewsRepository : INewsRepository, IDisposable {
        private DbEntities context;

        public NewsRepository(DbEntities context) {
            this.context = context;
        }

        #region IPostRepository Members

        public IEnumerable<News> GetNews() {
            return context.News.ToList();
        }

        public IEnumerable<News> GetNewsByTag(int tagId) {
            var tag = context.Tags.Find(tagId);
            return context.News.Where(n => n.Tags.Contains(tag));
        }

        public IEnumerable<News> GetNewsByPostedDate(DateTime date) {
            return context.News.Where(n => ((DateTime)n.PostedDate).Date == date.Date).ToList();
        }

        public News GetNewsByID(int newsId) {
            return context.News.Find(newsId);
        }

        public bool InsertNews(News news) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.News.Add(news);

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

        public bool UpdateNews(News news) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(news).State = EntityState.Modified;

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

        public bool DeleteNews(int newsId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var news = context.News.Find(newsId);
                    context.News.Remove(news);

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
