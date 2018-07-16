using Hibaza.CCP.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{

    public class ThreadModel : BaseModel
    {
        public string ext_id { get; set; }
        public bool archived { get; set; }
        public string status { get; set; }
        public bool unread { get; set; }
        public bool nonreply { get; set; }
        public string read_by { get; set; }
        public long read_at { get; set; }
        public string channel_id { get; set; }
        public string channel_ext_id { get; set; }
        public string channel_type { get; set; }
        public string customer_id { get; set; }
        public string agent_id { get; set; }
        public string referral_id { get; set; }                
        public string owner_id { get; set; }
        public string owner_ext_id { get; set; }
        public string owner_name { get; set; }
        public string owner_avatar { get; set; }
        public string last_message { get; set; }
        public long timestamp { get; set; }
        public string link_ext_id { get; set; }
        public string owner_last_message_ext_id { get; set; }
        public string owner_last_message_parent_ext_id { get; set; }
        public string last_message_id { get; set; }
        public string last_message_parent_ext_id { get; set; }
        public List<string> last_visits { get; set; }
        public string type { get; set; }
        public ThreadModel() { }
        public ThreadModel(Thread thread)
        {
            this.id = thread.id;
            this.type = thread.type;
            this.ext_id = thread.ext_id;
            this.archived = thread.archived;
            this.status = thread.status;
            this.unread = thread.unread;
            this.nonreply = thread.nonreply;
            this.read_at = thread.read_at;
            this.read_by = thread.read_by;
            this.channel_id = thread.channel_id;
            this.channel_ext_id = thread.channel_ext_id;
            this.customer_id = thread.customer_id;
            this.channel_type = thread.channel_type ?? "facebook";
            this.last_visits = string.IsNullOrWhiteSpace(thread.last_visits) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(thread.last_visits);
            this.owner_id = thread.owner_id;
            this.owner_ext_id = thread.owner_ext_id ?? thread.owner_id;
            this.referral_id = thread.link_ext_id;
            this.owner_name = thread.owner_name;
            this.owner_avatar = thread.owner_avatar;
            this.link_ext_id = thread.link_ext_id;
            this.last_message = thread.last_message;
            this.last_message_id = thread.last_message_ext_id;
            this.last_message_parent_ext_id = thread.last_message_parent_ext_id;
            this.owner_last_message_ext_id = thread.owner_last_message_ext_id;
            this.owner_last_message_parent_ext_id = thread.owner_last_message_parent_ext_id;
            this.timestamp = thread.timestamp;
        }
    }
}
