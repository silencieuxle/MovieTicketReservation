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
    
    public partial class TicketClass
    {
        public TicketClass()
        {
            this.Seat_ShowDetails = new HashSet<Seat_ShowDetails>();
        }
    
        public string ClassID { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Price { get; set; }
    
        public virtual ICollection<Seat_ShowDetails> Seat_ShowDetails { get; set; }
    }
}
