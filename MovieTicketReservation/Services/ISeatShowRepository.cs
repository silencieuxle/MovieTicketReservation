using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface ISeatShowRepository : IDisposable {
        IEnumerable<Seat_ShowDetails> GetAllDetails();
        IEnumerable<Seat_ShowDetails> GetDetailsByScheduleID(int scheduleId);
        IEnumerable<Seat_ShowDetails> GetDetailsByBookingHeaderID(int bookingHeaderId);
        Seat_ShowDetails GetDetailsBySeatID(int seatId);
        Seat_ShowDetails GetDetailsByScheduleIDAndSeatID(int scheduleId, int seatId);
        bool InsertSeatsWithDetails(int scheduleId, string roomId, string classId);
        bool InsertSeat(Seat_ShowDetails seatShowDetails);
        bool UpdateSeat(Seat_ShowDetails seatShowDetails);
    }
}
