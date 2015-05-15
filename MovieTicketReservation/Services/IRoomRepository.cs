using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IRoomRepository : IDisposable {
        IEnumerable<Room> GetRooms();
        IEnumerable<Room> GetRoomsByCinemaID(string cinemaId);
        Room GetRoomByID(int roomId);
    }
}
