using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Note : BaseEntity
    {
        public string business_id { get; set; }
        public string customer_id { get; set; }
        public string customer_name { get; set; }
        public string thread_id { get; set; }
        public string type { get; set; } //customer, thread, ticket, agent
        public string text { get; set; }
        public bool featured { get; set; }
        public string sender_id { get; set; }
        public string sender_name { get; set; }
        public string sender_avatar { get; set; }
    }
}
