using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface ITicketClassRepository : IDisposable {
        IEnumerable<TicketClass> GetTicketClasses();
        TicketClass GetTicketClassByID(string classId);
    }
}
