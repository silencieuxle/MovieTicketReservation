using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IPromotionService : IDisposable {
        IEnumerable<Promote> GetPromotions();
        IEnumerable<Promote> GetPromotionsByCinema(string cinemaId);
        IEnumerable<Promote> GetFixedDayOfWeekPromotions();
        IEnumerable<Promote> GetDuratedPromotions();
        IEnumerable<Promote> GetActiveDuratedPromotions();
        Promote GetPromotionByID(int promotionId);
        Promote GetFixedDayOfWeekPromotionByDay(int dayOfWeek);
        Promote GetPromotionByScheduleID(int scheduleId);
        bool Insert(Promote promotion);
        bool Delete(int promotionId);
    }
}
