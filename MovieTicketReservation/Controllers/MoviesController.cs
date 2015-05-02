using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.Models;
using MovieTicketReservation.ViewModels;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.MovieService;
using MovieTicketReservation.Services.GenreService;
using MovieTicketReservation.Services.EditionService;
using MovieTicketReservation.Services.MemberService;
using MovieTicketReservation.Services.CinemaService;
using MovieTicketReservation.Services.ScheduleService;
using MovieTicketReservation.Services.CinemaMovieDetailsService;
using Newtonsoft.Json;

namespace MovieTicketReservation.Controllers {
    public class MoviesController : Controller {
        private DbEntities context = new DbEntities();
        private IMovieRepository movieRepository;
        private IEditionRepository editionRepository;
        private IGenreRepository genreRepository;
        private ICinemaRepository cinemaReppsitory;
        private IScheduleRepository scheduleRepository;
        private ICinemaMovieRepository cinemaMovieRepository;

        public MoviesController() {
            this.movieRepository = new MovieRepository(context);
            this.genreRepository = new GenreRepository(context);
            this.editionRepository = new EditionRepository(context);
            this.cinemaReppsitory = new CinemaRepository(context);
            this.scheduleRepository = new ScheduleRepository(context);
            this.cinemaMovieRepository = new CinemaMovieDetailsRepository(context);
        }

        // GET: Movies
        [HttpGet]
        public ActionResult Index() {
            // Data to display
            ViewBag.EditionList = editionRepository.GetMovieEditions();
            ViewBag.CinemaList = cinemaReppsitory.GetCinemas();
            ViewBag.GenreList = genreRepository.GetMovieGenres();

            var movies = movieRepository.GetAllMovies().Select(m => new MovieExtended {
                Actors = m.Actors,
                AgeLimitation = m.AgeLimitation,
                AgeLimitationID = m.AgeLimitationID,
                Available = m.Available,
                BeginShowDate = m.BeginShowDate,
                Cine_MovieDetails = m.Cine_MovieDetails,
                Description = m.Description,
                Director = m.Director,
                Duration = m.Duration,
                LongDescription = m.LongDescription,
                MovieEditions = m.MovieEditions,
                MovieGenres = m.MovieGenres,
                MovieID = m.MovieID,
                MovieLength = m.MovieLength,
                ReleasedDate = m.ReleasedDate,
                ThumbnailURL = m.ThumbnailURL,
                Title = m.Title,
                TrailerURL = m.TrailerURL,
                WideThumbnail = m.WideThumbnail,
                ScheduleType = GetScheduleType(m)
            }).OrderByDescending(i => i.BeginShowDate).ToList();
            return View(movies);
        }

        [HttpPost]
        public ActionResult Index(string query) {
            ViewBag.EditionList = editionRepository.GetMovieEditions();
            ViewBag.CinemaList = cinemaReppsitory.GetCinemas();
            ViewBag.GenreList = genreRepository.GetMovieGenres();
            var result = movieRepository.GetMoviesByTitle(query);
            return View(result);
        }

        public ActionResult AjaxFilter(string cinemaFilter, string editionFilter, string genreFilter, string scheduleTypeFilter) {
            var cinemas = JsonConvert.DeserializeObject<string[]>(cinemaFilter);
            var editions = JsonConvert.DeserializeObject<string[]>(editionFilter);
            var genres = JsonConvert.DeserializeObject<string[]>(genreFilter);
            var schedules = JsonConvert.DeserializeObject<string[]>(scheduleTypeFilter);
            var movies = movieRepository.GetAllMovies();

            if (cinemas.Count() != 0) {
                movies = movieRepository.GetMoviesByCinemas(cinemas, movies);
            }

            if (editions.Count() != 0) {
                movies = movieRepository.GetMoviesByEditions(editions, movies);
            }

            if (genres.Count() != 0) {
                movies = movieRepository.GetMoviesByGenres(genres, movies);
            }

            if (schedules.Count() != 0) {
                movies = movieRepository.GetMoviesByScheduleTypes(schedules, movies);
            }

            movies = movies.OrderByDescending(m => m.BeginShowDate).ToList();

            return PartialView("_MovieTemplate", movies);
        }

        // GET: Details?ID
        public ActionResult Details(int movieId) {
            var result = movieRepository.GetMovieByID(movieId);
            ViewBag.Schedules = GetScheduleByMovieId(movieId);
            return View(result);
        }

        public ActionResult AjaxGetScheduleViewByMovieId(int movieId) {
            var result = GetScheduleByMovieId(movieId);
            return PartialView("_PopupScheduleTemplate", result);
        }

        #region Private methods
        private List<CinemaScheduleModel> GetScheduleByMovieId(int movieId) {
            var schedules = cinemaMovieRepository.GetDetailsByMovieID(movieId).Select(c => new CinemaScheduleModel {
                CinemaId = c.CinemaID,
                CinemaName = c.Cinema.Name,
                Dates = scheduleRepository.GetSchedules().Where(sc => sc.Cine_MovieDetails.CinemaID == c.CinemaID && sc.Cine_MovieDetails.MovieID == movieId)
                                     .GroupBy(sch => sch.Date, (key, group) => new DateModel {
                                         ShowingDate = (DateTime)key,
                                         Showtimes = group.Where(sche => sche.Cine_MovieDetails.CinemaID == c.CinemaID && sche.Date == key)
                                                          .Select(sh => sh.ShowTime.StartTime != null ? new Showtime {
                                                              ScheduleId = sh.ScheduleID,
                                                              Time = (TimeSpan)sh.ShowTime.StartTime
                                                          } : null).OrderBy(x => x.Time).ToList()
                                     }).ToList()
            });
            return schedules.ToList();
        }

        private int GetScheduleType(Movie movie) {
            var beginShowDate = (DateTime)movie.BeginShowDate;
            var duration = (int)movie.Duration;

            if (beginShowDate.AddDays(duration) >= DateTime.Now) {
                if (beginShowDate <= DateTime.Now && DateTime.Now <= beginShowDate.AddDays(duration)) return 1; //Now showing Movie
                if (beginShowDate <= DateTime.Now.AddDays(7) && beginShowDate > DateTime.Now) return 2; //Coming Soon Movies
                if (beginShowDate > DateTime.Now.AddDays(7)) return 3; // Future Movies
            }
            return 0; // Ended Movies
        }
        #endregion
    }
}