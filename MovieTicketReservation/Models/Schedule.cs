//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MovieTicketReservation.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Schedule
    {
        public Schedule()
        {
            this.Seat_ShowDetails = new HashSet<Seat_ShowDetails>();
        }
    
        public int ScheduleID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public int ShowTimeID { get; set; }
        public int Cine_MovieDetailsID { get; set; }
        public string RoomID { get; set; }
    
        public virtual Cine_MovieDetails Cine_MovieDetails { get; set; }
        public virtual Room Room { get; set; }
        public virtual ShowTime ShowTime { get; set; }
        public virtual ICollection<Seat_ShowDetails> Seat_ShowDetails { get; set; }
    }
}
