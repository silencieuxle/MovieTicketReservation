using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IBookingRepository : IDisposable {
        IEnumerable<BookingHeader> GetBookingHeaders();
        IEnumerable<BookingHeader> GetBookingHeadersByMemberID(int memberId);
        IEnumerable<BookingHeader> GetBookingHeadersByDate(DateTime date);
        IEnumerable<BookingHeader> GetBookingHeadersByMovieID(int movieId);

        BookingHeader GetBookingHeaderByID(int bookingHeaderId);
        int InsertBookingHeader(BookingHeader bookingHeader);
        int UpdateBookingHeader(BookingHeader bookingHeader);
        int DeleteBookingHeader(int bookingHeaderId);
    }
}
