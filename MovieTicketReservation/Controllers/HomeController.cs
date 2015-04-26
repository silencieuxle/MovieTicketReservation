using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;
using System.Net;
using System.Text;
using System.IO;

namespace MovieTicketReservation.Controllers {
    public class HomeController : Controller {
        readonly MoviesDbDataContext _db = new MoviesDbDataContext();
        public ActionResult Index() {
            return View(GetAllMovies());
        }

        public ActionResult GetShowtimeByScheduleId(int scheduleId, int movieId) {
            var result = new List<dynamic>();
            var date = _db.Schedules.FirstOrDefault(s => s.ScheduleID == scheduleId).Date;
            var cinema = _db.Schedules.FirstOrDefault(s => s.ScheduleID == scheduleId).Cine_MovieDetail.CinemaID;
            var schedule = _db.Schedules.Where(s => s.Date == (DateTime)date && s.Date >= DateTime.Now && s.Cine_MovieDetail.MovieID == movieId && s.Cine_MovieDetail.CinemaID == cinema);
            foreach (var item in schedule) {
                result.Add(new { Time = item.ShowTime.StartTime, item.ScheduleID });
            }
            return Json(result.OrderBy(r => r.Time), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDateByCinemaIdAndMovieId(int movieId, string cinemaId) {
            var dates = _db.Schedules.Where(s => s.Cine_MovieDetail.MovieID == movieId &&
                s.Cine_MovieDetail.CinemaID == cinemaId &&
                s.Date >= DateTime.Now);
            var result = new List<dynamic>();
            foreach (var item in dates) {
                if (result.All(x => x.Date != item.Date)) result.Add(new { item.Date, item.ScheduleID });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCinemasByMovieId(int movieId) {
            var cinemas = _db.Cine_MovieDetails.Where(x => x.MovieID == movieId);
            var result = cinemas.Join(_db.Cinemas, c => c.CinemaID, x => x.CinemaID, (c, x) => new {
                x.Name,
                ID = x.CinemaID
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMoviesByScheduleType(int type) {
            var movies = GetAllMovies().Where(m => m.ScheduleType == type);
            return Json(movies, JsonRequestBehavior.AllowGet);
        }

        public List<MovieBasicModel> GetAllMovies() {
            var movies = _db.Movies.Select(movie => new MovieBasicModel {
                MovieId = movie.MovieID,
                ScheduleType = MoviesController.GetScheduleType((DateTime)movie.BeginShowDate, (int)movie.Duration),
                ThumbnailUrl = movie.ThumbnailURL,
                BeginShowDate = ((DateTime)movie.BeginShowDate).ToShortDateString(),
                Title = movie.Title
            });
            return movies.ToList();
        }
    }
}