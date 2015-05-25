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
        Promote GetPromotionByID(int promotionId);
        bool Insert(Promote promotion);
        bool Delete(int promotionId);
    }
}
