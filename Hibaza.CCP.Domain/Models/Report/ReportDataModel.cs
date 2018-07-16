using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Report
{
    public class ReportAgentDataLine
    {
        public string id { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public int conversations { get; set; }
        public int customers { get; set; }
        public int tickets { get; set; }
        public int messages { get; set; }
        public int average_repsonse_time { get; set; }
        public int first_response_time { get; set; }
        public long last_login { get; set; }
        public long last_acted { get; set; }
    }

    public class ReportChatDataLine
    {
        public string name { get; set; }
        public DateTime date { get; set; }
        public int conversations { get; set; }
        public int customers { get; set; }
        public int tickets { get; set; }
        public int inboxes { get; set; }
        public int comments { get; set; }
        public int inbox_replies { get; set; }
        public int comment_replies { get; set; }
    }

    public class ReportTicketDataLine
    {
        public string id { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }
        public int tickets { get; set; }
        public int completed_tickets { get; set; }
        public int pending_tickets { get; set; }
        public int attention_tickets { get; set; }
        public int rejected_tickets { get; set; }
    }

    public class ReportResponseModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public int first_response_time { get; set; }
    }

    public class ReportMessageModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public int messages { get; set; }
    }

    public class ReportAgentListModel
    {
        public string last_page { get; set; }
        public List<ReportAgentDataLine> data { get; set; }
    }
    public class ReportTicketModel
    {
        public string id { get; set; }
        public long created_at { get; set; }
        public int status { get; set; }
        public string status_display { get; set; }
        public string short_description { get; set; }
        public string customer_id { get; set; }
        public string customer_name { get; set; }
        public string customer_avatar { get; set; }
        public string[] agents { get; set; }
        public string channel_id { get; set; }
        public string[] tags { get; set; }
    }

    public class ReportTicketListModel
    {
        public string last_page { get; set; }
        public List<ReportTicketModel> data { get; set; }
    }

    public class ReportCustomerModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string channel_id { get; set; }
        public long date_registered { get; set; }
        public int pending_tickets { get; set; }
        public int completed_tickets { get; set; }
        public int threads { get; set; }
        public int messages { get; set; }
    }

    public class ReportCustomerListModel
    {
        public string last_page { get; set; }
        public List<ReportCustomerModel> data { get; set; }
    }

    public class ReportLine : BaseModel
    {
        public string name { get; set; }
        public string avatar { get; set; } = "avatars/1.png";
    }


    public class ChartDataLine : BaseModel
    {
        public string name { get; set; }
        public dynamic data { get; set; }
    }

    public class AgentLine : ReportLine { }
    public class CustomerLine : ReportLine { }

    public class ReportChartDataModel
    {
        public int total { get; set; }
        public List<ChartDataLine> lines { get; set; }
    }

    
    public class TopAgentsModel
    {
        public IEnumerable<AgentLine> lines { get; set; }
    }

    public class TopCustomersModel
    {
        public IEnumerable<CustomerLine> lines { get; set; }
    }

}
