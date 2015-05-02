using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IEditionRepository : IDisposable {
        IEnumerable<MovieEdition> GetMovieEditions();
        MovieEdition GetMovieEditionByID(string editionId);
        bool InsertMovieEdition(MovieEdition movieEdition);
        bool UpdateMovieEdition(MovieEdition movieEdition);
        bool DeleteMovieEdition(string movieEditionId);
    }
}
