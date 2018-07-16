using Hibaza.CCP.Domain.Models.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class PostModel : BaseModel
    {
        public string parent_id { get; set; }
        public string link { get; set; }
        public string permalink_url { get; set; }
        public string message { get; set; }
        public string type { get; set; } // link, status, photo, video, offer
        public string source { get; set; }
        public string picture { get; set; }
        public string sender_id { get; set; }
        public string sender_name { get; set; }
        public string sender_avatar { get; set; }
        public string page_id { get; set; }
        public string page_name { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
        public IEnumerable<FacebookAttachment> attachments { get; set; }
    }
}
