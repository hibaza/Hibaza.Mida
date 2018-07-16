using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Facebook
{
    public class FacebookWebsite : BaseModel
    {
        public string title { get; set; }
        public string site_name { get; set; }
        public IEnumerable<FacebookImage> image { get; set; }
        public FacebookApplication application { get; set; }
    }
}
