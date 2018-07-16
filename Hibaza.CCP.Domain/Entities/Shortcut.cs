using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Shortcut : BaseEntity
    {
        public string business_id { get; set; }
        public string shortcut { get; set; }
        public string created_by { get; set; }
        public string name { get; set; }
    }
}
