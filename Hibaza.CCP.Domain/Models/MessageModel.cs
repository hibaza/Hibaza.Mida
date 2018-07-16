using Hibaza.CCP.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class MessageModel : BaseModel
    {
        public string created_time { get; set; }
        public DateTime? updated_time { get; set; }
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
        public bool liked { get; set; }
        public bool hidden { get; set; }
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
        public List<string> urls { get; set; }
        public string tag { get; set; }
        public string template { get; set; }
        public List<string> titles { get; set; }
        public string permalink
        {
            get
            {
                return string.Format("/{0}/messages/openlink/?item_id={1}", business_id, id);
            }
            set { }
        }
        public MessageModel() { }
        public MessageModel(Message m)
        {
            var titless = string.IsNullOrWhiteSpace(m.titles) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(m.titles);
            var urlss= string.IsNullOrWhiteSpace(m.urls) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(m.urls);

            id = m.id;
            parent_id = m.parent_id;
            parent_ext_id = m.parent_ext_id;
            root_ext_id = m.root_ext_id;
            created_time = m.created_time.ToString("hh:mm dd/MM/yyyy");
            updated_time = m.updated_time;
            ext_id = m.ext_id;
            url = m.url;
            urls = urlss;
            file_name = m.file_name;
            size = m.size;
            subject = m.subject;
            message =(urlss.Count==1 && titless.Count==1 )? titless[0] :  m.message;
            agent_id = m.agent_id;
            thread_id = m.thread_id;
            thread_type = m.thread_type;
            conversation_ext_id = m.conversation_ext_id;
            sender_id = m.sender_id;
            sender_ext_id = m.sender_ext_id;
            sender_avatar = string.IsNullOrWhiteSpace(m.sender_avatar) ? (string.IsNullOrWhiteSpace(m.agent_id) && m.sender_ext_id!=m.channel_ext_id ? "/avatars/customer.png" : "/avatars/agent.png") : m.sender_avatar;
            sender_name = string.IsNullOrWhiteSpace(m.sender_name) ? "?" : m.sender_name;
            recipient_avatar = m.recipient_avatar;
            recipient_ext_id = m.recipient_ext_id;
            recipient_id = m.recipient_id;
            recipient_name = m.recipient_name;
            author = m.author;
            type = m.type;
            read = m.read;
            deleted = m.deleted;
            liked = m.liked;
            hidden = m.hidden;
            starred = m.starred;
            customer_id = m.customer_id;
            owner_id = m.owner_id;
            timestamp = m.timestamp;
            business_id = m.business_id;
            channel_ext_id = m.channel_ext_id;
            channel_id = m.channel_id;
            channel_type = m.channel_type;
            replied = m.replied;
            replied_at = m.replied_at;
            tag = m.tag;
            template = m.template;
            titles = titless;
        }
    }
}
