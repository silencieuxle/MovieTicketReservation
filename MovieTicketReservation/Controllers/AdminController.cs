using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.AdminAccountService;
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
using MovieTicketReservation.Services.BookingHeaderService;
using MovieTicketReservation.Services.AgeLimitationService;
using MovieTicketReservation.Services.EditionService;
using MovieTicketReservation.Services.GenreService;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.ViewModels;

namespace MovieTicketReservation.Controllers {
    public class AdminController : Controller {
        private const string imagePath = "/Content/Images/";
        private DbEntities context = new DbEntities();

        private IEditionRepository editionRepository;
        private IGenreRepository genreRepository;
        private ITagRepository tagRepository;
        private IMovieRepository movieRepository;
        private IScheduleRepository scheduleRepository;
        private IRoomRepository roomRepository;
        private IMemberRepository memberRepository;
        private IShowtimeRepository showtimeRepository;
        private INewsRepository newsRepository;
        private ICinemaMovieRepository cinemaMovieRepository;
        private ISeatShowRepository seatShowRepository;
        private IAdminAccountRepository adminAccountRepository;
        private ITicketClassRepository ticketClassRepository;
        private IBookingRepository bookingRepository;
        private IAgeLimitationRepository ageLimitationRepository;

        public AdminController() {
            this.genreRepository = new GenreRepository(context);
            this.editionRepository = new EditionRepository(context);
            this.bookingRepository = new BookingHeaderRepository(context);
            this.tagRepository = new TagRepository(context);
            this.movieRepository = new MovieRepository(context);
            this.scheduleRepository = new ScheduleRepository(context);
            this.memberRepository = new MemberRepository(context);
            this.showtimeRepository = new ShowtimeRepository(context);
            this.roomRepository = new RoomRepository(context);
            this.newsRepository = new NewsRepository(context);
            this.seatShowRepository = new SeatShowRepository(context);
            this.ticketClassRepository = new TicketClassRepository(context);
            this.adminAccountRepository = new AdminAccountRepository(context);
            this.cinemaMovieRepository = new CinemaMovieDetailsRepository(context);
            this.ageLimitationRepository = new AgeLimitationRepository(context);
        }

        // GET: Admin
        public ActionResult Index() {
            if (Session["Role"] == null)
                return Redirect("Login");
            ViewBag.TotalMembers = memberRepository.GetAllMembers().Count();
            ViewBag.TotalBookings = bookingRepository.GetBookingHeadersByDate(DateTime.Now).Count();
            ViewBag.TotalMovies = movieRepository.GetAllMovies().Count();
            return View();
        }

