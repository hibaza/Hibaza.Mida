using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Facebook
{
    public class FacebookComment : BaseModel
    {
        public string message { get; set; }
        public FacebookComment parent { get; set; }
        public FacebookUser from { get; set; }
        public FacebookObject @object { get; set; }
        public DateTime created_time { get; set; }
        public FacebookAttachment attachment { get; set; }
        public FacebookCommentFeed comments { get; set; }
        public string permalink_url { get; set; }        
    }
}
