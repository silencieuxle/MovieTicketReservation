using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services.BookingHeaderService {
    public class BookingHeaderRepository : IBookingRepository, IDisposable {
        private DbEntities context;

        public BookingHeaderRepository(DbEntities context) {
            this.context = context;
        }

        #region IBookingRepository Members

        public IEnumerable<BookingHeader> GetBookingHeaders() {
            return context.BookingHeaders.ToList();
        }

        public IEnumerable<BookingHeader> GetBookingHeadersByMemberID(int memberId) {
            return context.BookingHeaders.Where(b => b.MemberID == memberId).ToList();
        }

        public IEnumerable<BookingHeader> GetBookingHeadersByDate(DateTime date) {
            return context.BookingHeaders.ToList().Where(b => ((DateTime)b.ReservedTime).Date == date.Date);
        }

        public IEnumerable<BookingHeader> GetBookingHeadersByMovieID(int movieId) {
            return context.BookingHeaders.Where(b => b.MovieID == movieId).ToList();
        }

        public BookingHeader GetBookingHeaderByID(int bookingHeaderId) {
            return context.BookingHeaders.Find(bookingHeaderId);
        }

        public int InsertBookingHeader(BookingHeader bookingHeader) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.BookingHeaders.Add(bookingHeader);

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return 0;
                }
            }
            return bookingHeader.HeaderID;
        }

        public int UpdateBookingHeader(BookingHeader bookingHeader) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(bookingHeader).State = EntityState.Modified;

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return 0;
                }
            }
            return bookingHeader.HeaderID;
        }

        public int DeleteBookingHeader(int bookingHeaderId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var bookingHeader = context.BookingHeaders.Find(bookingHeaderId);
                    context.BookingHeaders.Remove(bookingHeader);

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return 0;
                }
            }
            return bookingHeaderId;
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