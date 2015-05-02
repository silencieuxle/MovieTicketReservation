using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface INewsRepository : IDisposable {
        IEnumerable<News> GetNews();
        IEnumerable<News> GetNewsByTag(int tagId);
        IEnumerable<News> GetNewsByPostedDate(DateTime date);
        News GetNewsByID(int newsId);
        bool InsertNews(News news);
        bool UpdateNews(News news);
        bool DeleteNews(int newsId);
    }
}
