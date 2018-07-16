using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Facebook
{
    public class FacebookPost : BaseModel
    {
        public string parent_id { get; set; }
        public string object_id { get; set; }
        public string link { get; set; }
        public string permalink_url { get; set; }
        public string message { get; set; }
        public string type { get; set; } // link, status, photo, video, offer
        public string source { get; set; }
        public string picture { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
        public FacebookUser from { get; set; }
        public FacebookAttachmentFeed attachments { get; set; }
    }
}
