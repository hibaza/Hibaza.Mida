﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Facebook
{
    public class FacebookConversationFeed
    {
        public FacebookPaging Paging { get; set; }
        public IEnumerable<FacebookConversation> Data { get; set; }
    }
    
}
