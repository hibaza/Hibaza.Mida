using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Facebook
{
    public class FacebookConversation : BaseModel
    {
        public string link { get; set; }
        public string subject { get; set; }
        public string snippet { get; set; }
        public string message_count { get; set; }
        public string unread_count { get; set; }
        public bool can_reply { get; set; }
        public bool is_subscribed { get; set; }
        public DateTime updated_time { get; set; }
        public FacebookMessageFeed messages { get; set; }
        public FacebookUserFeed senders { get; set; }
    }
}
