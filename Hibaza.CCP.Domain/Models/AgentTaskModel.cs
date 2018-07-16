using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class AgentTaskModel : BaseModel
    {
        public string AgentId { get; set; }
        public AgentModel Agent { get; set; }
        public IEnumerable<TaskModel> Tasks { get; set; }
    }
}
