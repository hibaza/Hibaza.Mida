using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Channel : BaseEntity
    {
        public string name { get; set; }
        public string type { get; set; } //facebook page, facebook user, ...
        public string token { get; set; }
        public string ext_id { get; set; }
        public bool active { get; set; }
        public string business_id { get; set; }
        public double key { get; set; }
        public List<string> phones { get; set; }
        public string template_id { get; set; } = "";
        public string secret { get; set; } = "";
    }
}
