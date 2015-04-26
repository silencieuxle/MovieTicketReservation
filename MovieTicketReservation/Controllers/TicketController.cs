using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MovieTicketReservation.Models;
using MovieTicketReservation.App_Code;

namespace MovieTicketReservation.Controllers {
    public class TicketController : Controller {
        readonly MoviesDbDataContext _db = new MoviesDbDataContext();

        // GET: Ticket
        public ActionResult Index() {
            if (Session["UID"] == null) {
                Session["RedirectURL"] = Request.RawUrl;
                return Redirect("/User/Login");
            }
            var memberId = (int)Session["UID"];
            var member = _db.Members.FirstOrDefault(m => m.MemberID == memberId);
            var bookingHeaders = _db.BookingHeaders.Where(d => d.MemberID == member.MemberID);

            List<TicketModel> ticketModels = new List<TicketModel>();

            foreach (var item in bookingHeaders) {
                var seats = _db.Seat_ShowDetails.Where(s => s.BookingHeaderID == item.HeaderID).ToList();
                var schedule = seats[0].Schedule;
                var ticketModel = new TicketModel {
                    BookingHeaderId = item.HeaderID,
                    Cinema = new CinemaModel {
                        Name = schedule.Cine_MovieDetail.Cinema.Name,
                        CinemaId = schedule.Cine_MovieDetail.Cinema.CinemaID
                    },
                    MovieTitle = schedule.Cine_MovieDetail.Movie.Title,
                    ReservedDate = (DateTime)item.ReservedTime,
                    RoomName = schedule.Room.Name,
                    Seats = seats.Select(s => s.Seat.Name).ToList(),
                    ShowDate = (DateTime)schedule.Date,
                    ShowTime = (TimeSpan)schedule.ShowTime.StartTime,
                    ThumbnailUrl = schedule.Cine_MovieDetail.Movie.ThumbnailURL,
                    IsTaken = (bool)item.Took
                };
                ticketModels.Add(ticketModel);
            }
            return View(ticketModels);
        }

        public ActionResult Reserve(int scheduleId) {
            if (Session["UID"] == null) {
                Session["RedirectURL"] = Request.RawUrl;
                return Redirect("/User/Login");
            }
            var showingDate = _db.Schedules.FirstOrDefault(s => s.ScheduleID == scheduleId).Date;
            if (showingDate < DateTime.Now) return View("/Shared/Error");
            Session["Schedule"] = scheduleId;
            Session["ReservedSeats"] = new List<int>();
            var seats = _db.Seat_ShowDetails.Where(x => x.ScheduleID == scheduleId).Select(x => new SeatModel {
                SeatId = x.SeatID,
                Name = x.Seat.Name,
                Reserved = (bool)x.Reserved
            }).ToList();
            return View(seats);
        }

        [HttpGet]
        public ActionResult Confirm() {
            if (Session["Schedule"] == null) return Redirect("/Home/");
            var scheduleId = (int)Session["Schedule"];
            var schedule = _db.Schedules.FirstOrDefault(x => x.ScheduleID == scheduleId);
            var movie = _db.Schedules.FirstOrDefault(x => x.ScheduleID == scheduleId).Cine_MovieDetail.Movie;
            var seats = (List<int>)Session["ReservedSeats"];
            var showtime = (TimeSpan)_db.Schedules.FirstOrDefault(x => x.ScheduleID == scheduleId).ShowTime.StartTime;
            var details = new BookingDetailsModel {
                Cinema = schedule.Cine_MovieDetail.Cinema.Name,
                Room = schedule.Room.Name,
                MovieTitle = movie.Title,
                ReservedDate = DateTime.Now,
                ScheduleId = scheduleId,
                Seats = _db.Seats.Where(dbs => seats.Contains(dbs.SeatID)).Select(x => new String(x.Name.ToCharArray())).ToList(),
                Showtime = showtime
            };
            return View(details);
        }

        [HttpGet]
        public ActionResult CheckOut() {
            if (Session["Schedule"] == null) return Redirect("/Home/");
            int bookingHeaderId;
            bool error = false;
            if ((bookingHeaderId = CreateBookingHeader()) != 0) {
                int totalSeat = CheckSeats(bookingHeaderId);
                if (totalSeat != -1) {
                    if (totalSeat == UpdateTotalSeat(bookingHeaderId, totalSeat))
                        error = false;
                } else {
                    error = true;
                }
            }

            if (error) {
                return View("/Shared/Error.cshtml");
            } else {
                Session["Schedule"] = null;
                Session["ReservedSeats"] = null;
                return View();
            }
        }

