﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface ISeatRepository : IDisposable {
        IEnumerable<Seat> GetSeats();
        IEnumerable<Seat> GetSeatsByRoomID(string roomId);
        Seat GetSeatsByID(int seatId);
    }
}
