using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Conversation : BaseEntity
    {
        public string ext_id { get; set; }
        public string owner_ext_id { get; set; }
        public string owner_app_id { get; set; }
        public string link { get; set; }
        public long timestamp { get; set; }
        public string status { get; set; } = "pending";
        public string business_id { get; set; }
        public string channel_id { get; set; }
    }
}
