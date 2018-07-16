using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class ChannelFeed
    {
        public Paging Paging { get; set; }

        public IEnumerable<Entities.Channel> Data { get; set; }
    }
}
