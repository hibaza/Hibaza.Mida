using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Agent : BaseUser
    {
        public string username { get; set; }
        public string password { get; set; }
        public string password_confirmation { get; set; }
        public bool active { get; set; }
        public bool locked { get; set; } = false;
        public string status { get; set; } //online, offline, busy, locked
        public string login_status { get; set; } //online, offline, locked
        public DateTime? last_loggedin_time { get; set; } = DateTime.MinValue;
        public DateTime? last_loggedout_time { get; set; } = DateTime.MinValue;
        public DateTime? last_acted_time { get; set; } = DateTime.MinValue;
        public string facebook_access_token { get; set; }
        public string role { get; set; }
        public string channel_id { get; set; }
        //public string channel_type { get; set; }
        public string business_name { get; set; }
        public double key { get; set; }
        public string phone_account_id { get; set; }
        public string phone_status { get; set; }
        
    }
}
