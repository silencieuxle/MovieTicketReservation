using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;
using Newtonsoft.Json;
    
namespace MovieTicketReservation.Controllers {
    public class CinemasController : Controller {
        readonly private MoviesDbDataContext _db = new MoviesDbDataContext();
        // GET: Cinema
        public ActionResult Index() {
            return View(_db.Cinemas.Select(c => new CinemaModel { CinemaId = c.CinemaID, Name = c.Name }).ToList());
        }

        public ActionResult GetCinemaDetails(string cinemaId) {
            var cinemaInfo = _db.Cinemas.FirstOrDefault(c => c.CinemaID == cinemaId);
            var cinemaImage = _db.CinemaImages.Where(i => i.CinemaID == cinemaId).Select(x => x.ImageURL);

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