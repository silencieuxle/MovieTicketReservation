using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Controllers {
    public class NewsController : Controller {
        readonly private MoviesDbDataContext _db = new MoviesDbDataContext();
        // GET: News
        public ActionResult Index() {
            ViewBag.RecentPost = GetRecentPost();
            return View(_db.News.ToList());
        }

        public ActionResult Details(int newId) {
            ViewBag.RecentPost = GetRecentPost();
            var result = _db.News.FirstOrDefault(n => n.NewsID == newId);
            UpdateViewCount(newId);
            return View(result);
        }

        private List<PostModel> GetRecentPost() {
            return _db.News.Where(x => ((DateTime)x.PostedDate) >= DateTime.Now.AddDays(-1))
                           .Select(p => new PostModel { PostTitle = p.Title, ThumbnailUrl = p.ThumbnailURL, NewId = p.NewsID }).ToList();
        }

        private bool UpdateViewCount(int postId) {
            _db.Connection.Open();
            _db.Transaction = _db.Connection.BeginTransaction();
            try {
                var post = _db.News.FirstOrDefault(p => p.NewsID == postId);
                if (post == null) return false;
                post.ViewCount += 1;
                _db.SubmitChanges();
                _db.Transaction.Commit();
            } catch (Exception e) {
                _db.Transaction.Rollback();
                return false;
            } finally {
                _db.Connection.Close();
            }
            return true;
        }
    }
}