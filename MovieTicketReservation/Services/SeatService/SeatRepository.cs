using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services.SeatService {
    public class SeatRepository : ISeatRepository, IDisposable {
        private DbEntities context;

        public SeatRepository(DbEntities context) {
            this.context = context;
        }

        #region ISeatRepository Members

        public IEnumerable<Seat> GetSeats() {
            return context.Seats.ToList();
        }

        public Seat GetSeatsByID(int seatId) {
            return context.Seats.Find(seatId);
        }

        public IEnumerable<Seat> GetSeatsByRoomID(string roomId) {
            return context.Seats.Where(s => s.RoomID == roomId);
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