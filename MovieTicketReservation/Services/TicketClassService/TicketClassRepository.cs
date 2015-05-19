using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services.TicketClassService {
    public class TicketClassRepository : ITicketClassRepository, IDisposable {
        private DbEntities context;

        public TicketClassRepository(DbEntities context) {
            this.context = context;
        }

        #region ITicketClassRepository Members

        public IEnumerable<TicketClass> GetTicketClasses() {
            return context.TicketClasses.ToList();
        }

        public TicketClass GetTicketClassByID(string classId) {
            return context.TicketClasses.Find(classId);
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