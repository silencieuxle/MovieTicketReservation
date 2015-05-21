using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IShowtimeRepository : IDisposable {
        IEnumerable<ShowTime> GetShowtimes();
        IEnumerable<ShowTime> GetAvailableShowtimes();
        ShowTime GetShowtimeByID(int showtimeId);
        bool InsertShowtime(ShowTime showtime);
        bool UpdateShowtime(ShowTime showtime);
        bool DeleteShowtime(int showtimeId);
    }
}
