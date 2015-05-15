using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.MovieService;
using MovieTicketReservation.Services.MemberService;
using MovieTicketReservation.Services.ScheduleService;
using MovieTicketReservation.Services.ShowtimeService;
using MovieTicketReservation.Services.CinemaMovieDetailsService;
using MovieTicketReservation.Services.RoomService;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.ViewModels;

namespace MovieTicketReservation.Controllers {
    public class AdminController : Controller {
        private DbEntities context = new DbEntities();
        private IMovieRepository movieRepository;
        private IScheduleRepository scheduleRepository;
        private IRoomRepository roomRepository;
        private IMemberRepository memberRepository;
        private IShowtimeRepository showtimeRepository;
        private ICinemaMovieRepository cinemaMovieRepository;

        public AdminController() {
            this.movieRepository = new MovieRepository(context);
            this.scheduleRepository = new ScheduleRepository(context);
            this.memberRepository = new MemberRepository(context);
            this.showtimeRepository = new ShowtimeRepository(context);
            this.roomRepository = new RoomRepository(context);
            this.cinemaMovieRepository = new CinemaMovieDetailsRepository(context);
        }

        // GET: Admin
        public ActionResult Index() {
            return View();
        }

        public ActionResult GetPage(string page, string param) {
            switch (page) {
                case "moviestats_overall":
                    return PartialView("_MovieStats_Overall");
                case "moviestats_view":
                    return PartialView("_MovieStats_View");
                case "moviestats_ticket":
                    return PartialView("_MovieStats_Ticket");
                case "ticketstats_overall":
                    return PartialView("_TicketStats_Overall");
                case "ticketstats_movie":
                    return PartialView("_TicketStats_Movie", movieRepository.GetAllMovies());
                case "ticketstats_showtime":
                    return PartialView("_TicketStats_Showtime", showtimeRepository.GetShowtimes());
                case "ticketstats_room":
                    return PartialView("_TicketStats_Room");
                case "systemstats":
                    return PartialView("_SystemStats");
                case "managemovie_all":
                    return PartialView("_ManageMovie_All");
                case "manageschedule_create":
                    ViewBag.Rooms = roomRepository.GetRoomsByCinemaID("CINE1");
                    ViewBag.Showtimes = showtimeRepository.GetShowtimes();
                    return PartialView("_ManageSchedule_Create", movieRepository.GetMoviesByCinemaID("CINE1"));
                case "manageschedule_all":
                    return PartialView("_ManageSchedule_All");
                case "manageschedule_edit":
                    return PartialView("_ManageSchedule_Edit");
                case "mamangemember_all":
                    return PartialView("_ManageMember_All");
                case "managemember_add":
                    return PartialView("_ManageMember_Add");
                case "managemember_edit":
                    return PartialView("_ManageMember_Edit", memberRepository.GetMemberByID(Convert.ToInt32(param)));
                case "promotion_all":
                    return PartialView("_Promotion_All");
                case "promotion_add":
                    return PartialView("_Promotion_Add");
                case "promotion_edit":
                    return PartialView("_Promotion_Edit"); ;
                default: return View("Index");
            }
        }

        #region Member Management

        public ActionResult AjaxGetMembers(int headIndex, int type) {
            var result = memberRepository.GetAllMembers();
            int index = 0;
            switch (type) {
                case 0:
                    result = result.Skip(headIndex - 10).Take(10).ToList();
                    index = headIndex - 10;
                    break;
                case 1:
                    result = result.Skip(headIndex + 10).Take(10).ToList();
                    index = headIndex + 10;
                    break;
                default:
                    result = result.Take(10).ToList();
                    index = headIndex;
                    break;
            }
            return Json(new {
                Data = result.Select(r => new { MemberID = r.MemberID, Email = r.Email, Fullname = r.Lastname + " " + r.Firstname, IDCardNumber = r.IDCardNumber, Phone = r.Phone }),
                HeadIndex = index
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxDeleteMember(int memberId) {
            var result = memberRepository.DeleteMember(memberId);
            return Json(new { Success = result, ErrorMessage = "Cannot delete member" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxUpdateMember(Member member) {
            if (ModelState.IsValid) {
                if (memberRepository.UpdateMember(member))
                    return Json(new { Success = true, ErrorMessage = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, ErrorMessage = "Cannot update member with provided member ID" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxAddMember(UserRegisterModel userRegisterModel) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values.SelectMany(e => e.ToString() + "\n").ToList();
                return Json(new { Success = false, ErrorList = errors });
            }
            var userDuplicated = memberRepository.IsIdCardExisted(userRegisterModel.IdCardNumber);
            if (userDuplicated) {
                var error = "Số chứng minh nhân dân đã được sử dụng \n";
                return Json(new { Success = false, ErrorList = error });
            }
            var result = memberRepository.InsertMember(new Member {
                Address = userRegisterModel.Address,
                AvatarURL = null,
                Birthday = userRegisterModel.BirthDay,
                Email = userRegisterModel.Email,
                Firstname = userRegisterModel.FirstName,
                Gender = userRegisterModel.Gender,
                IDCardNumber = userRegisterModel.IdCardNumber,
                Lastname = userRegisterModel.LastName,
                Password = Helper.GenerateSHA1String(userRegisterModel.Password),
                Phone = userRegisterModel.Phone,
            });
            return Json(new { Success = result, ErrorMessage = result ? "" : "Error occured while process your request." }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Schedule Management

        public ActionResult GetDatesByMovieID(int movieId) {
            var beginDate = DateTime.Now;
            var details = cinemaMovieRepository.GetDetailsByCinemaIDAndMovieID("CINE1", movieId);
            var endDate = ((DateTime)details.BeginShowDate).AddDays((int)details.Duration);
            if (beginDate >= endDate) return null;
            List<String> result = new List<string>();
            for (var d = beginDate; d <= endDate; d = d.AddDays(1)) {
                result.Add(d.ToShortDateString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}