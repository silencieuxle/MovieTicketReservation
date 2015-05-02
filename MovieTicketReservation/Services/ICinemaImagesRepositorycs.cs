using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface ICinemaImagesRepositorycs : IDisposable {
        IEnumerable<CinemaImage> GetCinemaImages();
        IEnumerable<CinemaImage> GetCinemaImagesByCinemaID(string cinemaId);
        CinemaImage GetCinemaImageByID(int imageId);
        bool InsertCinemaImage(CinemaImage cinemaImage);
        bool UpdateCinemaImage(CinemaImage cinemaImage);
        bool DeleteCinemaImage(int cinemaImageId);
    }
}
