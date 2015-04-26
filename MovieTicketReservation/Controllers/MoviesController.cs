using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.Models;
using Newtonsoft.Json;

namespace MovieTicketReservation.Controllers {
    public class MoviesController : Controller {
        private readonly MoviesDbDataContext _db = new MoviesDbDataContext();

        // GET: Movies
        [HttpGet]
        public ActionResult Index() {
            // Data to display
            ViewBag.EditionList = _db.MovieEditions;
            ViewBag.CinemaList = _db.Cinemas;
            ViewBag.GenreList = _db.MovieGenres;
            return View(GetAllMovies());
        }

        [HttpPost]
        public ActionResult Index(string query) {
            ViewBag.EditionList = _db.MovieEditions;
            ViewBag.CinemaList = _db.Cinemas;
            ViewBag.GenreList = _db.MovieGenres;
            var result = GetMoviesTittle(query);
            return View(result);
        }

        public ActionResult AjaxFilter(string cinemaFilter, string editionFilter, string genreFilter) {
            var cinema = JsonConvert.DeserializeObject<string[]>(cinemaFilter);
            var edition = JsonConvert.DeserializeObject<string[]>(editionFilter);
            var genre = JsonConvert.DeserializeObject<string[]>(genreFilter);
            var movies = GetAllMovies();

            if (cinema.Count() != 0) {
                movies = movies.Where(m => cinema.Any(c => m.Cinemas.Any(mc => mc.CinemaId == c))).ToList();
            }

            if (edition.Count() != 0) {
                movies = movies.Where(m => edition.Any(e => m.Editions.Any(me => me.EditionId == e))).ToList();
            }

            if (genre.Count() != 0) {
                movies = movies.Where(m => genre.Any(g => m.Genres.Any(me => me.GenreId == g))).ToList();
            }

            return PartialView("_MovieTemplate", movies);
        }

        // GET: Details?ID
        public ActionResult Details(int movieId) {
            var result = GetAllMovies().FirstOrDefault(movie => movie.MovieId == movieId);
            ViewBag.Schedules = GetScheduleByMovieId(movieId);
            return View(result);
        }

        public ActionResult AjaxGetScheduleViewByMovieId(int movieId) {
            var result = GetScheduleByMovieId(movieId);
            return PartialView("_PopupScheduleTemplate", result);
        }

        #region Private methods
        private List<CinemaScheduleModel> GetScheduleByMovieId(int movieId) {
            var schedules = _db.Cine_MovieDetails.Where(cine => cine.MovieID == movieId).Select(c => new CinemaScheduleModel {
                CinemaId = c.CinemaID,
                CinemaName = c.Cinema.Name,
                Dates = _db.Schedules.Where(sc => sc.Cine_MovieDetail.CinemaID == c.CinemaID && sc.Cine_MovieDetail.MovieID == movieId)
                                     .GroupBy(sch => sch.Date, (key, group) => new DateModel {
                                         ShowingDate = (DateTime)key,
                                         Showtimes = group.Where(sche => sche.Cine_MovieDetail.CinemaID == c.CinemaID && sche.Date == key)
                                                          .Select(sh => sh.ShowTime.StartTime != null ? new Showtime {
                                                              ScheduleId = sh.ScheduleID,
                                                              Time = (TimeSpan)sh.ShowTime.StartTime
                                                          } : null).OrderBy(x => x.Time).ToList()
                                     }).ToList()
            });
            return schedules.ToList();
        }

