using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface IMemberRepository: IDisposable  {
        IEnumerable<Member> GetAllMembers();
        IEnumerable<Member> GetMembersByRole(int roleId);
        Member GetMemberByID(int memberId);
        Member GetMemberByEmailAndPassword(string email, string password);
        bool InsertMember(Member member);
        bool UpdateMember(Member member);
        bool DeleteMember(int memberId);
        bool IsEmailExisted(string email);
        bool IsIdCardExisted(string idCardNumber);
        bool Authenticate(string email, string password);
    }
}
