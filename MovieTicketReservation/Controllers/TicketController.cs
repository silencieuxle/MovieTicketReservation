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

        private bool IsLoggedIn() {
            if (Session["UID"] == null) return false;
            return true;
        }

        // GET: Ticket
        public ActionResult Index() {
            if (!IsLoggedIn()) {
                Session["RedirectURL"] = Request.RawUrl;
                return Redirect("/User/Login");
            }
            var memberId = (int)Session["UID"];
            var member = memberRepository.GetMemberByID(memberId);
            var bookingHeaders = bookingRepository.GetBookingHeadersByMemberID(memberId);

            List<TicketModel> ticketModels = new List<TicketModel>();

            foreach (var item in bookingHeaders) {
                var seats = seatShowRepository.GetDetailsByBookingHeaderID(item.HeaderID).ToList();
                decimal total = 0;
                foreach (var seat in seats) {
                    total += (decimal)seat.TicketClass.Price;
                }
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
                    IsTaken = (bool)item.Took,
                    Total = total
                };
                ticketModels.Add(ticketModel);
            }
            return View(ticketModels);
        }

        public ActionResult OutOfService() {
            return View();
        }

        public ActionResult Reserve(int scheduleId) {
            if (!IsLoggedIn()) {
                Session["RedirectURL"] = Request.RawUrl;
                return Redirect("/User/Login");
            }
            var showingDate = scheduleRepository.GetScheduleByID(scheduleId).Date;
            if (((DateTime)showingDate).Date < DateTime.Now.Date) return Redirect("/Movies");
            Session["Schedule"] = scheduleId;
            Session["ReservedSeats"] = new List<int>();

            var seats = seatShowRepository.GetDetailsByScheduleID(scheduleId).ToList();
            bool canReserve = seats.Any(s => s.Reserved == false);
            if (canReserve) return View(seats);
            else {
                ViewBag.ReturnURL = Request.RawUrl;
                ViewBag.ReturnMessage = "Nhấn vào để chọn suất chiếu khác.";
                ViewBag.Message = "Suất chiếu này đã hết ghế.";

                return View("OutOfService");
            }
        }

        [HttpGet]
        public ActionResult Confirm() {
            if (!IsLoggedIn()) return Redirect("/Home/");
            if (Session["Schedule"] == null) return Redirect("/Home/");

            var seats = (List<int>)Session["ReservedSeats"];
            var scheduleId = (int)Session["Schedule"];

            // Check if any seat is reserve while current user is checking seats or there is no seat is created for this schedule
            bool result = CheckSeatsForAvailability();

            decimal total = 0;
            foreach (var seat in seats) {
                total += (decimal)seatShowRepository.GetDetailsBySeatID(seat).TicketClass.Price;
            }

            ViewBag.ReturnURL = "/Ticket/Reserve?ScheduleID=" + scheduleId;
            ViewBag.ReturnMessage = "Nhấn vào để chọn ghế khác";
            ViewBag.Message = "Ghế bạn muốn đặt đã được đặt trong thời gian bạn chọn ghế.";

            if (!result) return View("OutOfService");

            var schedule = scheduleRepository.GetScheduleByID(scheduleId);
            var movie = schedule.Cine_MovieDetails.Movie;
            var showtime = (TimeSpan)schedule.ShowTime.StartTime;
            var reservedSeats = seatRepository.GetSeats().Where(dbs => seats.Contains(dbs.SeatID)).Select(x => new String(x.Name.ToCharArray())).ToList();
            var details = new BookingDetailsModel {
                Cinema = schedule.Cine_MovieDetails.Cinema.Name,
                Room = schedule.Room.Name,
                MovieTitle = movie.Title,
                ReservedDate = DateTime.Now,
                ScheduleId = scheduleId,
                Seats = reservedSeats,
                Showtime = showtime,
                Total = 0
            };
            return View(details);
        }

        [HttpGet]
        public ActionResult CheckOut() {
            if (!IsLoggedIn()) return Redirect("/Home/");
            if (Session["Schedule"] == null) return Redirect("/Home/");
            int bookingHeaderId;
            bool error = false;
            if ((bookingHeaderId = CreateBookingHeader()) == 0) {
                error = true;
            } else {
                int totalSeat = CheckSeats(bookingHeaderId);
                if (totalSeat != -1) {
                    if (totalSeat == UpdateTotalSeat(bookingHeaderId, totalSeat))
                        error = false;
                } else {
                    error = true;
                }
            }

            if (error) {
                return View("Error", "Yêu cầu của bạn không thể xử lý, vui lòng thử lại sau.");
            } else {
                Session["Schedule"] = null;
                Session["ReservedSeats"] = null;
                return View();
            }
        }

        public ActionResult CancelConfirmation() {
            if (!IsLoggedIn()) return Redirect("/Home/");
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
            var bookingHeader = bookingRepository.GetBookingHeaderByID(bookingHeaderId);
            if (bookingHeader != null) {
                var seats = seatShowRepository.GetDetailsByBookingHeaderID(bookingHeaderId);
                if (seats.Count() != 0) {
                    foreach (var seat in seats) {
                        seat.BookingHeaderID = null;
                        seat.Reserved = false;
                        seat.Paid = false;
                        seatShowRepository.UpdateSeat(seat);
                    }
                    bookingRepository.DeleteBookingHeader(bookingHeaderId);
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
                MemberID = (int)Session["UID"],
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
                if ((bool)seatShowDetails.Reserved) return -2;
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
                if (bookingRepository.UpdateBookingHeader(bookingHeader) != 0) return totalSeat;
            }
            return -1;
        }

        private bool CheckSeatsForAvailability() {
            var seats = (List<int>)Session["ReservedSeats"];
            foreach (var seat in seats) {
                var currentSeat = seatShowRepository.GetDetailsBySeatID(seat);
                if (currentSeat.Reserved == true || currentSeat == null) return false;
            }
            return true;
        }
    }
}