        private List<MovieDetailsModel> GetAllMovies() {
            var result = _db.Movies.Select(movie => new MovieDetailsModel {
                Cinemas = movie.Cine_MovieDetails.Select(c => new CinemaModel {
                    CinemaId = c.CinemaID,
                    Name = c.Cinema.Name,
                    Address = c.Cinema.Address,
                    Phone = c.Cinema.Phone,
                }).ToList(),
                MovieId = movie.MovieID,
                Duration = (int)movie.Duration,
                Title = movie.Title,
                Description = movie.Description,
                Director = movie.Director,
                Actors = movie.Actors,
                AgeLimit = movie.AgeLimitation.Description,
                Available = (bool)movie.Available,
                WideThumbnail = movie.WideThumbnail,
                LongDescription = movie.LongDescription,
                Length = (int)movie.MovieLength,
                ReleasedDate = ((DateTime)movie.ReleasedDate).ToShortDateString(),
                BeginShowDate = ((DateTime)movie.BeginShowDate).ToShortDateString(),
                ThumbnailUrl = movie.ThumbnailURL,
                TrailerUrl = movie.TrailerURL,
                ScheduleType = GetScheduleType((DateTime)movie.BeginShowDate, (int)movie.Duration),
                Genres = movie.Movie_GenreDetails.Join(_db.MovieGenres, mg => mg.GenreID, g => g.GenreID,
                    (mg, g) => new GenreModel { GenreId = g.GenreID, Name = g.Name, Description = g.Description }).ToList(),
                Editions = movie.Movie_EditionDetails.Join(_db.MovieEditions, mg => mg.EditionID, g => g.EditionID,
                    (mg, g) => new EditionModel { EditionId = g.EditionID, Name = g.Name, Description = g.Description }).ToList()
            }).ToList();
            return result;
        }

        private List<MovieDetailsModel> GetMoviesTittle(string title) {
            return GetAllMovies().Where(movie => movie.Title.ToLower().Contains(title.ToLower())).ToList();
        }

        public static int GetScheduleType(DateTime beginShowTime, int duration) {
            if (beginShowTime.AddDays(duration) >= DateTime.Now) {
                if (beginShowTime <= DateTime.Now && DateTime.Now <= beginShowTime.AddDays(duration)) return 1;
                if (beginShowTime <= DateTime.Now.AddDays(7) && beginShowTime > DateTime.Now) return 2;
                if (beginShowTime > DateTime.Now.AddDays(7)) return 3;
            }
            return 0;
        }

        //private List<MovieDetails> GetMoviesByGenre(IEnumerable<MovieDetails> movieList, string genreId) {
        //    var movies = movieList.Where(movie => movie.Genres.Any(x => x.GenreId == genreId)).ToList();
        //    return movies;
        //}

        //private List<MovieDetails> GetMoviesByEdition(IEnumerable<MovieDetails> movieList, string editionId) {
        //    var movies = movieList.Where(movie => movie.Editions.Any(m => m.EditionId == editionId)).ToList();
        //    return movies;
        //}

        //private List<MovieDetails> GetMoviesByCinema(IEnumerable<MovieDetails> movieList, string cinemaId) {
        //    var movies = movieList.Join(_db.Cine_MovieDetails.Where(x => x.Cinema.CinemaID == cinemaId), m => m.MovieId, c => c.MovieID, (movie, cine) => new {
        //        Movie = movie,
        //        Cinema = cine
        //    }).Select(x => new MovieDetails {
        //        MovieId = x.Movie.MovieId,
        //        Title = x.Movie.Title,
        //        Description = x.Movie.Description,
        //        Director = x.Movie.Director,
        //        Actors = x.Movie.Actors,
        //        AgeLimit = x.Movie.AgeLimit,
        //        Available = x.Movie.Available,
        //        Length = x.Movie.Length,
        //        ReleasedDate = x.Movie.ReleasedDate,
        //        BeginShowDate = x.Movie.BeginShowDate,
        //        ThumbnailUrl = x.Movie.ThumbnailUrl,
        //        TrailerUrl = x.Movie.TrailerUrl,
        //        ScheduleType = x.Movie.ScheduleType,
        //        Genres = x.Movie.Genres,
        //        Editions = x.Movie.Editions
        //    }).ToList();
        //    return movies;
        //} 
        #endregion
    }
}