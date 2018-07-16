using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class TagModel : BaseModel
    {
        public string owner_id { get; set; }
        public string tags { get; set; }
    }
}
