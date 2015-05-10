using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace MovieTicketReservation.Hubs {
    public class TicketHub : Hub {
        public void Notify(int seatId) {
            Clients.All.checkSeat(seatId);
        }
    }
}