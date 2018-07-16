using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Node : BaseEntity
    {
        public string business_id { get; set; }
        public string channel_id { get; set; }
        public string data { get; set; }
        public string status { get; set; } = "pending";
        public long timestamp { get; set; }
        public string type { get; set; } //message, comment
    }
}
