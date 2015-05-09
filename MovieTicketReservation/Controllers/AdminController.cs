using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.MovieService;
using MovieTicketReservation.Services.MemberService;
using MovieTicketReservation.Services.ScheduleService;
using MovieTicketReservation.Services.ShowtimeService;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.ViewModels;

namespace MovieTicketReservation.Controllers {
    public class AdminController : Controller {
        private DbEntities context = new DbEntities();
        private IMovieRepository movieRepository;
        private IScheduleRepository scheduleRepository;
        private IMemberRepository memberRepository;
        private IShowtimeRepository showtimeRepository;

        public AdminController() {
            this.movieRepository = new MovieRepository(context);
            this.scheduleRepository = new ScheduleRepository(context);
            this.memberRepository = new MemberRepository(context);
            this.showtimeRepository = new ShowtimeRepository(context);
        }

        // GET: Admin
        public ActionResult Index() {
            return View();
        }

        public ActionResult GetPage(string page, string[] parameters) {
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
                    return PartialView("_TicketStats_Movie", movieRepository.GetAllMovies());
                case "ticketstats_showtime":
                    return PartialView("_TicketStats_Showtime", showtimeRepository.GetShowtimes());
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
                    return PartialView("_ManageMember_All", memberRepository.GetAllMembers());
                case "managemember_add":
                    return PartialView("_ManageMember_Add");
                case "managemember_edit":
                    return PartialView("_ManageMember_Edit", memberRepository.GetMemberByID(Convert.ToInt32(parameters[0])));
                case "promotion_all":
                    return PartialView("_Promotion_All");
                case "promotion_add":
                    return PartialView("_Promotion_Add");
                case "promotion_edit":
                    return PartialView("_Promotion_Edit"); ;
                default: return View("Index");
            }
        }

        #region Member Management

        public ActionResult AjaxGetMembers(int headIndex, int type) {
            var result = memberRepository.GetAllMembers();
            int index = 0;
            switch (type) {
                case 0:
                    result = result.Skip(headIndex - 10).Take(10).ToList();
                    index = headIndex -10;
                    break;
                case 1:
                    result = result.Skip(headIndex + 10).Take(10).ToList();
                    index = headIndex - 10;
                    break;
                default:
                    result = result.Skip(headIndex).Take(10).ToList();
                    index = headIndex;
                    break;
            }
            return Json(new { Data = result, HeadIndex = index }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateMember(Member member) {
            return null;
        }

        public ActionResult AjaxAddMember(UserRegisterModel userRegisterModel) {
            if (!ModelState.IsValid) return Json(new { Success = false, ErrorMessage = "Model state is invalid" });
            var userDuplicated = memberRepository.IsIdCardExisted(userRegisterModel.IdCardNumber);
            if (userDuplicated) {
                ModelState.AddModelError("IdCardNumber", "SỐ chứng minh nhân dân này đã được sử dụng.");
                return View(userRegisterModel);
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
                RoleID = 4
            });
            return Json(new { Success = result, ErrorMessage = result ? "" : "Error occured while process your request." }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}