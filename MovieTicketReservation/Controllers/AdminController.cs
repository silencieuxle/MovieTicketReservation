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
using MovieTicketReservation.Services.NewsRepository;
using MovieTicketReservation.Services.RoomService;
using MovieTicketReservation.Services.TagRepository;
using MovieTicketReservation.Services.SeatShowDetailsService;
using MovieTicketReservation.Services.TicketClassService;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.ViewModels;

namespace MovieTicketReservation.Controllers {
    public class AdminController : Controller {
        private const string imagePath = "/Content/Images/";

        private DbEntities context = new DbEntities();
        private IMovieRepository movieRepository;
        private IScheduleRepository scheduleRepository;
        private IRoomRepository roomRepository;
        private IMemberRepository memberRepository;
        private IShowtimeRepository showtimeRepository;
        private INewsRepository newsRepository;
        private ICinemaMovieRepository cinemaMovieRepository;
        private ISeatShowRepository seatShowRepository;
        private ITicketClassRepository ticketClassRepository;

        public AdminController() {
            this.movieRepository = new MovieRepository(context);
            this.scheduleRepository = new ScheduleRepository(context);
            this.memberRepository = new MemberRepository(context);
            this.showtimeRepository = new ShowtimeRepository(context);
            this.roomRepository = new RoomRepository(context);
            this.newsRepository = new NewsRepository(context);
            this.seatShowRepository = new SeatShowRepository(context);
            this.ticketClassRepository = new TicketClassRepository(context);
            this.cinemaMovieRepository = new CinemaMovieDetailsRepository(context);
        }

        // GET: Admin
        public ActionResult Index() {
            return View();
        }

        public ActionResult GetPage(string page, string param) {
            //if (Session["AdminSession"] == null) return PartialView("_Login");
            switch (page) {
                case "login":
                    Session.Abandon();
                    return PartialView("_Login");
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
                case "manageschedule_all":
                    return PartialView("_ManageSchedule_All");
                case "manageschedule_edit":
                    return PartialView("_ManageSchedule_Edit");
                case "manageschedule_add":
                    ViewBag.Movies = movieRepository.GetCanBeScheduledMovies();
                    ViewBag.Rooms = roomRepository.GetRoomsByCinemaID("CINE1");
                    ViewBag.Showtimes = showtimeRepository.GetShowtimes();
                    ViewBag.TicketClasses = ticketClassRepository.GetTicketClasses();
                    return PartialView("_ManageSchedule_Create");
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
                    return PartialView("_Promotion_Edit");
                case "managenews_all":
                    return PartialView("_ManageNews_All");
                case "managenews_add":
                    return PartialView("_ManageNews_Add");
                case "managenews_edit":
                    return PartialView("_ManageNews_Edit");
                default: return View("Index");
            }
        }

