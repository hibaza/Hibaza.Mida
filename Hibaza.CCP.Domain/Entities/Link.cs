using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Link : BaseEntity
    {
        public string business_id { get; set; }
        public string channel_id { get; set; }
        public string channel_ext_id { get; set; }
        public string message { get; set; }
        public string objectId { get; set; }
        public string url { get; set; }
        public string author_id { get; set; }
        public string author_name { get; set; }
        public string status { get; set; } = "pending";
        public long timestamp { get; set; }
        public string type { get; set; } //post, photo, album, comment, message, product, ...
    }
}
