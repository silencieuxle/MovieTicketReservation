using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Controllers {
	public class AdminController : Controller {
        readonly private MoviesDbDataContext _db = new MoviesDbDataContext();
		// GET: Admin
		public ActionResult Index() {
			return View();
		}

		public ActionResult GetPage(string page) {
			switch (page) {
				case "moviestats_overall":
					return PartialView("_MovieStats_Overall");
				case "moviestats_view":
					return PartialView("_MovieStats_View");
				case "moviestats_ticket":
					return PartialView("_MovieStats_Ticket");
				case "ticketstats_overall":
					return PartialView("_TicketStats_Overall");
				case "ticketstats_movie":
					return PartialView("_TicketStats_Movie", _db.Movies);
				case "ticketstats_showtime":
					return PartialView("_TicketStats_Showtime");
				case "ticketstats_room":
					return PartialView("_TicketStats_Room");
				case "systemstats":
					return PartialView("_SystemStats");
				case "managemovie_all":
					return PartialView("_ManageMovie_All");
				case "manageschedule_create":
					return PartialView("_ManageSchedule_Create");
				case "manageschedule_all":
					return PartialView("_ManageSchedule_All");
				case "manageschedule_edit":
					return PartialView("_ManageSchedule_Edit");
				case "mamangemember_all":
					return PartialView("_ManageMember_All");
				case "managemember_add":
					return PartialView("_ManageMember_Add");
				case "managemember_edit":
					return PartialView("_ManageMember_Edit");
				case "promotion_all":
					return PartialView("_Promotion_All");
				case "promotion_add": 
					return PartialView("_Promotion_Add");
				case "promotion_edit":
					return PartialView("_Promotion_Edit");;
				default: return View("Index");
			}
		}

        public ActionResult AjaxGetMovieList() {
            return Json(_db.Movies.Select(x => new { x.Title, x.MovieID }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxTicketStats_Movie(int movieId) {
            var data = _db.Seat_ShowDetails.Where(x => x.Schedule.Cine_MovieDetail.MovieID == movieId && x.Reserved == true).GroupBy(a => a.BookingHeader.ReservedTime)
                .Select(s => new { 
                    count = s.Count(),
                    byDate = s.Key
                });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}