        [HttpGet]
        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel) {
            if (!ModelState.IsValid) return View(loginModel);
            if (memberRepository.Authenticate(loginModel.Email, Helper.GenerateSHA1String(loginModel.Password))) {

            };
            return null;
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

        public ActionResult AjaxGetAvailableSchedules() {
            var result = scheduleRepository.GetAvailableSchedules().Select(s => new {
                ScheduleID = s.ScheduleID,
                MovieTitle = s.Cine_MovieDetails.Movie.Title,
                Date = ((DateTime)s.Date).ToShortDateString(),
                Time = Helper.ToTimeString((TimeSpan)s.ShowTime.StartTime),
                Room = s.Room.Name
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetAvailableSchedulesByMovieID(int movieId) {
            var result = scheduleRepository.GetAvailableSchedules().Where(s => s.Cine_MovieDetails.Movie.MovieID == movieId).Select(s => new {
                ScheduleID = s.ScheduleID,
                MovieTitle = s.Cine_MovieDetails.Movie.Title,
                Date = ((DateTime)s.Date).ToShortDateString(),
                Time = Helper.ToTimeString((TimeSpan)s.ShowTime.StartTime),
                Room = s.Room.Name
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetDatesByMovieID(int movieId) {
            var details = cinemaMovieRepository.GetDetailsByCinemaIDAndMovieID("CINE1", movieId);
            var beginShowDate = (DateTime)details.BeginShowDate;
            var endShowDate = beginShowDate.AddDays((int)details.Duration);
            DateTime startDate;
            if (beginShowDate > DateTime.Now) startDate = beginShowDate;
            else startDate = DateTime.Now;

            List<String> result = new List<string>();
            for (var d = startDate; d <= endShowDate; d = d.AddDays(1)) {
                result.Add(d.ToShortDateString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxAddScheduleManually(int movieId, string roomId, string date, int showtimeId, string classId, int? promoteId) {
            DateTime showdate = DateTime.Parse(date);
            int cineMovieID = cinemaMovieRepository.GetDetailsByCinemaIDAndMovieID("CINE1", movieId).DetailsID;

            var duplicate = scheduleRepository.GetSchedules().Where(x => x.ShowTimeID == showtimeId && x.RoomID == roomId &&
                x.Cine_MovieDetails.MovieID == movieId && (DateTime)x.Date == showdate);
            if (duplicate.Count() != 0)
                return Json(new { Success = false, ErrorMessage = "Suất chiếu cần thêm đã có sẵn" }, JsonRequestBehavior.AllowGet);

            var result = scheduleRepository.InsertSchedule(new Schedule {
                Cine_MovieDetailsID = cineMovieID,
                Date = showdate,
                RoomID = roomId,
                ShowTimeID = showtimeId,
                PromoteID = promoteId
            });

            var movie = movieRepository.GetMovieByID(movieId).MovieEditions;

            if (result != 0) {
                CreateSeatShowDetails(result, roomId, classId);
            }
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxAddScheduleAuto(int movieId, string roomId, string date, int firstShowtimeId, int numberOfSchedule, string classId, int? promoteId) {
            var showtime = (TimeSpan)showtimeRepository.GetShowtimeByID(firstShowtimeId).StartTime;
            int startTime = showtime.Hours * 60 + showtime.Minutes;
            int movielength = (int)movieRepository.GetMovieByID(movieId).MovieLength;

            int numberOfScheduleToCreate = (22 * 60 + 30 - startTime) / movielength;
            numberOfScheduleToCreate = numberOfScheduleToCreate > numberOfSchedule ? numberOfSchedule : numberOfScheduleToCreate;
            int result = 0;
            int stId = firstShowtimeId;
            for (int i = 1; i <= numberOfScheduleToCreate; i++) {
                DateTime showdate = DateTime.Parse(date);
                int cineMovieID = cinemaMovieRepository.GetDetailsByCinemaIDAndMovieID("CINE1", movieId).DetailsID;

                result = scheduleRepository.InsertSchedule(new Schedule {
                    Cine_MovieDetailsID = cineMovieID,
                    Date = showdate,
                    RoomID = roomId,
                    ShowTimeID = stId,
                    PromoteID = promoteId
                });

                if (result == 0) return Json(new { Success = false, ErrorMessage = "Some schedules can't be created." });
                CreateSeatShowDetails(result, roomId, classId);
                stId += movielength / 15 + 1;
            }

            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        private bool CreateSeatShowDetails(int scheduleId, string roomId, string classId) {
            return seatShowRepository.InsertSeatsWithDetails(scheduleId, roomId, classId);
        }

        #endregion

        #region News Management

        public ActionResult AddNews(PostUploadModel postUploadModel) {
            if (!ModelState.IsValid) return View(postUploadModel);
            var title = postUploadModel.Title;
            var description = postUploadModel.Description;
            var content = postUploadModel.Content;
            var file = postUploadModel.ThumbnailURL;
            string thumbnailUrl = imagePath + file.FileName + file.ContentType.Split('/')[1];
            if (file.ContentLength > 0) {
                file.SaveAs(thumbnailUrl);
            }
            newsRepository.InsertNews(new News {
                Title = title,
                Description = description,
                Content = content,
                ThumbnailURL = thumbnailUrl,
                Tags = new List<Tag> {

                }
            });
            return null;
        }

        #endregion
    }
}