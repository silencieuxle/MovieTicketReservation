using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieTicketReservation.Models;

namespace MovieTicketReservation.Services {
    public interface ITagRepository : IDisposable {
        IEnumerable<Tag> GetTags();
        Tag GetTagByID(int tagId);

        bool InsertTag(Tag tag);
        bool UpdateTag(Tag tag);
        bool DeleteTag(int tagId);
    }
}
