using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;
using System.Data.Entity;

namespace MovieTicketReservation.Services.SeatShowDetailsService {
    public class SeatShowRepository : ISeatShowRepository, IDisposable {
        private DbEntities context;

        public SeatShowRepository(DbEntities context) {
            this.context = context;
        }

        #region ISeatShowRepository Members

        public IEnumerable<Seat_ShowDetails> GetAllDetails() {
            return context.Seat_ShowDetails.ToList();
        }

        public IEnumerable<Seat_ShowDetails> GetDetailsByScheduleID(int scheduleId) {
            return context.Seat_ShowDetails.Where(d => d.ScheduleID == scheduleId).ToList();
        }

        public IEnumerable<Seat_ShowDetails> GetDetailsByBookingHeaderID(int bookingHeaderId) {
            return context.Seat_ShowDetails.Where(d => d.BookingHeaderID == bookingHeaderId).ToList();
        }

        public Seat_ShowDetails GetDetailsBySeatID(int seatId) {
            return context.Seat_ShowDetails.FirstOrDefault(d => d.SeatID == seatId);
        }

        public bool InsertSeatsWithDetails(int scheduleId, string roomId, string classId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    List<Seat> seats = context.Seats.Where(s => s.RoomID == roomId).ToList();

                    foreach (var seat in seats) {
                        Seat_ShowDetails details = new Seat_ShowDetails {
                            ClassID = classId,
                            BookingHeaderID = null,
                            Paid = false,
                            Reserved = false,
                            ScheduleID = scheduleId,
                            SeatID = seat.SeatID
                        };
                        context.Seat_ShowDetails.Add(details);
                    }

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

        public bool InsertSeat(Seat_ShowDetails seatShowDetails) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Seat_ShowDetails.Add(seatShowDetails);

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

        public bool UpdateSeat(Seat_ShowDetails seatShowDetails) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(seatShowDetails).State = EntityState.Modified;

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