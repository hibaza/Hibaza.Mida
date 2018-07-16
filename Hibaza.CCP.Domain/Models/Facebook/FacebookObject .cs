using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Facebook
{
    public class FacebookObject : BaseModel
    {
        public string title { get; set; }
        public DateTime created_Time { get; set; }
        public string type { get; set; } //website
    }
}
