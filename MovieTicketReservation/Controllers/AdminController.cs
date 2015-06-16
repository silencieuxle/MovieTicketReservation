using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.Models;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.AdminAccountService;
using MovieTicketReservation.Services.AgeLimitationService;
using MovieTicketReservation.Services.BookingHeaderService;
using MovieTicketReservation.Services.CinemaMovieDetailsService;
using MovieTicketReservation.Services.CinemaService;
using MovieTicketReservation.Services.EditionService;
using MovieTicketReservation.Services.GenreService;
using MovieTicketReservation.Services.MemberService;
using MovieTicketReservation.Services.MovieService;
using MovieTicketReservation.Services.NewsRepository;
using MovieTicketReservation.Services.PromotionService;
using MovieTicketReservation.Services.RoomService;
using MovieTicketReservation.Services.ScheduleService;
using MovieTicketReservation.Services.SeatShowDetailsService;
using MovieTicketReservation.Services.ShowtimeService;
using MovieTicketReservation.Services.SubtitleService;
using MovieTicketReservation.Services.TagRepository;
using MovieTicketReservation.Services.TicketClassService;
using MovieTicketReservation.ViewModels;

namespace MovieTicketReservation.Controllers {
    public class AdminController : Controller {
        private DbEntities context = new DbEntities();

        private IAdminAccountRepository     adminAccountRepository;
        private IAgeLimitationRepository    ageLimitationRepository;
        private IBookingRepository          bookingRepository;
        private ICinemaMovieRepository      cinemaMovieRepository;
        private ICinemaRepository           cinemaRepository;
        private IEditionRepository          editionRepository;
        private IGenreRepository            genreRepository;
        private IMemberRepository           memberRepository;
        private IMovieRepository            movieRepository;
        private INewsRepository             newsRepository;
        private IPromotionService           promotionRepository;
        private IRoomRepository             roomRepository;
        private IScheduleRepository         scheduleRepository;
        private ISeatShowRepository         seatShowRepository;
        private IShowtimeRepository         showtimeRepository;
        private ISubtitleRepository         subtitleRepository;
        private ITagRepository              tagRepository;
        private ITicketClassRepository      ticketClassRepository;

        public AdminController() {
            this.adminAccountRepository = new AdminAccountRepository(context);
            this.ageLimitationRepository = new AgeLimitationRepository(context);
            this.bookingRepository = new BookingHeaderRepository(context);
            this.cinemaMovieRepository = new CinemaMovieDetailsRepository(context);
            this.cinemaRepository = new CinemaRepository(context);
            this.editionRepository = new EditionRepository(context);
            this.genreRepository = new GenreRepository(context);
            this.memberRepository = new MemberRepository(context);
            this.movieRepository = new MovieRepository(context);
            this.newsRepository = new NewsRepository(context);
            this.promotionRepository = new PromotionRepository(context);
            this.roomRepository = new RoomRepository(context);
            this.scheduleRepository = new ScheduleRepository(context);
            this.seatShowRepository = new SeatShowRepository(context);
            this.showtimeRepository = new ShowtimeRepository(context);
            this.subtitleRepository = new SubtitleRepository(context);
            this.tagRepository = new TagRepository(context);
            this.ticketClassRepository = new TicketClassRepository(context);
        }

        public ActionResult Index() {
            if (!IsCompany() && !IsCinema()) {
                Session.Abandon();
                return Redirect("/Admin/Login");
            }
            ViewBag.TotalMembers = memberRepository.GetAllMembers().Count();
            ViewBag.TotalBookings = bookingRepository.GetBookingHeadersByDate(DateTime.Now).Count();
            ViewBag.ShowingMovies = movieRepository.GetCanBeReservedMovies().Count();
            var bookingHeaders = bookingRepository.GetBookingHeadersByDate(DateTime.Now.Date);
            long total = 0;
            foreach (var item in bookingHeaders) {
                var price = seatShowRepository.GetDetailsByBookingHeaderID(item.HeaderID).Sum(x => x.TicketClass.Price);
                total += (int)price;
            }
            ViewBag.TodayRevenue = total;
            return View();
        }

