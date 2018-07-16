using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Business : BaseEntity
    {
        public string name { get; set; }
        public string type { get; set; } //brand, shop, company
        public string token { get; set; }
        public string logo { get; set; }
        public string ext_id { get; set; }
        public bool active { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string zip { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public bool auto_like { get; set; } = true;
        public bool auto_hide { get; set; } = true;
        public bool auto_assign { get; set; } = true;
        public bool auto_ticket { get; set; } = true;
        public bool auto_message { get; set; } = true;
        public double key { get; set; }
        public int phone_limit { get; set; } = 0;
        public string phone_incoming { get; set; } = "";
        public bool hibaza_using { get; set; } = true;
        public string token_client { get; set; } = "";

    }
}
