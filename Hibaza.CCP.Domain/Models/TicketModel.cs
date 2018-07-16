using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class TicketModel : BaseModel
    {
        public string number
        {
            get
            {
                try
                {
                    return this.id.Substring(this.id.Length - 5);
                }
                catch {
                    return ""; }
            }
        }
        public string getStatusName(int status)
        {
            switch (status)
            {
                case 1: return "Attention";
                case 2: return "Completed";
                case 3: return "Rejected";
                default: return "Pending";
            }
        }
        public int type { get; set; }
        public string business_id { get; set; }
        public string channel_id { get; set; }
        public string customer_id { get; set; }
        public string customer_name { get; set; }
        public string customer_avatar { get; set; }
        public string thread_id { get; set; }
        public string order_id { get; set; }
        public string description { get; set; }
        public string short_description { get; set; }
        public string tags { get; set; }
        public int status { get; set; }
        public string sender_id { get; set; }
        public string sender_name { get; set; }
        public string sender_avatar { get; set; }
        public string created_time { get; set; }
        public long timestamp { get; set; }
        public TicketModel() { }
        public TicketModel(Ticket ticket)
        {
            if (ticket == null) return ;
            id = ticket.id;
            created_time = ticket.created_time.ToLocalTime().ToString("hh:mm dd/MM/yyyy");
            sender_id = ticket.sender_id;
            sender_name = ticket.sender_name;
            sender_avatar = ticket.sender_avatar;
            short_description = ticket.short_description;
            description = ticket.description;
            status = ticket.status;
            tags = ticket.tags;
            type = ticket.type;
            thread_id = ticket.thread_id;
            order_id = ticket.order_id;
            customer_id = ticket.customer_id;
            customer_name = ticket.customer_name;
            business_id = ticket.business_id;
            timestamp = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(ticket.created_time);

        }
    }
}