        public ActionResult GetPage(string page, string param) {
            int role = (int)Session["Role"];
            if (role != 1 && role != 2) return Redirect("Login");
            switch (page) {
                case "login":
                    Session.Abandon();
                    return Redirect("Login");
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
                    ViewBag.Movies = cinemaMovieRepository.GetCanBeScheduledMoviesByCinema("CINE1");
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

        private bool IsAdmin() {
            if (Session["Role"] == null) return false;
            if ((int)Session["Role"] == 1 || (int)Session["Role"] == 2) return true;
            return false;
        }

        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel) {
            if (!ModelState.IsValid) return View(loginModel);
            var user = memberRepository.GetMemberByEmailAndPassword(loginModel.Email, Helper.GenerateSHA1String(loginModel.Password));
            if (user == null) {
                ModelState.AddModelError("", "Sai thông tin đăng nhập");
                return View(loginModel);
            }
            var admin = adminAccountRepository.GetAdminAccountByMemberID(user.MemberID);
            if (admin == null) {
                ModelState.AddModelError("", "Tài khoản không có quyền hạn truy cập vào khu vực này!");
                return View(loginModel);
            }
            Session["UID"] = user.MemberID;
            Session["Role"] = admin.RoleID;
            Session["AdminSection"] = admin.Section;
            if (Session["RedirectURL"] != null) return Redirect((string)Session["RedirectURL"]);
            return Redirect("/Admin/Index");
        }

        public ActionResult Logout() {
            Session.Abandon();
            return Redirect("/Home/");
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
            if (IsAdmin()) {
                var result = scheduleRepository.GetAvailableSchedules().Select(s => new {
                    ScheduleID = s.ScheduleID,
                    MovieTitle = s.Cine_MovieDetails.Movie.Title,
                    Date = ((DateTime)s.Date).ToShortDateString(),
                    Time = Helper.ToTimeString((TimeSpan)s.ShowTime.StartTime),
                    Room = s.Room.Name
                }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            } else {
                return null;
            }
        }

        public ActionResult AjaxGetAvailableSchedulesByMovieID(int movieId) {
            if (IsAdmin()) {
                var result = scheduleRepository.GetAvailableSchedules().Where(s => s.Cine_MovieDetails.Movie.MovieID == movieId).Select(s => new {
                    ScheduleID = s.ScheduleID,
                    MovieTitle = s.Cine_MovieDetails.Movie.Title,
                    Date = ((DateTime)s.Date).ToShortDateString(),
                    Time = Helper.ToTimeString((TimeSpan)s.ShowTime.StartTime),
                    Room = s.Room.Name
                }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            } else {
                return null;
            }
        }

        public ActionResult AjaxGetDatesByMovieID(int movieId) {
            if (IsAdmin()) {
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
            } else {
                return null;
            }
        }

        public ActionResult AjaxAddScheduleManually(int movieId, string roomId, string date, int showtimeId, string ticketClassId, int? promoteId) {
            if (IsAdmin()) {
                DateTime showdate = DateTime.Parse(date);
                int cineMovieID = cinemaMovieRepository.GetDetailsByCinemaIDAndMovieID("CINE1", movieId).DetailsID;

                var duplicate = scheduleRepository.GetSchedules().Where(x => x.ShowTimeID == showtimeId && x.RoomID == roomId && (DateTime)x.Date == showdate);
                if (duplicate.Count() != 0)
                    return Json(new { Success = false, ErrorMessage = "Suất cần thêm đã có." }, JsonRequestBehavior.AllowGet);

                var result = scheduleRepository.InsertSchedule(new Schedule {
                    Cine_MovieDetailsID = cineMovieID,
                    Date = showdate,
                    RoomID = roomId,
                    ShowTimeID = showtimeId,
                    PromoteID = promoteId
                });

                if (result != 0) {
                    CreateSeatShowDetails(result, roomId, ticketClassId);
                }
                return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
            } else {
                return null;
            }
        }

        public ActionResult AjaxAddScheduleAuto(int movieId, string roomId, string date, int firstShowtimeId, int numberOfSchedule, string ticketClassId, int? promoteId) {
            if (IsAdmin()) {
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
                    CreateSeatShowDetails(result, roomId, ticketClassId);
                    stId += movielength / 15 + 1;
                }
                return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
            } else {
                return null;
            }
        }

        private bool CreateSeatShowDetails(int scheduleId, string roomId, string classId) {
            return seatShowRepository.InsertSeatsWithDetails(scheduleId, roomId, classId);
        }

        #endregion

        #region News Management

        public ActionResult ManageNews_Add(PostUploadModel postUploadModel) {
            if (!IsAdmin()) {
                Session["RedirectURL"] = "/Admin/ManageNews_Add";
                return Redirect("/Admin/Login");
            }
            if (!ModelState.IsValid) return View(postUploadModel);
            var title = postUploadModel.Title;
            var description = postUploadModel.Description;
            var content = postUploadModel.Content;
            var file = postUploadModel.ThumbnailURL;
            string thumbnailUrl = imagePath + file.FileName + file.ContentType.Split('/')[1];
            if (file.ContentLength > 0) {
                file.SaveAs(thumbnailUrl);
            }
            var tags = postUploadModel.Tags.Split(' ');
            List<Tag> tagList = new List<Tag>();
            foreach (var item in tags) {
                Tag tag = new Tag {
                    Name = item
                };
                int res = tagRepository.InsertIfNotExist(tag);
                if (res != 0) tagList.Add(tagRepository.GetTagByID(res));
            }
            newsRepository.InsertNews(new News {
                Title = title,
                Description = description,
                Content = content,
                ThumbnailURL = thumbnailUrl,
                Tags = tagList
            });
            return null;
        }

        #endregion

        #region Manage Movie

        [HttpGet]
        public ActionResult ManageMovie_Add() {
            if (!IsAdmin()) {
                Session["RedirectURL"] = "/Admin/ManageMovie_Add";
                return Redirect("/Admin/Login");
            }
            ViewBag.Ages = ageLimitationRepository.GetAgeLimitations();
            ViewBag.Editions = editionRepository.GetMovieEditions();
            ViewBag.Genres = genreRepository.GetMovieGenres();
            return View();
        }

        [HttpPost]
        public ActionResult ManageMovie_Add(MovieAddModel movieAddModel) {
            if (!IsAdmin()) {
                Session["RedirectURL"] = "/Admin/ManageMovie_Add";
                return Redirect("/Admin/Login");
            }
            ViewBag.Ages = ageLimitationRepository.GetAgeLimitations();
            ViewBag.Editions = editionRepository.GetMovieEditions();
            ViewBag.Genres = genreRepository.GetMovieGenres();

            if (!ModelState.IsValid) {
                return View(movieAddModel);
            }

            if (movieAddModel.BeginShowDate.Date < movieAddModel.ReleasedDate.Date) {
                ModelState.AddModelError("BeginShowDate", "Ngày chiếu không thể lớn hơn ngày ra mắt");
                return View(movieAddModel);
            }

            if (movieAddModel.BeginShowDate.Date > movieAddModel.ReleasedDate.Date.AddDays(movieAddModel.Duration)) {
                ModelState.AddModelError("BeginShowDate", "Ngày chiếu không thể vượt qua thời gian chiếu cho phép.");
                return View(movieAddModel);
            }

            // Get uploaded HttpPostedFileBase
            var thumbnail = movieAddModel.Thumbnail;
            var wideThumbnail = movieAddModel.WideThumbnail;

            // Get uploaded file path
            string thumbnailUrl = imagePath + thumbnail.FileName + thumbnail.ContentType.Split('/')[1];
            string wideThumbnailUrl = imagePath + wideThumbnail.FileName + wideThumbnail.ContentType.Split('/')[1];

            // Save uploaded poster and cover image
            thumbnail.SaveAs(thumbnailUrl);
            wideThumbnail.SaveAs(wideThumbnailUrl);

            // Get genre ids
            List<string> genreIds = movieAddModel.Genres;

            var result = movieRepository.InsertMovie(new Movie {
                Actors = movieAddModel.Actors,
                AgeLimitationID = movieAddModel.AgeLimitation,
                Available = movieAddModel.Available,
                BeginShowDate = movieAddModel.BeginShowDate,
                Description = movieAddModel.ShortDescription,
                Director = movieAddModel.Director,
                Duration = movieAddModel.Duration,
                HotMovie = movieAddModel.HotMovie,
                LongDescription = movieAddModel.LongDescription,
                MovieLength = movieAddModel.MovieLength,
                Rate = movieAddModel.Rate,
                ReleasedDate = movieAddModel.ReleasedDate,
                ThumbnailURL = thumbnailUrl,
                WideThumbnail = wideThumbnailUrl,
                TrailerURL = movieAddModel.TrailerURL,
                Title = movieAddModel.Title,
            });

            //// If succeeded add movie, add movie genres
            //if (result) {

            //}

            return Redirect("/Admin/ManageMovie_Add");
        }

        #endregion
    }
}