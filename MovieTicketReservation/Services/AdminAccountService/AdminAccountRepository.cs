using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services.AdminAccountService {
    public class AdminAccountRepository : IAdminAccountRepository, IDisposable {
        private DbEntities context;

        public AdminAccountRepository(DbEntities context) {
            this.context = context;
        }

        #region IAdminAccountRepository Members

        public IEnumerable<AdminAccount> GetAdminAccounts() {
            return context.AdminAccounts.ToList();
        }

        public AdminAccount GetAdminAccountByMemberID(int memberId) {
            return context.AdminAccounts.FirstOrDefault(a => a.MemberID == memberId);
        }

        #endregion

        #region IDisposable Members

        private bool disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (!this.disposed) {
                if (disposing) {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}