        public ActionResult GetPage(string page, string param) {
            switch (page) {
                case "login":
                    Session.Abandon();
                    return Redirect("Login");

                case "moviestats_overall":
                    ViewBag.TotalMovies = movieRepository.GetAllMovies().Count();
                    ViewBag.ShowingMovies = movieRepository.GetCanBeReservedMovies().Count();
                    ViewBag.CommingMovies = movieRepository.GetCommingMovies().Count();
                    ViewBag.FutureMovies = movieRepository.GetFutureMovies().Count();
                    return PartialView("_MovieStats_Overall");

                case "ticketstats_overall":
                    return PartialView("_TicketStats_Overall");

                case "ticketstats_movie":
                    var dates = new List<string>();
                    for (var date = DateTime.Now; date >= DateTime.Now.AddDays(-30); date = date.AddDays(-1)) {
                        dates.Add(date.ToShortDateString());
                    }
                    ViewBag.Dates = dates;
                    return PartialView("_TicketStats_CanBeReservedMovies");

                case "systemstats":
                    return PartialView("_SystemStats");

                case "managemovie_all":
                    return PartialView("_ManageMovie_All");

                case "manageschedule_all":
                    return PartialView("_ManageSchedule_All");

                case "manageschedule_add":
                    string cinemaId = (string)Session["AdminSection"];
                    var movies = movieRepository.GetCanBeScheduledMovies()
                        .Join(cinemaMovieRepository.GetDetailsByCinemaID((string)Session["AdminSection"]), m => m.MovieID, d => d.MovieID, (m, d) => new { Movie = m })
                        .Select(x => x.Movie).ToList();
                    ViewBag.Movies = movies;
                    ViewBag.Rooms = roomRepository.GetRoomsByCinemaID(cinemaId);
                    ViewBag.Showtimes = showtimeRepository.GetShowtimes();
                    var promotion = promotionRepository.GetFixedDayOfWeekPromotionByDay((int)DateTime.Now.DayOfWeek);
                    if (promotion != null) {
                        ViewBag.Promotion = promotion;
                    } else {
                        ViewBag.Promotions = promotionRepository.GetActiveDuratedPromotions();
                    }
                    return PartialView("_ManageSchedule_Create");

                case "managemember_all":
                    return PartialView("_ManageMember_All");

                case "managemember_add":
                    return PartialView("_ManageMember_Add");

                case "managemember_edit":
                    return PartialView("_ManageMember_Edit", memberRepository.GetMemberByID(Convert.ToInt32(param)));

                case "promotion_all":
                    return PartialView("_ManagePromotion_All");

                case "managenews_all":
                    return PartialView("_ManageNews_All");

                case "managenews_add":
                    return PartialView("_ManageNews_Add");

                case "revenue":
                    ViewBag.Cinemas = cinemaRepository.GetCinemas();
                    return PartialView("_Revenue_Cinema");

                default:
                    return Redirect("Index");
            }
        }

        private bool IsCinema() {
            if (Session["Role"] == null)
                return false;
            if ((int)Session["Role"] == 2)
                return true;
            return false;
        }

        private bool IsCompany() {
            if (Session["Role"] == null)
                return false;
            if ((int)Session["Role"] == 1)
                return true;
            return false;
        }

