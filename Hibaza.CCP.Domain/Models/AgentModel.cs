using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{  
    public class AgentModel : BaseModel
    { 
        public string username { get; set; }
        public string status { get; set; }
        public bool admin { get
            {
                return this.role == "admin";
            }
            set { }
        }

        public string name { get
            {
                return this.first_name + " " + this.last_name;
            }
            set { }
        }
        public string avatar { get; set; }
        public string role { get; set; }
        public string business_id { get; set; }
        public string business_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public bool active { get; set; }
        public bool locked { get; set; }
        public long timestamp { get; set; }
        public AgentModel() { }
        public AgentModel(Agent agent)
        {
            this.id = agent.id;
            this.business_id = agent.business_id;
            this.business_name = agent.business_name;
            this.username = agent.username;
            this.first_name = agent.first_name;
            this.last_name = agent.last_name;
            this.status = agent.status;
            this.role = agent.role;
            this.avatar = agent.avatar;
            this.email = agent.email;
            this.active = agent.active;
            this.locked = agent.locked;
            this.status = agent.login_status == "online" ? agent.active ? "online" : "busy" : agent.login_status;
            this.timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(agent.updated_time);
        }
    }
}
