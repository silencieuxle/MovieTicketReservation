using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IMemberRepository<Member>  {
        IEnumerable<Member> GetAll();
        Member GetByID(int id);
        Member Add(Member member);
    }
}
