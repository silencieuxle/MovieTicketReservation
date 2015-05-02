using System;
using System.Web.Mvc;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.Models;
using MovieTicketReservation.ViewModels;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.MemberService;

namespace MovieTicketReservation.Controllers {
    public class UserController : Controller {
        private DbEntities context = new DbEntities();
        private IMemberRepository memberRepository;

        public UserController() {
            this.memberRepository = new MemberRepository(context);
        }

        // GET: User
        [HttpGet]
        public ActionResult Index() {
            if (Session["UID"] == null) {
                Session["RedirectURL"] = Request.RawUrl;
                return Redirect("/User/Login");
            }

            var userId = (int)Session["UID"];
            var member = memberRepository.GetMemberByID(userId);
            return View(member);
        }

        [HttpGet]
        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string currentURL, LoginModel loginModel) {
            if (!ModelState.IsValid) return View(loginModel);
            int userId = 0;
            if ((userId = Authenticate(loginModel.Email, loginModel.Password)) != 0) {
                Session["UID"] = userId;
                string redirectUrl = (String)Session["RedirectURL"];
                Session["RedirectURL"] = null;
                if (!String.IsNullOrEmpty(redirectUrl)) return Redirect(redirectUrl);
                else return Redirect("/Home/");
            } else {
                return View("Login");
            }
        }

        public ActionResult LogOff() {
            Session.Abandon();
            return Redirect("/Home/");
        }

        [HttpGet]
        public ActionResult Register() {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserRegisterModel userRegisterModel) {
            if (!ModelState.IsValid) return View(userRegisterModel);
            string firstName = userRegisterModel.FirstName;
            string lastName = userRegisterModel.LastName;
            string idCardNumber = userRegisterModel.IdCardNumber;
            string email = userRegisterModel.Email;
            string password = userRegisterModel.Password;
            string phone = userRegisterModel.Phone;
            string address = userRegisterModel.Address;

            var userDuplicated = memberRepository.IsIdCardExisted(idCardNumber);
            if (userDuplicated) return View("DuplicateIDCard");
            var user = new Member {
                Firstname = firstName,
                Lastname = lastName,
                IDCardNumber = idCardNumber,
                Email = email,
                Phone = phone,
                Password = Helper.GenerateSHA1String(password),
                Address = address
            };
            if (memberRepository.InsertMember(user)) {
                return View("RegisterSucceeded");
            }

            return View(userRegisterModel);
        }

        #region Ajax methods
        public ActionResult AjaxCheckEmail(string email) {
            if (memberRepository.IsEmailExisted(email))
                return Json(new { Success = false, ErrorMessage = "Email đã được sử dụng." }, JsonRequestBehavior.AllowGet);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxCheckIdCard(string idCardNumber) {
            if (memberRepository.IsIdCardExisted(idCardNumber))
                return Json(new { Success = false, ErrorMessage = "Số chứng minh nhân dân đã được sử dụng." }, JsonRequestBehavior.AllowGet);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxAuthenticate(LoginModel loginModel) {
            if (!ModelState.IsValid)
                return Json(new { Success = false, ErrorMessage = "Thông tin đăng nhập không đầy đủ." });
            int userId = 0;
            if ((userId = Authenticate(loginModel.Email, loginModel.Password)) != 0)
                return Json(new { Success = true, ErrorMessage = "" });
            return Json(new { Success = false, ErrorMessage = "Sai thông tin đăng nhập." });
        }
        #endregion

        private int Authenticate(string email, string password) {
            var pword = Helper.GenerateSHA1String(password);
            var user = memberRepository.Authenticate(email, pword);
            if (user == false) return 0;
            return memberRepository.GetMemberByEmailAndPassword(email, pword).MemberID;
        }
    }
}