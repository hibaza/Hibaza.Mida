using Hibaza.CCP.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class CustomerModel : BaseModel
    {
        public bool archived { get; set; }
        public bool blocked { get; set; }
        public string status { get; set; }
        public bool unread { get; set; }
        public bool nonreply { get; set; }
        public bool open { get; set; }
        public string agent_id { get; set; }
        public string channel_id { get; set; }
        public string ext_id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string avatar { get; set; }
        public string last_message { get; set; }
        public long timestamp { get; set; }
        public string assign_by { get; set; }
        public long assign_at { get; set; }
        public string sex { get; set; }
        public int weight { get; set; }
        public int height { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string real_name { get; set; } = "";
        public ThreadModel active_thread { get; set; }
        public TicketModel active_ticket { get; set; }
        public CustomerModel() { }
        public CustomerModel(Customer customer)
        {
            this.id = customer.id;
            this.blocked = customer.blocked;
            this.archived = customer.archived;
            this.status = customer.status;
            this.unread = customer.unread;
            this.nonreply = customer.nonreply;
            this.agent_id = customer.agent_id;
            this.channel_id = customer.channel_id;
            this.active_thread = string.IsNullOrWhiteSpace(customer.active_thread) ? null : new ThreadModel(JsonConvert.DeserializeObject<Thread>(customer.active_thread));
            this.active_ticket = string.IsNullOrWhiteSpace(customer.active_ticket) ? null : new TicketModel(JsonConvert.DeserializeObject<Ticket>(customer.active_ticket));
            this.active_thread.agent_id = customer.agent_id;
            this.open = customer.open;
            this.ext_id = customer.ext_id;
            this.name = string.IsNullOrWhiteSpace(customer.name) ? customer.first_name + " " + customer.last_name : customer.name;
            this.phone = customer.phone;
            this.avatar = customer.avatar;
            this.timestamp = customer.timestamp;
            this.assign_by = customer.assigned_by;
            this.assign_at = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(customer.assigned_at);
            this.sex = customer.sex;
            this.weight = customer.weight;
            this.height = customer.height;
            this.address = customer.address;
            this.city = customer.city;
            this.real_name = customer.real_name;
    }

    }
}
