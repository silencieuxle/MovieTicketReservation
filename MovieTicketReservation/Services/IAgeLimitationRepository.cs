﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IAgeLimitationRepository : IDisposable {
        IEnumerable<AgeLimitation> GetAgeLimitations();
        AgeLimitation GetAgeLimitationByID(string ageLimitationId);
    }
}