        public ActionResult CancelConfirmation() {
            if (Session["Schedule"] == null) return Redirect("/Home/");
            Session["Schedule"] = null;
            Session["ReservedSeats"] = null;
            return Redirect("/Movies/");
        }

        #region Ajax methods
        [HttpPost]
        public void AjaxAddSeat(int seatId) {
            if (Session["Schedule"] == null) return;
            ((List<int>)Session["ReservedSeats"]).Add(seatId);
        }

        [HttpPost]
        public void AjaxRemoveSeat(int seatId) {
            if (Session["Schedule"] == null) return;
            ((List<int>)Session["ReservedSeats"]).Remove(seatId);
        }

        public ActionResult AjaxCancelReservation(int bookingHeaderId) {
            if (Session["UID"] == null) return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            _db.Connection.Open();
            _db.Transaction = _db.Connection.BeginTransaction();
            try {
                var bookingHeader = _db.BookingHeaders.FirstOrDefault(b => b.HeaderID == bookingHeaderId);
                _db.BookingHeaders.DeleteOnSubmit(bookingHeader);
                var seats = _db.Seat_ShowDetails.Where(s => s.BookingHeaderID == bookingHeader.HeaderID);
                foreach (var seat in seats) {
                    seat.BookingHeaderID = null;
                    seat.Reserved = false;
                    seat.Paid = false;
                }
                _db.SubmitChanges();
                _db.Transaction.Commit();
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                _db.Transaction.Rollback();
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            } finally {
                _db.Connection.Close();
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        /// <summary>
        /// Create booking header as the header for reserved seats.
        /// </summary>  
        /// <returns>If the booking header is created, return the booking header ID, else return null.</returns>
        private int CreateBookingHeader() {
            var schedule = Session["Schedule"];
            var sessionString = Helper.GenerateUniqueString(16);
            int bookingHeaderId;
            _db.Connection.Open();
            _db.Transaction = _db.Connection.BeginTransaction();
            try {
                BookingHeader bookingHeader = new BookingHeader {
                    MemberID = 1, //Testing purpose
                    ReservedTime = DateTime.Now,
                    SessionID = sessionString,
                    Took = false,
                    Total = 0 //Should be 0 before check the reserved seats.
                };
                _db.BookingHeaders.InsertOnSubmit(bookingHeader);
                _db.SubmitChanges();
                bookingHeaderId = bookingHeader.HeaderID;
                _db.Transaction.Commit();
            } catch (Exception ex) {
                Console.Write(ex.StackTrace.ToString());
                _db.Transaction.Rollback();
                return 0;
            } finally {
                _db.Connection.Close();
            }
            return bookingHeaderId;
        }

        /// <summary>
        /// Check all the reserved seats.
        /// </summary>
        /// <param name="bookingHeader">The booking header that is created previously.</param>
        /// <returns>Return -1 if no seat or failed to check, total seat if success</returns>
        private int CheckSeats(int bookingHeaderId) {
            var seats = (List<int>)Session["ReservedSeats"];
            var totalSeat = seats.Count();
            _db.Connection.Open();
            _db.Transaction = _db.Connection.BeginTransaction();
            try {
                foreach (var seat in seats) {
                    var s = _db.Seat_ShowDetails.FirstOrDefault(x => x.SeatID == seat);
                    s.Reserved = true;
                    s.Paid = false;
                    s.BookingHeaderID = bookingHeaderId;
                    _db.SubmitChanges();
                }
                _db.Transaction.Commit();
            } catch (Exception ex) {
                Console.Write(ex.StackTrace.ToString());
                _db.Transaction.Rollback();
                return -1;
            } finally {
                _db.Connection.Close();
            }
            return totalSeat;
        }

        /// <summary>
        /// Update total seats in BookingHeader
        /// </summary>
        /// <param name="totalSeat">Total reserved seats</param>
        /// <param name="bookingHeaderId">ID of the BookingHeader that is needed to edit.</param>
        /// <returns>Return total seats if success, else return -1</returns>
        private int UpdateTotalSeat(int bookingHeaderId, int totalSeat) {
            _db.Connection.Open();
            _db.Transaction = _db.Connection.BeginTransaction();
            try {
                var bookingHeader = _db.BookingHeaders.FirstOrDefault(b => b.HeaderID == bookingHeaderId);
                bookingHeader.Total = totalSeat;
                _db.SubmitChanges();
                _db.Transaction.Commit();
            } catch (Exception e) {
                Console.Write(e.StackTrace.ToString());
                _db.Transaction.Rollback();
                return -1;
            } finally {
                _db.Connection.Close();
            }
            return totalSeat;
        }
    }
}