using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;
using System.Data.Entity;

namespace MovieTicketReservation.Services.MemberService {
    public class MemberRepository : IMemberRepository, IDisposable {
        private DbEntities context;

        public MemberRepository(DbEntities context) {
            this.context = context;
        }

        #region IMemberRepository Members

        public IEnumerable<Member> GetAllMembers() {
            return context.Members.ToList();
        }

        public IEnumerable<Member> GetMembersByRole(int roleId) {
            return context.Members.Where(m => m.Role.RoleID == roleId);
        }

        public Member GetMemberByID(int memberId) {
            return context.Members.Find(memberId);
        }

        public Member GetMemberByEmailAndPassword(string email, string password) {
            return context.Members.FirstOrDefault(m => m.Email == email && m.Password == password);
        }

        public bool InsertMember(Member member) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Members.Add(member);

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool UpdateMember(Member member) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(member).State = EntityState.Modified;

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool DeleteMember(int memberId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var member = context.Members.Find(memberId);
                    context.Members.Remove(member);

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool IsEmailExisted(string email) {
            return context.Members.Any(x => x.Email == email);
        }

        public bool IsIdCardExisted(string idCardNumber) {
            return context.Members.Any(x => x.IDCardNumber == idCardNumber);
        }

        public bool Authenticate(string email, string password) {
            return context.Members.Any(m => m.Email == email && m.Password == password);
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