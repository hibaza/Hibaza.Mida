using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Facebook
{
    public class FacebookImage
    {
        public string url { get; set; }
        public string src { get; set; }
        public string preview_url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public bool render_as_sticker { get; set; }
        public int image_type { get; set; }
        public int max_width { get; set; }
        public int max_height { get; set; }
    }
    public class FacebookMedia
    {
        public FacebookImage image { get; set; }
    }

    public class FacebookAttachment : BaseModel
    {
        public FacebookTarget target { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public FacebookMedia media { get; set; }
        public FacebookAttachmentFeed subattachments { get; set; }
        //public string mime_type { get; set; }
        //public string name { get; set; }
        public FacebookImage image_data { get; set; }

    }
}
