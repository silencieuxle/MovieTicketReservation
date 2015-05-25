using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.ScheduleService;
using MovieTicketReservation.Services.MovieService;
using MovieTicketReservation.Services.CinemaService;
using MovieTicketReservation.Services.CinemaMovieDetailsService;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.ViewModels;

namespace MovieTicketReservation.Controllers {
    public class ScheduleController : Controller {
        private IMovieRepository movieRepository;
        private ICinemaRepository cinemaRepository;
        private IScheduleRepository scheduleRepository;
        private ICinemaMovieRepository cinemaMovieRepository;
        private DbEntities context = new DbEntities();

        public ScheduleController() {
            this.movieRepository = new MovieRepository(context);
            this.cinemaRepository = new CinemaRepository(context);
            this.scheduleRepository = new ScheduleRepository(context);
            this.cinemaMovieRepository = new CinemaMovieDetailsRepository(context);
        }
        // GET: Schedule
        public ActionResult Index() {
            ViewBag.Movies = movieRepository.GetCanBeReservedMovies();
            return View();
        }

        #region Ajax methods

        public ActionResult AjaxGetCinemaByMovieID(int movieId) {
            var cinemas = cinemaMovieRepository.GetDetailsByMovieID(movieId).Select(d => d.Cinema).ToList();
            return Json(cinemas.Select(c => new { Name = c.Name, CinemaID = c.CinemaID }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetScheduleByCinemaIDAndModieID(string cinemaId, int movieId) {
            var schedules = scheduleRepository.GetSchedulesByCinemaIDAndMovieID(cinemaId, movieId)
                .Where(s => (TimeSpan)s.ShowTime.StartTime >= DateTime.Now.TimeOfDay && ((DateTime)s.Date).Date == DateTime.Now.Date)
                .GroupBy(sch => sch.Date, (key, group) => new {
                    ShowingDate = ((DateTime)key).ToShortDateString(),
                    Times = group.Where(sche => sche.Cine_MovieDetails.CinemaID == cinemaId && sche.Date == key)
                        .OrderBy(x => x.ShowTime.StartTime)
                        .Select(sh => new {
                            ScheduleID = sh.ScheduleID,
                            ShowTime = ((TimeSpan)sh.ShowTime.StartTime).ToTimeString()
                        }).ToList()
                }).ToList();
            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}