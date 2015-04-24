using MovieTicketReservation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using MovieTicketReservation.App_Code;

namespace MovieTicketReservation.Controllers {
    public class UserController : Controller {
        readonly private MoviesDbDataContext _db = new MoviesDbDataContext();
        // GET: User
        [HttpGet]
        public ActionResult Index() {
            var userId = (int)Session["UID"];
            var user = _db.Members.FirstOrDefault(u => u.MemberID == userId);
            return View(user);
        }

        [HttpGet]
        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate(LoginModel loginModel) {
            if (!ModelState.IsValid) return Json(new { Success = false, ErrorMessage = "Invalid login info." });
            int userId = 0;
            if ((userId = Authenticate(loginModel.Email, loginModel.Password)) != 0) {
                return Json(new { Success = true, ErrorMessage = "" });
            }
            return Json(new { Success = false, ErrorMessage = "Wrong login info." });
        }

        [HttpPost]
        public ActionResult Login(string currentURL, LoginModel loginModel) {
            if (!ModelState.IsValid) return View();
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
            if (!ModelState.IsValid) return View();
            string firstName = userRegisterModel.FirstName;
            string lastName = userRegisterModel.LastName;
            string idCardNumber = userRegisterModel.IdCardNumber;
            string email = userRegisterModel.Email;
            string password = userRegisterModel.Password;
            string phone = userRegisterModel.Phone;
            string address = userRegisterModel.Address;

            _db.Connection.Open();
            _db.Transaction = _db.Connection.BeginTransaction();
            try {
                var userDuplicated = _db.Members.Any(u => u.IDCardNumber == idCardNumber);
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

                _db.Members.InsertOnSubmit(user);
                _db.SubmitChanges();
                _db.Transaction.Commit();
            } catch (Exception e) {
                _db.Transaction.Rollback();
                return null;
            } finally {
                _db.Connection.Close();
            }
            return View("RegisterSucceeded");
        }

        #region Email and ID Card Number validation
        public ActionResult CheckEmail(string email) {
            var emailRegistered = _db.Members.Any(u => u.Email == email);
            if (emailRegistered) return Json(new { Success = false, Error = "Email is used" }, JsonRequestBehavior.AllowGet);
            else return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckIdCard(string idCardNumber) {
            var idCardRegistered = _db.Members.Any(u => u.IDCardNumber == idCardNumber);
            if (idCardRegistered) return Json(new { Success = false, Error = "ID Card Number is used" }, JsonRequestBehavior.AllowGet);
            else return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        private int Authenticate(string email, string password) {
            var pword = Helper.GenerateSHA1String(password);
            var user = _db.Members.FirstOrDefault(m => m.Email == email && m.Password == pword);
            if (user == null) return 0;
            return user.MemberID;
        }
    }
}