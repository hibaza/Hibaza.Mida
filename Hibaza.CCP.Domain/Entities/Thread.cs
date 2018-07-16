using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Thread : BaseEntity
    {
        public string ext_id { get; set; }
        public string channel_ext_id { get; set; }
        public bool archived { get; set; }
        public string status { get; set; }
        public bool unread { get; set; } = true;
        public bool nonreply { get; set; } = true;
        public long read_at { get; set; }
        public string read_by { get; set; }
        public string type { get; set; } //message, comment
        public string business_id { get; set; }
        public string channel_id { get; set; }
        public string channel_type { get; set; }
        public string agent_id { get; set; }
        public string customer_id { get; set; }
        public string link_ext_id { get; set; }
        public string owner_id { get; set; }
        public string owner_ext_id { get; set; }
        public string owner_app_id { get; set; }
        public string owner_name { get; set; }
        public string owner_avatar { get; set; }
        public long owner_timestamp { get; set; }
        public string owner_last_message_ext_id { get; set; }
        public string owner_last_message_parent_ext_id { get; set; }
        public string last_message { get; set; }
        public string last_message_ext_id { get; set; }
        public string last_message_parent_ext_id { get; set; }
        public string sender_id { get; set; }
        public string sender_ext_id { get; set; }
        public string sender_name { get; set; }
        public string sender_avatar { get; set; }
        public long timestamp { get; set; }
        public string last_visits { get; set; }

        //public string flag_timestamp { get; set; }
        //public string channel_flag_timestamp { get; set; }
        //public string channel_agent_flag_timestamp { get; set; }
        //public string agent_flag_timestamp { get; set; }

        //public string channel_timestamp { get; set; }
        //public string agent_timestamp { get; set; }
        //public string status_timestamp { get; set; }
        //public string channel_agent_timestamp { get; set; }
        //public string channel_status_timestamp { get; set; }
        //public string channel_agent_status_timestamp { get; set; }
        //public string agent_status_timestamp { get; set; }

    }
}
