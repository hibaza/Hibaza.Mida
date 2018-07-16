using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Web.Models
{
    //public class FacebookAgentEdit : Agent
    //{
    //    public FacebookAgentEdit(Agent data)
    //    {
    //        this.ext_id = data.ext_id;
    //        this.id = data.id;
    //        this.first_name = data.first_name;
    //        this.last_name = data.last_name;
    //        this.email = data.email;
    //        this.avatar = data.avatar;
    //    }

    //    public string Token { get; set; }
    //}

    public class AgentSettings
    {
        public bool admin { get; set; }
        public string user_id { get; set; }
        public string business_id { get; set; }
        public IEnumerable<AgentModel> Agents { get; set; }
    }
}
