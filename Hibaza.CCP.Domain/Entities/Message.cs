using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class BaseMessage: BaseEntity
    {
        public string parent_id { get; set; }
        public string root_ext_id { get; set; }
        public string parent_ext_id { get; set; }
        public string ext_id { get; set; }
        public string url { get; set; }
        public string file_name { get; set; }
        public long size { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
        public string agent_id { get; set; }
        public string thread_id { get; set; }
        public string conversation_ext_id { get; set; }
        public string sender_id { get; set; }
        public string sender_ext_id { get; set; }
        public string sender_name { get; set; }
        public string sender_avatar { get; set; }
        public string recipient_id { get; set; }
        public string recipient_ext_id { get; set; }
        public string recipient_avatar { get; set; }
        public string recipient_name { get; set; }
        public string author { get; set; }
        public bool read { get; set; }
        public bool deleted { get; set; }
        public bool liked { get; set; } = true;
        public bool hidden { get; set; } = true;
        public bool starred { get; set; }
        public string customer_id { get; set; }
        public string owner_id { get; set; }
        public string type { get; set; } //message, comment, reply
        public long timestamp { get; set; }
        public string business_id { get; set; }
        public string channel_id { get; set; }
        public string channel_ext_id { get; set; }
        public string thread_type { get; set; }
        public string channel_type { get; set; }
        public bool replied { get; set; }
        public long replied_at { get; set; }
      
    }

    public class Message : BaseMessage
    {
        public string urls { get; set; }
        public string tag { get; set; }
        public string template { get; set; }
        public string titles { get; set; }
        public string extention { get; set; }
        public string state { get; set; }
        public string trunk { get; set; }
        public bool incoming { get; set; }
    }
    public class FbMessage : BaseMessage
    {

    }
}
