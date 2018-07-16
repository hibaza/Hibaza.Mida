using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Web.Models
{
    public class ShortCutAddEdit {
        public bool edit { get; set; }
        public Shortcut data { get; set; }
    }

    public class BusinessSettings
    {
        public bool admin { get; set; }
        public string user_id { get; set; }
        public string id { get; set; }
        public Business Data { get; set; }
    }
}
