using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{  
    public class Phone 
    {
        public string id { get; set; }
        public string agent_id { get; set; }
        public string business_id { get; set; }
        public string extention { get; set; }
        public string state { get; set; }
        public string ext_id { get; set; }
        public string customer_phone { get; set; }
        public string trunk { get; set; }
        public bool incoming { get; set; }
        public long timestamp { get; set; }
        public string url_audio { get; set; }
        public string channel_id { get; set; }
    }
    public class PhoneAccounts
    {
        public string phone_account_id { get; set; }
        public bool outgoing_enable { get; set; }
        public string outgoing_display_name { get; set; }
        public bool incoming_enable { get; set; }
        public string incoming_extention { get; set; }
        public string phone_status { get; set; }
        public Dictionary<string, string> phone_account_not_using { get; set; }
    }
}
