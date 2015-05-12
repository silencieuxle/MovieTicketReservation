using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.MovieService;
using MovieTicketReservation.Services.ScheduleService;
using MovieTicketReservation.Services.ShowtimeService;
using MovieTicketReservation.Services.CinemaService;
using MovieTicketReservation.Services.CinemaMovieDetailsService;

namespace MovieTicketReservation.Controllers {
    public class HomeController : Controller {
        private DbEntities context = new DbEntities();
        private IMovieRepository movieRepository;
        private IScheduleRepository scheduleRepository;
        private IShowtimeRepository showtimeRepository;
        private ICinemaRepository cinemaRepository;
        private ICinemaMovieRepository cinemaMovieRepository;

        public HomeController() {
            this.movieRepository = new MovieRepository(context);
            this.scheduleRepository = new ScheduleRepository(context);
            this.showtimeRepository = new ShowtimeRepository(context);
            this.cinemaRepository = new CinemaRepository(context);
            this.cinemaMovieRepository = new CinemaMovieDetailsRepository(context);
        }

        public ActionResult About() {
            return View();
        }

        public ActionResult Index() {
            return View(movieRepository.GetCanBeReservedMovies());
        }

        public ActionResult GetShowtimeByScheduleId(int scheduleId, int movieId) {
            var result = new List<dynamic>();
            var date = scheduleRepository.GetScheduleByID(scheduleId).Date;
            var cinema = scheduleRepository.GetScheduleByID(scheduleId).Cine_MovieDetails.CinemaID;
            var schedule = scheduleRepository.GetSchedules().Where(s => s.Date == (DateTime)date && s.Date >= DateTime.Now && s.Cine_MovieDetails.MovieID == movieId && s.Cine_MovieDetails.CinemaID == cinema);
            foreach (var item in schedule) {
                result.Add(new { Time = item.ShowTime.StartTime, item.ScheduleID });
            }
            return Json(result.OrderBy(r => r.Time), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDateByCinemaIdAndMovieId(int movieId, string cinemaId) {
            var dates = scheduleRepository.GetSchedules().Where(s => s.Cine_MovieDetails.MovieID == movieId &&
                s.Cine_MovieDetails.CinemaID == cinemaId &&
                s.Date >= DateTime.Now);
            var result = new List<dynamic>();
            foreach (var item in dates) {
                if (result.All(x => x.Date != item.Date)) result.Add(new { item.Date, item.ScheduleID });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCinemasByMovieId(int movieId) {
            var cinemas = cinemaMovieRepository.GetDetailsByMovieID(movieId);
            var result = cinemas.Join(cinemaRepository.GetCinemas(), c => c.CinemaID, x => x.CinemaID, (c, x) => new {
                x.Name,
                ID = x.CinemaID
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMoviesByScheduleType(int type) {
            string[] types = { type + "" };
            var movies = movieRepository.GetMoviesByScheduleTypes(types);
            return Json(movies.Select(m => new { MovieID = m.MovieID, ThumbnailUrl = m.ThumbnailURL }), JsonRequestBehavior.AllowGet);
        }
    }
}