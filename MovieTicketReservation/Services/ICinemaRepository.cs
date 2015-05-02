using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface ICinemaRepository : IDisposable {
        IEnumerable<Cinema> GetCinemas();
        Cinema GetCinemaByID(string cinemaId);
        bool InsertCinema(Cinema cinema);
        bool UpdateCinema(Cinema cinema);
        bool DeleteCinema(string cinemaId);
    }
}
