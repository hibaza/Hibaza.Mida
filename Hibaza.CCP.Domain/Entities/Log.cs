using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Log : BaseEntity
    {
        public int key { get; set; }
        public string business_id { get; set; }
        public string link { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public string message { get; set; }
        public string details { get; set; }
    }
}
