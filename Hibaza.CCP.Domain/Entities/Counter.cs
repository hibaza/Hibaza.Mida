using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Counter
    {
        public string id { get; set; }
        public int count { get; set; }
        public int unread { get; set; }
    }
}
