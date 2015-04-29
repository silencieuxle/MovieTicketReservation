using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Controllers {
    public class NewsController : Controller {
        readonly private MoviesDbDataContext _db = new MoviesDbDataContext();

        [HttpGet]
        public ActionResult Index() {
            ViewBag.Tags = GetTags();
            ViewBag.RecentPost = GetRecentPost();
            return View(GetAllNews());
        }

        public ActionResult Details(int newId) {
            ViewBag.Tags = GetTags();
            ViewBag.PostTags = _db.TagDetails.Where(t => t.NewsID == newId).Select(x => new TagModel { TagId = x.TagID, Name = x.Tag.Name }).ToList();
            ViewBag.RecentPost = GetRecentPost();
            var result = _db.News.FirstOrDefault(n => n.NewsID == newId);
            UpdateViewCount(newId);
            return View(result);
        }

        public ActionResult GetNewsByTagID(int tagId) {
            var newsId = _db.TagDetails.Where(t => t.TagID == tagId).Select(t => t.NewsID);
            var news = _db.News.Where(m => newsId.Any(g => m.NewsID == g)).ToList();
            return PartialView("_NewsTemplate", news);
        }

        private List<New> GetAllNews() {
            return _db.News.ToList();
        }

        private List<TagModel> GetTags() {
            return _db.Tags.Select(x => new TagModel { TagId = x.TagID, Name = x.Name }).ToList();
        }

        private List<PostBasicModel> GetRecentPost() {
            return _db.News.Select(p => new PostBasicModel { PostTitle = p.Title, ThumbnailUrl = p.ThumbnailURL, NewId = p.NewsID }).Take(5).ToList();
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