using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class ApiResponse
    {
        public bool ok { get; set; } = false;
        public dynamic data { get; set; } = null;
        public string view { get; set; }
        public string msg { get; set; }
    }
}
