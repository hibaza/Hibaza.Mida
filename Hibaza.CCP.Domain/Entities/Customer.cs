using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Customer : BaseUser
    {
        public int key { get; set; }
        public string app_id { get; set; }
        public string status { get; set; }
        public long timestamp { get; set; }
        public string business_name { get; set; }
        public string agent_id { get; set; }
        public string assigned_by { get; set; }
        public DateTime? assigned_at { get; set; }
        public string channel_id { get; set; }
        public bool unread { get; set; }
        public bool nonreply { get; set; }
        public bool open { get; set; }
        public string active_thread { get; set; }
        public string active_ticket { get; set; }
        public List<string> phone_list { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public bool blocked { get; set; }
        public DateTime? birthdate { get; set; }
        public string sex { get; set; }
        public int age { get; set; }
        public int weight { get; set; }
        public int height { get; set; }
        public string real_name { get; set; } = "";
    }
}
