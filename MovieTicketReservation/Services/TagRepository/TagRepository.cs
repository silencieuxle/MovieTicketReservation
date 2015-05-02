using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;
using System.Data.Entity;

namespace MovieTicketReservation.Services.TagRepository {
    public class TagRepository : ITagRepository, IDisposable {
        private DbEntities context;

        public TagRepository(DbEntities context) {
            this.context = context;
        }

        #region ITagRepository Members

        public IEnumerable<Tag> GetTags() {
            return context.Tags.ToList();
        }

        public Tag GetTagByID(int tagId) {
            return context.Tags.Find(tagId);
        }

        public bool InsertTag(Tag tag) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Tags.Add(tag);

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

        public bool UpdateTag(Tag tag) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(tag).State = EntityState.Modified;

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

        public bool DeleteTag(int tagId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var tag = context.Tags.Find(tagId);
                    context.Tags.Remove(tag);

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