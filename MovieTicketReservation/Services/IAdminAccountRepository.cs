﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IAdminAccountRepository : IDisposable {
        IEnumerable<AdminAccount> GetAdminAccounts();
        AdminAccount GetAdminAccountByMemberID(int memberId);
    }
}
