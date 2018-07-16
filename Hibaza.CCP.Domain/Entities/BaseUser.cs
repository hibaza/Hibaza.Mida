using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class BaseUser : BaseEntity
    {
        public string ext_id { get; set; }
        public string global_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string avatar { get; set; }
        public string phone { get; set; }
        public bool archived { get; set; }
        //public string channel_id { get; set; }
        public string business_id { get; set; }
    }
}

