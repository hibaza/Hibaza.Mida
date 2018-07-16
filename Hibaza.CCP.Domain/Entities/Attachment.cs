using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Attachment
    {
        public string id { get; set; } //unique, format: business_channel_url
        public string business_id { get; set; }
        public string channel_id { get; set; }
        public string attachment_id { get; set; }
        public string attachment_url { get; set; }
        public string type { get; set; } //image, audio, video, file
        public string tag { get; set; }   //product sku, category
        public string target { get; set; } = "facebook"; //facebook, zalo
        public string source_url { get; set; }
        public long timestamp { get; set; }
    }
}
