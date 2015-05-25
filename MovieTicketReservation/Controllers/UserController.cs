using System;
using System.Web.Mvc;
using MovieTicketReservation.App_Code;
using MovieTicketReservation.Models;
using MovieTicketReservation.ViewModels;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.MemberService;
using System.Globalization;

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
            MemberModel memberModel = new MemberModel {
                Address = member.Address,
                AvatarURL = member.AvatarURL,
                Birthday = member.Birthday,
                Email = member.Email,
                Firstname = member.Firstname,
                Gender = member.Gender,
                IdCardNumber = member.IDCardNumber,
                Lastname = member.Lastname,
                Password = member.Password,
                Phone = member.Phone
            };

            return View(memberModel);
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
            var userDuplicated = memberRepository.IsIdCardExisted(userRegisterModel.IdCardNumber);
            if (userDuplicated) {
                ModelState.AddModelError("IdCardNumber", "SỐ chứng minh nhân dân này đã được sử dụng.");
                return View(userRegisterModel);
            }
            var user = new Member {
                Firstname = userRegisterModel.FirstName,
                Lastname = userRegisterModel.LastName,
                IDCardNumber = userRegisterModel.IdCardNumber,
                Email = userRegisterModel.Email,
                Phone = userRegisterModel.Phone,
                Password = Helper.GenerateSHA1String(userRegisterModel.Password),
                Address = userRegisterModel.Address,
                Gender = userRegisterModel.Gender,
                Birthday = userRegisterModel.BirthDay,
                AvatarURL = null
            };
            if (memberRepository.InsertMember(user)) {
                return View("RegisterSucceeded");
            }

            return View(userRegisterModel);
        }

        [HttpPost]
        public ActionResult UpdateProfile(Member member) {
            if (!ModelState.IsValid) return View(member);
            memberRepository.UpdateMember(member);
            return View("/User/");
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

        [HttpPost]
        public ActionResult AjaxUpdateBasicInfo(string firstName, string lastName, bool gender, string birthday) {
            string errorMessage = "";
            var member = memberRepository.GetMemberByID((int)Session["UID"]);
            member.Firstname = firstName;
            member.Lastname = lastName;
            member.Gender = gender;
            var temp = DateTime.ParseExact(birthday, "dd/MM/yyyy", new CultureInfo("en-US"));
            try {
                member.Birthday = DateTime.ParseExact(birthday, "dd/MM/yyyy", new CultureInfo("en-US"));
            } catch (Exception ex) {
                Console.Write(ex.StackTrace);
                errorMessage = "Ngày sinh không hợp lệ.";
                return Json(new { Success = false, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }
            var result = memberRepository.UpdateMember(member);
            if (!result) errorMessage = "Có lỗi khi cập nhật dữ liệu";
            return Json(new { Success = result, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxUpdatePassword(string oldPassword, string newPassword) {
            string errorMessage = "";
            var member = memberRepository.GetMemberByID((int)Session["UID"]);
            if (member.Password != Helper.GenerateSHA1String(oldPassword)) {
                errorMessage = "Mật khẩu cũ không đúng";
                return Json(new { Success = false, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }
            member.Password = Helper.GenerateSHA1String(newPassword);
            var result = memberRepository.UpdateMember(member);
            if (!result) errorMessage = "Có lỗi khi cập nhật dữ liệu";
            return Json(new { Success = result, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AjaxUpdateContactInfo(string idCardNumber, string address, string phoneNumber) {
            string errorMessage = "";
            var member = memberRepository.GetMemberByID((int)Session["UID"]);
            member.IDCardNumber = idCardNumber;
            member.Address = address;
            member.Phone = phoneNumber;
            var result = memberRepository.UpdateMember(member);
            if (!result) errorMessage = "Có lỗi khi cập nhật dữ liệu";
            return Json(new { Success = result, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
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