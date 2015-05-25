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
using MovieTicketReservation.Services.SubtitleService;
using MovieTicketReservation.Services.CinemaService;
using MovieTicketReservation.Services.PromotionService;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.ViewModels;

namespace MovieTicketReservation.Controllers {
    public class AdminController : Controller {
        private DbEntities context = new DbEntities();

        private IPromotionService promotionRepository;
        private ICinemaRepository cinemaRepository;
        private ISubtitleRepository subtitleRepository;
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
            this.promotionRepository = new PromotionRepository(context);
            this.cinemaRepository = new CinemaRepository(context);
            this.subtitleRepository = new SubtitleRepository(context);
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
            if (!IsCompany() && !IsCinema()) return View("Login");
            ViewBag.TotalMembers = memberRepository.GetAllMembers().Count();
            ViewBag.TotalBookings = bookingRepository.GetBookingHeadersByDate(DateTime.Now).Count();
            ViewBag.TotalMovies = movieRepository.GetAllMovies().Count();
            return View();
        }

        public ActionResult GetPage(string page, string param) {
            if (!IsCinema() && !IsCompany()) return View("Login");
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
                    string cinemaId = (string)Session["AdminSection"];
                    var movies = movieRepository.GetCanBeScheduledMovies().Join(cinemaMovieRepository.GetDetailsByCinemaID((string)Session["AdminSection"]), m => m.MovieID, d => d.MovieID, (m, d) => new { Movie = m }).Select(x => x.Movie).ToList();
                    ViewBag.Movies = movies;
                    ViewBag.Rooms = roomRepository.GetRoomsByCinemaID(cinemaId);
                    ViewBag.Showtimes = showtimeRepository.GetShowtimes();
                    ViewBag.TicketClasses = ticketClassRepository.GetTicketClasses();
                    ViewBag.Promotions = promotionRepository.GetPromotionsByCinema(cinemaId);
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

        private bool IsCinema() {
            if (Session["Role"] == null) return false;
            if ((int)Session["Role"] == 2) return true;
            return false;
        }

        private bool IsCompany() {
            if (Session["Role"] == null) return false;
            if ((int)Session["Role"] == 1) return true;
            return false;
        }

        [HttpGet]
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

        [HttpGet]
        public ActionResult Logout() {
            Session.Abandon();
            return Redirect("/Home/");
        }

        #region Member Management
        [HttpPost]
        public ActionResult AjaxUpdateAvatar(HttpPostedFileBase avatar) {
            string imagePath = "/Content/Images/MemberImages/";
            // Get uploaded file path
            string avatarUrl = imagePath + (string)Session["UID"] + "-" + avatar.FileName;

            // Save uploaded poster and cover image
            if (avatar.ContentLength != 0) avatar.SaveAs(Server.MapPath(Url.Content(avatarUrl)));
            else {
                return Json(new { Success = false, ErrorMessage = "Lỗi khi nhận file" }, JsonRequestBehavior.AllowGet);
            }
            var member = memberRepository.GetMemberByID((int)Session["UID"]);
            member.AvatarURL = avatarUrl;
            var result = memberRepository.UpdateMember(member);
            if (!result) return Json(new { Success = false, ErrorMessage = "Xảy ra lỗi khi cập nhật thông tin." }, JsonRequestBehavior.AllowGet);
            return Json(new { Success = true, AvatarURL = avatarUrl }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
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

        [HttpPost]
        public ActionResult AjaxGetAvailableSchedules() {
            if (IsCinema()) {
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

        [HttpPost]
        public ActionResult AjaxGetAvailableSchedulesByMovieID(int movieId) {
            if (IsCinema()) {
                var result = scheduleRepository.GetAvailableSchedules().Where(s => s.Cine_MovieDetails.Movie.MovieID == movieId && s.Cine_MovieDetails.CinemaID == (string)Session["AdminSection"]).Select(s => new {
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

        [HttpPost]
        public ActionResult AjaxGetDatesByMovieID(int movieId) {
            if (IsCinema()) {
                var movie = movieRepository.GetMovieByID(movieId);
                var beginShowDate = (DateTime)movie.BeginShowDate;
                var endShowDate = beginShowDate.AddDays((int)movie.Duration);
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

        [HttpPost]
        public ActionResult AjaxAddScheduleManually(int movieId, string roomId, string date, int showtimeId, string ticketClassId, int? promoteId) {
            if (IsCinema()) {
                DateTime showdate = DateTime.Parse(date);
                int cineMovieID = cinemaMovieRepository.GetDetailsByCinemaIDAndMovieID((string)Session["AdminSection"], movieId).DetailsID;

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

        [HttpPost]
        public ActionResult AjaxAddScheduleAuto(int movieId, string roomId, string date, int firstShowtimeId, int numberOfSchedule, string ticketClassId, int? promoteId) {
            if (IsCinema()) {
                var showtime = (TimeSpan)showtimeRepository.GetShowtimeByID(firstShowtimeId).StartTime;
                int startTime = showtime.Hours * 60 + showtime.Minutes;
                int movielength = (int)movieRepository.GetMovieByID(movieId).MovieLength;

                int numberOfScheduleToCreate = (22 * 60 + 30 - startTime) / movielength;
                numberOfScheduleToCreate = numberOfScheduleToCreate > numberOfSchedule ? numberOfSchedule : numberOfScheduleToCreate;

                int result = 0;
                int stId = firstShowtimeId;
                for (int i = 1; i <= numberOfScheduleToCreate; i++) {
                    DateTime showdate = DateTime.Parse(date);
                    int cineMovieID = cinemaMovieRepository.GetDetailsByCinemaIDAndMovieID((string)Session["AdminSection"], movieId).DetailsID;
                    var duplicate = scheduleRepository.GetSchedules()
                        .Where(x => x.ShowTimeID == stId && x.RoomID == roomId && (DateTime)x.Date == showdate);
                    if (duplicate.Count() != 0)
                        return Json(new { Success = false, ErrorMessage = "Suất cần thêm đã có." }, JsonRequestBehavior.AllowGet);
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

        public ActionResult ManageNews_Add(NewsModel newsModel) {
            string imagePath = "/Content/Images/NewsImages/";
            bool result = false;

            if (!IsCinema() || !IsCompany()) {
                Session["RedirectURL"] = "/Admin/ManageNews_Add";
                return Redirect("/Admin/Login");
            }
            if (!ModelState.IsValid) return View(newsModel);

            // Get uploaded HttpPostedFileBase
            var thumbnail = newsModel.Thumbnail;

            // Get uploaded file path
            string thumbnailUrl = imagePath + thumbnail.FileName;

            // Save uploaded poster and cover image
            thumbnail.SaveAs(Server.MapPath(Url.Content(thumbnailUrl)));

            var title = newsModel.Title;
            var description = newsModel.Description;
            var content = newsModel.Content;

            var tags = newsModel.Tags.Split(' ').ToList();
            var news = new News {
                Title = title,
                Description = description,
                Content = content,
                ThumbnailURL = thumbnailUrl,
                PostedDate = DateTime.Now.Date,
                ViewCount = 0
            };
            result = newsRepository.InsertNews(news);
            if (result && tags.Count() != 0) {
                result = tagRepository.InsertTagForNews(tags, news.NewsID);
            }
            if (result)
                ViewBag.Message = "Thêm tin thành công";
            else
                ViewBag.Message = "Xảy ra lỗi khi thêm tin mới";
            return View("ManageNews_Add");
        }

        #endregion

        #region Manage Movie

        [HttpGet]
        public ActionResult ManageMovie_Add() {
            if (!IsCompany()) {
                Session["RedirectURL"] = "/Admin/ManageMovie_Add";
                return Redirect("/Admin/Login");
            }
            InitializePage();
            return View();
        }

        private void InitializePage() {
            ViewBag.Ages = ageLimitationRepository.GetAgeLimitations();
            ViewBag.Editions = editionRepository.GetMovieEditions();
            ViewBag.Subtitles = subtitleRepository.GetSubtitles();
            ViewBag.Genres = genreRepository.GetMovieGenres();
            ViewBag.Cinemas = cinemaRepository.GetCinemas();
        }

        [HttpPost]
        public ActionResult ManageMovie_Add(MovieAddModel movieAddModel) {
            InitializePage();
            string imagePath = "/Content/Images/MovieImages/";

            if (!IsCompany()) {
                Session["RedirectURL"] = "/Admin/ManageMovie_Add";
                return Redirect("/Admin/Login");
            }

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
            string thumbnailUrl = imagePath + thumbnail.FileName;
            string wideThumbnailUrl = imagePath + wideThumbnail.FileName;

            // Save uploaded poster and cover image
            thumbnail.SaveAs(Server.MapPath(Url.Content(thumbnailUrl)));
            wideThumbnail.SaveAs(Server.MapPath(Url.Content(wideThumbnailUrl)));
            bool result = false;

            var movie = new Movie {
                Actors = movieAddModel.Actors,
                AgeLimitationID = movieAddModel.AgeLimitation,
                Available = movieAddModel.Available,
                BeginShowDate = movieAddModel.BeginShowDate,
                Description = movieAddModel.ShortDescription,
                Director = movieAddModel.Director,
                Duration = movieAddModel.Duration,
                EditionID = movieAddModel.Edition,
                SubtitleID = movieAddModel.Subtitle,
                HotMovie = movieAddModel.HotMovie,
                LongDescription = movieAddModel.LongDescription,
                MovieLength = movieAddModel.MovieLength,
                Rate = movieAddModel.Rate,
                ReleasedDate = movieAddModel.ReleasedDate,
                ThumbnailURL = thumbnailUrl,
                WideThumbnail = wideThumbnailUrl,
                TrailerURL = movieAddModel.TrailerURL,
                Title = movieAddModel.Title,
            };

            result = movieRepository.InsertMovie(movie);
            // If succeeded add movie, add movie genres
            if (result) {
                List<MovieGenre> genres = new List<MovieGenre>();
                foreach (var item in movieAddModel.Genres) {
                    var genre = genreRepository.GetMovieGenreByID(item);
                    genres.Add(genre);
                }
                result = genreRepository.InsertMovieGenreForMovie(movie.MovieID, genres);
            }

            if (result) {
                List<Cine_MovieDetails> details = new List<Cine_MovieDetails>();
                foreach (var item in movieAddModel.Cinemas) {
                    result = cinemaMovieRepository.InsertDetails(new Cine_MovieDetails {
                        CinemaID = item,
                        MovieID = movie.MovieID
                    });
                    if (result) continue; else break;
                }
            }

            if (result) {
                ViewBag.Message = "Thêm film mới thành công";
            } else {
                ViewBag.Message = "Xảy ra lỗi khi thêm film";
            }
            return View();
        }

        #endregion

        #region Manage Promote

        [HttpGet]
        public ActionResult ManagePromotion_Add() {
            return View();
        }

        [HttpPost]
        public ActionResult ManagePromotion_Add(PromotionModel promotionModel) {
            string imagePath = "/Content/Images/NewsImages/";
            bool result = false;

            if (!IsCinema() || !IsCompany()) {
                Session["RedirectURL"] = "/Admin/ManageNews_Add";
                return Redirect("/Admin/Login");
            }
            if (!ModelState.IsValid) return View(promotionModel);

            // // Get uploaded HttpPostedFileBase
            var thumbnail = promotionModel.Thumbnail;

            // Get uploaded file path
            string thumbnailUrl = imagePath + thumbnail.FileName;

            // Save uploaded poster and cover image
            thumbnail.SaveAs(Server.MapPath(Url.Content(thumbnailUrl)));

            var title = promotionModel.Title;
            var description = promotionModel.Description;
            var content = promotionModel.Content;
            var priceOff = promotionModel.PriceOff;

            var news = new News {
                Title = title,
                Description = description,
                Content = content,
                ThumbnailURL = thumbnailUrl,
                PostedDate = DateTime.Now.Date,
                ViewCount = 0
            };

            result = newsRepository.InsertNews(news);

            if (result) {
                Promote promote = new Promote {
                    NewsID = news.NewsID,
                    PriceOff = promotionModel.PriceOff
                };
                result = promotionRepository.Insert(promote);
            }

            var tags = promotionModel.Tags.Split(' ').ToList();
            if (result && tags.Count() != 0) {
                result = tagRepository.InsertTagForNews(tags, news.NewsID);
            }

            if (result)
                ViewBag.Message = "Thêm tin thành công";
            else
                ViewBag.Message = "Xảy ra lỗi khi thêm tin mới";
            return View("ManagePromotion_Add");
        }

        #endregion
    }
}