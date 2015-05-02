using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MovieTicketReservation.ViewModels;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.Models;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.BookingHeaderService;
using MovieTicketReservation.Services.MemberService;
using MovieTicketReservation.Services.ScheduleService;
using MovieTicketReservation.Services.SeatService;
using MovieTicketReservation.Services.SeatShowDetailsService;

namespace MovieTicketReservation.Controllers {
    public class TicketController : Controller {
        private DbEntities context = new DbEntities();
        private ISeatShowRepository seatShowRepository;
        private IBookingRepository bookingRepository;
        private IMemberRepository memberRepository;
        private IScheduleRepository scheduleRepository;
        private ISeatRepository seatRepository;

        public TicketController() {
            this.bookingRepository = new BookingHeaderRepository(context);
            this.memberRepository = new MemberRepository(context);
            this.scheduleRepository = new ScheduleRepository(context);
            this.seatRepository = new SeatRepository(context);
            this.seatShowRepository = new SeatShowRepository(context);
        }

        // GET: Ticket
        public ActionResult Index() {
            if (Session["UID"] == null) {
                Session["RedirectURL"] = Request.RawUrl;
                return Redirect("/User/Login");
            }
            var memberId = (int)Session["UID"];
            var member = memberRepository.GetMemberByID(memberId);
            var bookingHeaders = bookingRepository.GetBookingHeadersByMemberID(memberId);

            List<TicketModel> ticketModels = new List<TicketModel>();

            foreach (var item in bookingHeaders) {
                var seats = seatShowRepository.GetDetailsByBookingHeaderID(item.HeaderID).ToList();
                var schedule = seats[0].Schedule;
                var ticketModel = new TicketModel {
                    BookingHeaderId = item.HeaderID,
                    Cinema = new CinemaModel {
                        Name = schedule.Cine_MovieDetails.Cinema.Name,
                        CinemaId = schedule.Cine_MovieDetails.Cinema.CinemaID
                    },
                    MovieTitle = schedule.Cine_MovieDetails.Movie.Title,
                    ReservedDate = (DateTime)item.ReservedTime,
                    RoomName = schedule.Room.Name,
                    Seats = seats.Select(s => s.Seat.Name).ToList(),
                    ShowDate = (DateTime)schedule.Date,
                    ShowTime = (TimeSpan)schedule.ShowTime.StartTime,
                    ThumbnailUrl = schedule.Cine_MovieDetails.Movie.ThumbnailURL,
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
            var showingDate = scheduleRepository.GetScheduleByID(scheduleId).Date;
            if (showingDate < DateTime.Now) return View("/Shared/Error");
            Session["Schedule"] = scheduleId;
            Session["ReservedSeats"] = new List<int>();
            var seats = seatShowRepository.GetDetailsByScheduleID(scheduleId).Select(x => new SeatModel {
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
            var schedule = scheduleRepository.GetScheduleByID(scheduleId);
            var movie = schedule.Cine_MovieDetails.Movie;
            var seats = (List<int>)Session["ReservedSeats"];
            var showtime = (TimeSpan)schedule.ShowTime.StartTime;
            var details = new BookingDetailsModel {
                Cinema = schedule.Cine_MovieDetails.Cinema.Name,
                Room = schedule.Room.Name,
                MovieTitle = movie.Title,
                ReservedDate = DateTime.Now,
                ScheduleId = scheduleId,
                Seats = seatRepository.GetSeats().Where(dbs => seats.Contains(dbs.SeatID)).Select(x => new String(x.Name.ToCharArray())).ToList(),
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
            int deletedBookingHeaderId = bookingRepository.DeleteBookingHeader(bookingHeaderId);
            if (deletedBookingHeaderId != 0) {
                var seats = seatShowRepository.GetDetailsByBookingHeaderID(deletedBookingHeaderId);
                if (seats.Count() != 0) {
                    foreach (var seat in seats) {
                        seat.BookingHeaderID = null;
                        seat.Reserved = false;
                        seat.Paid = false;
                        seatShowRepository.UpdateSeat(seat);
                    }
                    return Json(new { Success = true, ErrorMessage = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false, ErrorMessage = "Booking header not found." }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        /// <summary>
        /// Create booking header as the header for reserved seats.
        /// </summary>  
        /// <returns>If the booking header is created, return the booking header ID, else return 0.</returns>
        private int CreateBookingHeader() {
            var schedule = Session["Schedule"];
            var sessionString = Helper.GenerateUniqueString(16);
            int bookingHeaderId;

            BookingHeader bookingHeader = new BookingHeader {
                MemberID = 1, //Testing purpose
                ReservedTime = DateTime.Now,
                SessionID = sessionString,
                Took = false,
                Total = 0 //Should be 0 before check the reserved seats.
            };
            bookingHeaderId = bookingRepository.InsertBookingHeader(bookingHeader);
            if (bookingHeaderId != 0) return bookingHeaderId;
            return 0;
        }

        /// <summary>
        /// Check all the reserved seats.
        /// </summary>
        /// <param name="bookingHeader">The booking header that is created previously.</param>
        /// <returns>Return -1 if no seat or failed to check, total seat if success</returns>
        private int CheckSeats(int bookingHeaderId) {
            var seats = (List<int>)Session["ReservedSeats"];
            var totalSeat = seats.Count();

            foreach (var seat in seats) {
                var seatShowDetails = seatShowRepository.GetDetailsBySeatID(seat);
                if (seatShowDetails == null) return -1;
                seatShowDetails.Reserved = true;
                seatShowDetails.Paid = false;
                seatShowDetails.BookingHeaderID = bookingHeaderId;
                seatShowRepository.UpdateSeat(seatShowDetails);
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
            var bookingHeader = bookingRepository.GetBookingHeaderByID(bookingHeaderId);
            if (bookingHeader != null) {
                bookingHeader.Total = totalSeat;
                bookingRepository.UpdateBookingHeader(bookingHeader);
                return totalSeat;
            }
            return -1;
        }
    }
}