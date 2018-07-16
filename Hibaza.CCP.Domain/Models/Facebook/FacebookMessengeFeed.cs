using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Facebook
{
    public class FacebookMessageFeed
    {
        public FacebookPaging paging { get; set; }
        public IEnumerable<FacebookMessage> data { get; set; }
    }
    
}
