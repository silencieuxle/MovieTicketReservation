using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services.AgeLimitationService {
    public class AgeLimitationRepository : IAgeLimitationRepository, IDisposable {
        private DbEntities context;

        public AgeLimitationRepository(DbEntities context) {
            this.context = context;
        }

        #region IAgeLimitationRepository Members

        public IEnumerable<AgeLimitation> GetAgeLimitations() {
            return context.AgeLimitations.ToList();
        }

        public AgeLimitation GetAgeLimitationByID(string ageLimitationId) {
            return context.AgeLimitations.Find(ageLimitationId);
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