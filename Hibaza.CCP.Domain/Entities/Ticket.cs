using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public string business_id { get; set; }
        public string channel_id { get; set; }
        public string customer_id { get; set; }
        public string customer_name { get; set; }
        public string thread_id { get; set; }
        public string order_id { get; set; }
        public int type { get; set; } //sale:0, support:1, promotion:2
        public string description { get; set; }
        public string short_description { get; set; }
        public int status { get; set; } //"pending : 0, attention:1, completed:2, rejected:3";
        public string tags { get; set; }
        public string sender_id { get; set; }
        public string sender_name { get; set; }
        public string sender_avatar { get; set; }
        public long timestamp { get; set; }
    }
}
