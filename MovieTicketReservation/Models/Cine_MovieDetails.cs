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
    
    public partial class Cine_MovieDetails
    {
        public Cine_MovieDetails()
        {
            this.Schedules = new HashSet<Schedule>();
        }
    
        public int DetailsID { get; set; }
        public int MovieID { get; set; }
        public string CinemaID { get; set; }
        public Nullable<int> Duration { get; set; }
        public Nullable<System.DateTime> BeginShowDate { get; set; }
    
        public virtual Cinema Cinema { get; set; }
        public virtual Movie Movie { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
