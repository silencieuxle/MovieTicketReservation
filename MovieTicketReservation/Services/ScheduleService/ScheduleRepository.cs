﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieTicketReservation.Models;
using System.Data.Entity;

namespace MovieTicketReservation.Services.ScheduleService {
    public class ScheduleRepository : IScheduleRepository, IDisposable {
        private DbEntities context;

        public ScheduleRepository(DbEntities context) {
            this.context = context;
        }

        #region IScheduleRepository Members

        public IEnumerable<Schedule> GetSchedules() {
            return context.Schedules.ToList();
        }

        public IEnumerable<Schedule> GetSchedulesByDate(DateTime date) {
            return context.Schedules.Where(s => ((DateTime)s.Date).Date == date.Date).ToList();
        }

        public IEnumerable<Schedule> GetSchedulesByRoomID(string roomId) {
            return context.Schedules.Where(s => s.RoomID == roomId).ToList();
        }

        public IEnumerable<Schedule> GetSchedulesByCinemaIDAndMovieID(string cinemaId, int movieId) {
            return context.Schedules.Where(s => s.Cine_MovieDetails.CinemaID == cinemaId && s.Cine_MovieDetails.MovieID == movieId).ToList();
        }

        public Schedule GetScheduleByID(int scheduleId) {
            return context.Schedules.Find(scheduleId);
        }

        public bool InsertSchedule(Schedule schedule) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Schedules.Add(schedule);

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool UpdateSchedule(Schedule schedule) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    context.Entry(schedule).State = EntityState.Modified;

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public bool DeleteSchedule(int scheduleId) {
            using (var transaction = context.Database.BeginTransaction()) {
                try {
                    var schedule = context.Schedules.Find(scheduleId);
                    context.Schedules.Remove(schedule);

                    context.SaveChanges();

                    transaction.Commit();
                } catch (Exception ex) {
                    Console.Write(ex.StackTrace);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region IDisposable Members

        private bool disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (!this.disposed) {
                if (disposing) {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}