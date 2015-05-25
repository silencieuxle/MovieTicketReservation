using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface ISubtitleRepository : IDisposable {
        IEnumerable<Subtitle> GetSubtitles();
        Subtitle GetSubtitleByID(int subtitleId);
    }
}
