using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services.SubtitleService {
    public class SubtitleRepository : ISubtitleRepository, IDisposable {
        private DbEntities context;

        public SubtitleRepository(DbEntities context) {
            this.context = context;
        }

        #region ISubtitleRepository Members

        public IEnumerable<Subtitle> GetSubtitles() {
            return context.Subtitles.ToList();
        }

        public Subtitle GetSubtitleByID(int subtitleId) {
            return context.Subtitles.Find(subtitleId);
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