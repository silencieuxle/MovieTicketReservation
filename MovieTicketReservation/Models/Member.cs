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
    using System.ComponentModel.DataAnnotations;
    
    public partial class Member
    {
        public Member()
        {
            this.BookingHeaders = new HashSet<BookingHeader>();
            this.AdminAccounts = new HashSet<AdminAccount>();
        }
    
        public int MemberID { get; set; }
        public string IDCardNumber { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public Nullable<bool> Gender { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Birthday { get; set; }
        public string AvatarURL { get; set; }
    
        public virtual ICollection<BookingHeader> BookingHeaders { get; set; }
        public virtual ICollection<AdminAccount> AdminAccounts { get; set; }
    }
}
