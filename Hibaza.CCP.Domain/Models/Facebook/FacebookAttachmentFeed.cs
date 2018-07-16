using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Facebook
{
    public class FacebookAttachmentFeed
    {
        public FacebookPaging paging { get; set; }
        public IEnumerable<FacebookAttachment> data { get; set; }
    }
}
