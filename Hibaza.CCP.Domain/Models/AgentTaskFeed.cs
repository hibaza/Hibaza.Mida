using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class AgentTaskFeed
    {
        public Paging Paging { get; set; }
        public List<AgentTaskModel> Data { get; set; }
    }
}
