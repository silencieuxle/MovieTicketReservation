﻿using System;
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
using MovieTicketReservation.Services.AgeLimitationService;
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
        private IAgeLimitationRepository ageLimitationRepository;

        public MoviesController() {
            this.movieRepository = new MovieRepository(context);
            this.genreRepository = new GenreRepository(context);
            this.editionRepository = new EditionRepository(context);
            this.cinemaReppsitory = new CinemaRepository(context);
            this.scheduleRepository = new ScheduleRepository(context);
            this.cinemaMovieRepository = new CinemaMovieDetailsRepository(context);
            this.ageLimitationRepository = new AgeLimitationRepository(context);
        }

        // GET: Movies
        [HttpGet]
        public ActionResult Index() {
            // Data to display
            ViewBag.EditionList = editionRepository.GetMovieEditions();
            ViewBag.CinemaList = cinemaReppsitory.GetCinemas();
            ViewBag.GenreList = genreRepository.GetMovieGenres();
            ViewBag.AgeList = ageLimitationRepository.GetAgeLimitations();

            var movies = movieRepository.GetAllMovies().Select(m => new MovieExtendedModel {
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
                MovieEdition = m.MovieEdition,
                MovieGenres = m.MovieGenres,
                MovieID = m.MovieID,
                Subtitle = m.Subtitle,
                MovieLength = m.MovieLength,
                ReleasedDate = m.ReleasedDate,
                EditionID = m.EditionID,
                SubtitleID = m.SubtitleID,
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
            ViewBag.AgeList = ageLimitationRepository.GetAgeLimitations();

            var result = movieRepository.GetMoviesByTitle(query).Select(m => new MovieExtendedModel {
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
                MovieEdition = m.MovieEdition,
                Subtitle = m.Subtitle,
                MovieGenres = m.MovieGenres,
                EditionID = m.EditionID,
                SubtitleID = m.SubtitleID,
                MovieID = m.MovieID,
                MovieLength = m.MovieLength,
                ReleasedDate = m.ReleasedDate,
                ThumbnailURL = m.ThumbnailURL,
                Title = m.Title,
                TrailerURL = m.TrailerURL,
                WideThumbnail = m.WideThumbnail,
                ScheduleType = GetScheduleType(m)
            }).OrderByDescending(i => i.BeginShowDate).ToList(); ;
            return View(result);
        }

        public ActionResult AjaxFilter(string cinemaFilter, string editionFilter, string genreFilter, string scheduleTypeFilter, string rateFilter, string ageFilter) {
            var cinemas = JsonConvert.DeserializeObject<string[]>(cinemaFilter);
            var editions = JsonConvert.DeserializeObject<string[]>(editionFilter);
            var genres = JsonConvert.DeserializeObject<string[]>(genreFilter);
            var schedules = JsonConvert.DeserializeObject<string[]>(scheduleTypeFilter);
            var rates = JsonConvert.DeserializeObject<string[]>(rateFilter);
            var ages = JsonConvert.DeserializeObject<string[]>(ageFilter);

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

            if (rates.Count() != 0) {
                if (rates.Contains("hot")) {
                    movies = movieRepository.GetHotMovies(movies);
                }
                if (rates.Contains("rate")) {
                    movies = movieRepository.GetMoviesByRate(movies);
                }
            }

            if (ages.Count() != 0) {
                movies = movieRepository.GetMoviesByAgeLitmitation(ages, movies);
            }

            movies = movies.OrderByDescending(m => m.BeginShowDate).Select(m => new MovieExtendedModel {
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
                MovieEdition = m.MovieEdition,
                MovieGenres = m.MovieGenres,
                MovieID = m.MovieID,
                Subtitle = m.Subtitle,
                EditionID = m.EditionID,
                SubtitleID = m.SubtitleID,
                MovieLength = m.MovieLength,
                ReleasedDate = m.ReleasedDate,
                ScheduleType = GetScheduleType(m),
                ThumbnailURL = m.ThumbnailURL,
                Title = m.Title,
                TrailerURL = m.TrailerURL,
                WideThumbnail = m.WideThumbnail
            }).ToList();

            return PartialView("_MovieTemplate", movies);
        }

        // GET: Details?ID
        public ActionResult Details(int movieId) {
            var result = movieRepository.GetMovieByID(movieId);
            var movie = new MovieExtendedModel {
                Actors = result.Actors,
                AgeLimitation = result.AgeLimitation,
                AgeLimitationID = result.AgeLimitationID,
                Available = result.Available,
                BeginShowDate = result.BeginShowDate,
                Cine_MovieDetails = result.Cine_MovieDetails,
                Description = result.Description,
                EditionID = result.EditionID,
                SubtitleID = result.SubtitleID,
                Director = result.Director,
                Duration = result.Duration,
                LongDescription = result.LongDescription,
                MovieEdition = result.MovieEdition,
                MovieGenres = result.MovieGenres,
                Subtitle = result.Subtitle,
                MovieID = result.MovieID,
                MovieLength = result.MovieLength,
                ReleasedDate = result.ReleasedDate,
                ThumbnailURL = result.ThumbnailURL,
                Title = result.Title,
                TrailerURL = result.TrailerURL,
                WideThumbnail = result.WideThumbnail,
                ScheduleType = GetScheduleType(result)
            };
            ViewBag.Schedules = GetScheduleByMovieId(movieId);
            return View(movie);
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
                Dates = scheduleRepository.GetSchedulesByCinemaIDAndMovieID(c.CinemaID, movieId)
                    .Where(sc => ((DateTime)sc.Date).Date >= DateTime.Now.Date)
                    .GroupBy(sch => sch.Date, (key, value) => new DateModel {
                        ShowingDate = (DateTime)key,
                        Showtimes = value.Select(sh => sh.ShowTime.StartTime != null ? new Showtime {
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