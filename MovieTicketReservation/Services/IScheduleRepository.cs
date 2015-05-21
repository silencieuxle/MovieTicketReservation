using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IScheduleRepository : IDisposable {
        IEnumerable<Schedule> GetSchedules();
        IEnumerable<Schedule> GetSchedulesByDate(DateTime date);
        IEnumerable<Schedule> GetCanBeReserveScheduleByMovieID(int movieId);
        IEnumerable<Schedule> GetSchedulesByRoomID(string roomId);
        IEnumerable<Schedule> GetSchedulesByCinemaIDAndMovieID(string cinemaId, int movieId);
        IEnumerable<Schedule> GetAvailableSchedules();
        Schedule GetScheduleByID(int scheduleId);
        Int32 InsertSchedule(Schedule schedule);
        bool UpdateSchedule(Schedule schedule);
        bool DeleteSchedule(int scheduleId);
    }
}
