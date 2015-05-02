using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.CinemaService;
using MovieTicketReservation.Services.CinemaImageService;
using Newtonsoft.Json;
    
namespace MovieTicketReservation.Controllers {
    public class CinemasController : Controller {
        private CinemaRepository cinemaRepository;
        private CinemaImageRepository cinemaImageRepository;
        private DbEntities context = new DbEntities();

        public CinemasController() {
            this.cinemaRepository = new CinemaRepository(context);
            this.cinemaImageRepository = new CinemaImageRepository(context);
        }
        // GET: Cinema
        public ActionResult Index() {
            return View(cinemaRepository.GetCinemas());
        }

        public ActionResult GetCinemaDetails(string cinemaId) {
            var cinemaInfo = cinemaRepository.GetCinemaByID(cinemaId);
            var cinemaImage = cinemaImageRepository.GetCinemaImagesByCinemaID(cinemaId).Select(x => x.ImageURL);

            var result = new {
                CinemaName = cinemaInfo.Name,
                CinemaAddress = cinemaInfo.Address,
                CinemaPhone = cinemaInfo.Phone,
                Images = JsonConvert.SerializeObject(cinemaImage)
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}