using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services.RoomService {
    public class RoomRepository : IRoomRepository, IDisposable {
        private DbEntities context;

        public RoomRepository(DbEntities context) {
            this.context = context;
        }

        #region IRoomRepository Members

        public IEnumerable<Room> GetRooms() {
            return context.Rooms.ToList();
        }

        public IEnumerable<Room> GetRoomsByCinemaID(string cinemaId) {
            return context.Rooms.Where(r => r.CinemaID == cinemaId).ToList();
        }

        public Room GetRoomByID(int roomId) {
            return context.Rooms.Find(roomId);
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