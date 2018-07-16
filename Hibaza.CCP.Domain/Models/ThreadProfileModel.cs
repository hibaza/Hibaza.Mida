using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{

    public class ThreadProfileModel : BaseModel
    {
        public PostModel post { get; set; }
        public List<string> last_visits { get; set; }

    }

}