        [HttpGet]
        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel) {
            if (!ModelState.IsValid)
                return View(loginModel);
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
            Session["ReservingSeats"] = new List<int>();
            Session["UID"] = user.MemberID;
            Session["Role"] = admin.RoleID;
            Session["AdminSection"] = admin.Section;
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
            string avatarUrl = imagePath + Session["UID"].ToString() + "-" + avatar.FileName;

            // Save uploaded poster and cover image
            if (avatar.ContentLength != 0)
                avatar.SaveAs(Server.MapPath(Url.Content(avatarUrl)));
            else {
                return Json(new { Success = false, ErrorMessage = "Lỗi khi nhận file" }, JsonRequestBehavior.AllowGet);
            }
            var member = memberRepository.GetMemberByID((int)Session["UID"]);
            member.AvatarURL = avatarUrl;
            var result = memberRepository.UpdateMember(member);
            if (!result)
                return Json(new { Success = false, ErrorMessage = "Xảy ra lỗi khi cập nhật thông tin." }, JsonRequestBehavior.AllowGet);
            return Json(new { Success = true, AvatarURL = avatarUrl }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxGetMembers(int headIndex, int type) {
            var result = memberRepository.GetAllMembers();
            if (headIndex >= result.Count()) {
                return null;
            }
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
                Data = result.Select(r => new {
                    MemberID = r.MemberID,
                    Email = r.Email,
                    Fullname = r.Lastname + " " + r.Firstname,
                    IDCardNumber = r.IDCardNumber,
                    Phone = r.Phone
                }),
                HeadIndex = index
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxDeleteMember(int memberId) {
            var result = memberRepository.DeleteMember(memberId);
            return Json(new { Success = result, ErrorMessage = "Cannot delete member" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxUpdateMember([Bind(Exclude = "Password, MemberID")]Member updateMember, int memberId) {
            if (!ModelState.IsValid)
                return Json(new { Success = false, ErrorMessage = "Cannot update member information." }, JsonRequestBehavior.AllowGet);

            var member = memberRepository.GetMemberByID(memberId);
            member.Firstname = updateMember.Firstname;
            member.Lastname = updateMember.Lastname;
            member.Phone = updateMember.Phone;
            member.Address = updateMember.Address;
            member.IDCardNumber = updateMember.IDCardNumber;
            member.Birthday = updateMember.Birthday;
            member.Gender = updateMember.Gender;

            var result = memberRepository.UpdateMember(member);

            if (result)
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            return Json(new { Success = false, ErrorMessage = "Cannot update member information." }, JsonRequestBehavior.AllowGet);
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
        public ActionResult AjaxGetPromotionsByDate(string stringDate) {
            var date = DateTime.Parse(stringDate);
            var promotion = promotionRepository.GetFixedDayOfWeekPromotionByDay((int)date.DayOfWeek);
            if (promotion != null) {
                return Json(new List<object> { new { PromotionID = promotion.PromoteID, Title = promotion.Title, Type = "Fixed" } }, JsonRequestBehavior.AllowGet);
            } else {
                var cinemas = cinemaRepository.GetCinemaByID((string)Session["AdminSection"]);
                var promotions = promotionRepository.GetActiveDuratedPromotions();
                var pr = cinemas.Promotes.Join(promotions, c => c.PromoteID, p => p.PromoteID, (c, p) => new {
                    PromotionID = p.PromoteID,
                    Title = p.Title,
                    Type = "Duration"
                });
                return Json(pr, JsonRequestBehavior.AllowGet);
                //return Json(promotions.Select(p => new { PromotionID = p.PromoteID, Title = p.Title, Type = "Duration" }), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AjaxGetSchedules(int headIndex, int type) {
            var result = scheduleRepository.GetSchedulesByCinemaID((string)Session["AdminSection"]);
            if (headIndex >= result.Count()) {
                return null;
            }
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
                Data = result.Select(s => new {
                    ID = s.ScheduleID,
                    MovieID = s.Cine_MovieDetails.MovieID,
                    MovieTitle = s.Cine_MovieDetails.Movie.Title,
                    Room = s.Room.Name,
                    Date = ((DateTime)s.Date).ToShortDateString(),
                    Time = Helper.ToTimeString((TimeSpan)s.ShowTime.StartTime)
                }),
                HeadIndex = index
            }, JsonRequestBehavior.AllowGet);
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
                if (beginShowDate > DateTime.Now)
                    startDate = beginShowDate;
                else
                    startDate = DateTime.Now;

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
        public ActionResult AjaxAddScheduleManually(int movieId, string roomId, string date, int showtimeId, int? promoteId) {
            if (IsCinema()) {
                // Get needed information
                var movie = movieRepository.GetMovieByID(movieId);
                var showtime = (TimeSpan)showtimeRepository.GetShowtimeByID(showtimeId).StartTime;

                DateTime showdate = DateTime.Parse(date);
                int cineMovieID = cinemaMovieRepository.GetDetailsByCinemaIDAndMovieID((string)Session["AdminSection"], movieId).DetailsID;

                // Check if any schedule is created at the current date, showtime and room
                var duplicate = scheduleRepository.GetSchedules()
                    .Where(x => x.ShowTimeID == showtimeId && x.RoomID == roomId && (DateTime)x.Date == showdate);

                // ASSUME THAT THE MANAGERS ARE SMART ENOUGH SO THAT THEY WILL NOT ADD SCHEDULE THE DUMP WAY

                if (duplicate.Count() != 0)
                    return Json(new { Success = false, ErrorMessage = "Suất cần thêm đã có." }, JsonRequestBehavior.AllowGet);

                // If nothing went wrong, ready to go
                // Create schedule
                var result = scheduleRepository.InsertSchedule(new Schedule {
                    Cine_MovieDetailsID = cineMovieID,
                    Date = showdate,
                    RoomID = roomId,
                    ShowTimeID = showtimeId,
                    PromoteID = promoteId
                });

                // Succeeded create schedule? Good
                if (result != 0) {
                    string ticketClassId = "";
                    if (movie.EditionID == "MOV2D") {
                        if (showtime.Hours >= 17)
                            ticketClassId = "2DM";
                        else
                            ticketClassId = "2DN";
                    } else {
                        if (showtime.Hours >= 17)
                            ticketClassId = "3DM";
                        else
                            ticketClassId = "3DN";
                    }
                    CreateSeatShowDetails(result, roomId, ticketClassId);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                } else {
                    return Json(new { Success = false, ErrorMessage = "System's error. Cannot create schedule." }, JsonRequestBehavior.AllowGet);
                }
            } else {
                Session.Abandon();
                return Redirect("/Admin/Login");
            }
        }

        [HttpPost]
        public ActionResult AjaxAddScheduleAuto(int movieId, string roomId, string date, int firstShowtimeId, int numberOfSchedule, int? promoteId) {
            if (IsCinema()) {
                // Get needed information
                var movie = movieRepository.GetMovieByID(movieId);

                // Do some stupid complex equations to get ready
                var firstShowTime = (TimeSpan)showtimeRepository.GetShowtimeByID(firstShowtimeId).StartTime;
                int startTime = firstShowTime.Hours * 60 + firstShowTime.Minutes;
                int movielength = (int)movieRepository.GetMovieByID(movieId).MovieLength;
                int numberOfScheduleToCreate = (22 * 60 + 30 - startTime) / movielength;
                numberOfScheduleToCreate = numberOfScheduleToCreate > numberOfSchedule ? numberOfSchedule : numberOfScheduleToCreate;

                int result = 0;
                // First showtime
                int currentShowTimeId = firstShowtimeId;
                DateTime showdate = DateTime.Parse(date);

                for (int i = 1; i <= numberOfScheduleToCreate; i++) {
                    var currentShowTime = (TimeSpan)showtimeRepository.GetShowtimeByID(currentShowTimeId).StartTime;

                    int cineMovieID = cinemaMovieRepository.GetDetailsByCinemaIDAndMovieID((string)Session["AdminSection"], movieId).DetailsID;

                    // Check if any schedule is created at the current date, showtime and room
                    var duplicate = scheduleRepository.GetSchedules()
                        .Where(x => x.ShowTimeID == currentShowTimeId && x.RoomID == roomId && (DateTime)x.Date == showdate);
                    if (duplicate.Count() != 0)
                        return Json(new { Success = false, ErrorMessage = "Một lịch chiếu cần tạo trùng lặp với lịch chiếu khác. Một số suất khác đã được tạo, xem danh sách bên. Quy trình được huỷ bỏ." }, JsonRequestBehavior.AllowGet);

                    // If nothing went wrong, ready to go
                    // Create schedule
                    result = scheduleRepository.InsertSchedule(new Schedule {
                        Cine_MovieDetailsID = cineMovieID,
                        Date = showdate,
                        RoomID = roomId,
                        ShowTimeID = currentShowTimeId,
                        PromoteID = promoteId
                    });

                    // Succeeded create schedule? Good
                    if (result != 0) {
                        string ticketClassId = "";
                        if (movie.EditionID == "MOV2D") {
                            if (currentShowTime.Hours < 17)
                                ticketClassId = "2DM";
                            else
                                ticketClassId = "2DN";
                        } else {
                            if (currentShowTime.Hours < 17)
                                ticketClassId = "3DM";
                            else
                                ticketClassId = "3DN";
                        }
                        CreateSeatShowDetails(result, roomId, ticketClassId);
                    } else {
                        return Json(new { Success = false, ErrorMessage = "Có lỗi xảy ra với hệ thống, một số suất chiếu đã được tạo, xem danh sách bên. Quy trình được huỷ bỏ" });
                    }
                    currentShowTimeId += movielength / 15 + 1;
                }
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            } else {
                return null;
            }
        }

        private bool CreateSeatShowDetails(int scheduleId, string roomId, string classId) {
            return seatShowRepository.InsertSeatsWithDetails(scheduleId, roomId, classId);
        }

        #endregion

        #region News Management

        [HttpGet]
        public ActionResult ManageNews_Add() {
            return View();
        }

        [HttpPost]
        public ActionResult ManageNews_Add(NewsModel newsModel) {
            string imagePath = "/Content/Images/NewsImages/";
            bool result = false;

            if (!ModelState.IsValid)
                return View(newsModel);

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

        [HttpPost]
        public ActionResult AjaxGetNews(int headIndex, int type) {
            var result = newsRepository.GetNews();
            if (headIndex >= result.Count()) {
                return null;
            }
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
                Data = result.Select(r => new {
                    NewsID = r.NewsID,
                    NewsTitle = r.Title,
                    ThumbnailURL = r.ThumbnailURL,
                    CreatedDate = ((DateTime)r.PostedDate).ToShortDateString()
                }),
                HeadIndex = index
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxDeleteNews(int newsId) {
            var result = newsRepository.DeleteNews(newsId);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Manage Movie

        [HttpGet]
        public ActionResult ManageMovie_Add() {
            InitializePage();
            return View();
        }

        [HttpPost]
        public ActionResult ManageMovie_Add(MovieAddModel movieAddModel) {
            InitializePage();
            string imagePath = "/Content/Images/MovieImages/";

            if (!IsCompany()) {
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
                    if (result)
                        continue;
                    else
                        break;
                }
            }

            if (result) {
                ViewBag.Message = "Thêm film mới thành công";
            } else {
                ViewBag.Message = "Xảy ra lỗi khi thêm film";
            }
            return View();
        }

        [HttpPost]
        public ActionResult AjaxGetMovies(int headIndex, int type) {
            var result = movieRepository.GetAllMovies();
            if (headIndex >= result.Count()) {
                return null;
            }
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
                Data = result.Select(m => new {
                    MovieID = m.MovieID,
                    Title = m.Title,
                    Edition = m.MovieEdition.Name,
                    ReleasedDate = ((DateTime)m.ReleasedDate).ToShortDateString(),
                    BeginShowDate = ((DateTime)m.BeginShowDate).ToShortDateString()
                }),
                HeadIndex = index
            },
            JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxDeleteMovie(int movieId) {
            var result = movieRepository.DeleteMovie(movieId);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        private void InitializePage() {
            ViewBag.Ages = ageLimitationRepository.GetAgeLimitations();
            ViewBag.Editions = editionRepository.GetMovieEditions();
            ViewBag.Subtitles = subtitleRepository.GetSubtitles();
            ViewBag.Genres = genreRepository.GetMovieGenres();
            ViewBag.Cinemas = cinemaRepository.GetCinemas();
        }

        #endregion

        #region Manage Promotion

        [HttpGet]
        public ActionResult ManagePromotion_Add() {
            if (!IsCompany())
                return View("Index");
            ViewBag.Cinemas = cinemaRepository.GetCinemas().ToList();
            return View();
        }

        [HttpPost]
        public ActionResult ManagePromotion_Add(PromotionModel promotionModel) {
            if (!ModelState.IsValid) {
                ViewBag.Cinemas = cinemaRepository.GetCinemas().ToList();
                return View(promotionModel);
            }
            string imagePath = "/Content/Images/NewsImages/";
            bool result = false;


            // // Get uploaded HttpPostedFileBase
            var thumbnail = promotionModel.Thumbnail;

            // Get uploaded file path
            string thumbnailUrl = imagePath + thumbnail.FileName;

            // Save uploaded poster and cover image
            thumbnail.SaveAs(Server.MapPath(Url.Content(thumbnailUrl)));

            var title = promotionModel.Title;
            var promotionTitle = promotionModel.PromotionTitle;
            var description = promotionModel.Description;
            var content = promotionModel.Content;
            var priceOff = promotionModel.PriceOff;
            var cinemas = promotionModel.Cinemas;
            var promotionType = promotionModel.PromotionType;

            int? fixedDay = promotionModel.FixedDayOfWeek;
            int? duration = promotionModel.Duration;
            DateTime? beginDate = promotionModel.BeginDay;

            if (promotionType == 0) {
                duration = null;
                beginDate = null;
            } else {
                fixedDay = null;
            }

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
                    PriceOff = priceOff,
                    Title = promotionTitle,
                    BeginDay = beginDate,
                    Duration = duration,
                    FixedDayOfWeek = fixedDay
                };
                result = promotionRepository.Insert(promote);
                foreach (var cinemaId in cinemas) {
                    var cinema = cinemaRepository.GetCinemaByID(cinemaId);
                    cinema.Promotes.Add(promote);
                    cinemaRepository.UpdateCinema(cinema);
                }
            }

            var tags = promotionModel.Tags.Split(' ').ToList();
            if (result && tags.Count() != 0) {
                result = tagRepository.InsertTagForNews(tags, news.NewsID);
            }

            if (result)
                ViewBag.Message = "Thêm tin thành công";
            else
                ViewBag.Message = "Xảy ra lỗi khi thêm tin mới";
            ViewBag.Cinemas = cinemaRepository.GetCinemas().ToList();
            return View("ManagePromotion_Add");
        }

        [HttpPost]
        public ActionResult AjaxGetPromotions(int headIndex, int type) {
            var result = promotionRepository.GetPromotions();
            if (headIndex >= result.Count()) {
                return null;
            }
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
                Data = result.Select(r => new {
                    PromotionID = r.PromoteID,
                    Title = r.Title,
                    PriceOff = r.PriceOff,
                    FixedDay = r.FixedDayOfWeek != null ? r.FixedDayOfWeek.ToString() : "N/A",
                    BeginDay = r.BeginDay != null ? ((DateTime)r.BeginDay).ToShortDateString() : "N/A",
                    Duration = r.Duration != null ? r.Duration.ToString() : "N/A"
                }),
                HeadIndex = index
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxDeletePromotion(int promotionId) {
            var cinemas = cinemaRepository.GetCinemas().Where(c => c.Promotes.Any(x => x.PromoteID == promotionId));
            foreach (var item in cinemas) {
                item.Promotes.Remove(promotionRepository.GetPromotionByID(promotionId));
                cinemaRepository.UpdateCinema(item);
            }
            var data = promotionRepository.GetPromotionByID(promotionId);

            var news = newsRepository.GetNewsByID((int)data.NewsID);
            news.Tags.Clear();
            newsRepository.UpdateNews(news);
            var newsRes = newsRepository.DeleteNews((int)data.NewsID);

            var promotionRes = promotionRepository.Delete(promotionId);
            var result = (promotionRes && newsRes) ? true : false;
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Staticstics

        public ActionResult AjaxTicketStat_OverallStats_CanBeReserved(string stringDate = null) {
            if (stringDate == null) {

                var bookingHeaders = bookingRepository.GetBookingHeaders();
                var movies = movieRepository.GetCanBeReservedMovies();

                var result = new List<object>();

                foreach (var movie in movies) {
                    result.Add(new { label = movie.Title, value = bookingRepository.GetBookingHeadersByMovieID(movie.MovieID).Count() });
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            } else {
                var date = DateTime.Parse(stringDate);
                var bookingHeaders = bookingRepository.GetBookingHeadersByDate(date);
                var movies = movieRepository.GetCanBeReservedMovies();

                var result = new List<object>();

                foreach (var movie in movies) {
                    result.Add(new { label = movie.Title, value = bookingRepository.GetBookingHeadersByMovieID(movie.MovieID).Count() });
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AjaxTicketStat_OverallStats_AllMovies() {
            var bookingHeaders = bookingRepository.GetBookingHeaders();
            var movies = movieRepository.GetAllMovies();

            var result = new List<object>();

            foreach (var movie in movies) {
                result.Add(new { label = movie.Title, value = bookingRepository.GetBookingHeadersByMovieID(movie.MovieID).Count() });
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AjaxRevenue_OverallStat_Week() {
            int today = (int)DateTime.Now.DayOfWeek;
            var result = bookingRepository.GetBookingHeaders()
                .Where(b => ((DateTime)b.ReservedTime).Date >= DateTime.Now.AddDays(-today).Date && ((DateTime)b.ReservedTime).Date <= DateTime.Now.Date).ToList()
                .GroupBy(b => ((DateTime)b.ReservedTime).Date, (date, data) => new {
                    date = date.ToShortDateString(),
                    total = (int)data.Sum(x => x.Seat_ShowDetails.Sum(d => d.TicketClass.Price))
                });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxRevenue_OverallStat_All() {
            int today = (int)DateTime.Now.DayOfWeek;
            List<object> result = new List<object>();
            var cinemaList = cinemaRepository.GetCinemas();
            for (var date = DateTime.Now.AddDays(-today).Date; date <= DateTime.Now.Date; date = date.AddDays(1)) {
                List<long> res = new List<long>();
                foreach (var cinema in cinemaList) {
                    var revenue = bookingRepository.GetBookingHeadersByDate(date)
                    .Where(b => b.Seat_ShowDetails.First().Schedule.Cine_MovieDetails.CinemaID == cinema.CinemaID)
                    .Sum(b => b.Seat_ShowDetails.Sum(d => d.TicketClass.Price));
                    res.Add((long)revenue);
                }
                result.Add(new { date = date.ToShortDateString(), a = res[0], b = res[1], c = res[2], d = res[3] });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxRevenue_Cinema(string cinemaId) {
            int today = (int)DateTime.Now.DayOfWeek;

            var todayResult = bookingRepository.GetBookingHeadersByDate(DateTime.Now.Date)
                .Where(b => b.Seat_ShowDetails.First().Schedule.Cine_MovieDetails.CinemaID == cinemaId)
                .Sum(b => b.Seat_ShowDetails.Sum(d => d.TicketClass.Price));

            var lineResult = bookingRepository.GetBookingHeaders()
                .Where(b => ((DateTime)b.ReservedTime).Date >= DateTime.Now.AddDays(-today).Date &&
                    ((DateTime)b.ReservedTime).Date <= DateTime.Now.Date &&
                    b.Seat_ShowDetails.First().Schedule.Cine_MovieDetails.CinemaID == cinemaId).ToList()
                .GroupBy(b => ((DateTime)b.ReservedTime).Date, (date, data) => new {
                    date = date.ToShortDateString(),
                    total = (int)data.Sum(x => x.Seat_ShowDetails.Sum(d => d.TicketClass.Price))
                });
            return Json(new { Today = todayResult, Line = lineResult }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}