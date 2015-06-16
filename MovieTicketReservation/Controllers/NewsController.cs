using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieTicketReservation.Models;
using MovieTicketReservation.ViewModels;
using MovieTicketReservation.Services;
using MovieTicketReservation.Services.NewsRepository;
using MovieTicketReservation.Services.TagRepository;

namespace MovieTicketReservation.Controllers {
    public class NewsController : Controller {
        private INewsRepository newsRepository;
        private ITagRepository tagRepository;
        private DbEntities context = new DbEntities();

        public NewsController() {
            this.newsRepository = new NewsRepository(context);
            this.tagRepository = new TagRepository(context);
        }

        [HttpGet]
        public ActionResult Index() {
            var newsList = newsRepository.GetNews();
            ViewBag.Tags = tagRepository.GetTags();
            ViewBag.RecentPost = newsRepository.GetNews().OrderByDescending(n => n.PostedDate).Take(5).ToList();
            return View(newsList);
        }

        public ActionResult Details(int newId) {
            var news = newsRepository.GetNewsByID(newId);
            ViewBag.RecentPost = newsRepository.GetNews().OrderByDescending(n => n.PostedDate).Take(5).ToList();
            ViewBag.Tags = tagRepository.GetTags();
            ViewBag.PostTags = news.Tags.ToList();
            UpdateViewCount(newId);
            return View(news);
        }

        public ActionResult GetNewsByTagID(int tagId) {
            var news = newsRepository.GetNewsByTag(tagId).ToList();
            return PartialView("_NewsTemplate", news);
        }

        private bool UpdateViewCount(int postId) {
            var post = newsRepository.GetNewsByID(postId);
            if (post == null) return false;
            post.ViewCount += 1;
            newsRepository.UpdateNews(post);
            return true;
        }
    }
}