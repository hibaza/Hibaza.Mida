using Hibaza.CCP.Domain.Models.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class CustomerProfileModel : BaseModel
    {
        public string customer_id { get; set; }
        public bool blocked { get; set; }
        public string last_contacted_since { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string avatar { get; set; }
        public string business_id { get; set; }
        public string profile_ext_url { get; set; }
        public string openlink { get; set; }
        public string sex { get; set; }
        public List<ReferralModel> referrals { get; set; }
        public List<AgentModel> agents { get; set; }
        public List<NoteModel> notes { get; set; }
        public List<TicketModel> tickets { get; set; }
        public List<MessageModel> starred_messages { get; set; }
        public List<MessageModel> last_messages { get; set; }
        public List<string> last_visits { get; set; }
        public PostModel post { get; set; }
        public int weight { get; set; }
        public int height { get; set; }
        public string address { get; set; }
        public List<string> phone_list { get; set; }
        public string real_name { get; set; } = "";
    }
}
