using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class MessageFormData
    {
        public string tag { get; set; }
        public string description { get; set; }
        public string message { get; set; }
        public string recipient_id { get; set; }
        public string image_flags { get; set; }
        public string image_urls { get; set; }
        public string business_id { get; set; }
        public string channel_id { get; set; }
        public string agent_id { get; set; }
        public string thread_id { get; set; }
        public string ticket_id { get; set; }
        public string titles { get; set; }
        public string button_title { get; set; }
        public string channel_type { get; set; }
        public string channel_format { get; set; }
        public string config { get; set; }
        public string para { get; set; }
    }
}
