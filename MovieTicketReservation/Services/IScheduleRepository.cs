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
        IEnumerable<Schedule> GetSchedulesByRoomID(string roomId);
        Schedule GetScheduleByID(int scheduleId);
        bool InsertSchedule(Schedule schedule);
        bool UpdateSchedule(Schedule schedule);
        bool DeleteSchedule(int scheduleId);
    }
}
