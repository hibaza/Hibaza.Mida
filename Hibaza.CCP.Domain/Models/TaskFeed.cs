using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class TaskFeed
    {
        public Paging Paging { get; set; }
        public IEnumerable<TaskModel> Data { get; set; }
    }
}
