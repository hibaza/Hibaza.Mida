using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Service;
using Hibaza.CCP.Service.Facebook;
using Hibaza.CCP.Core;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Facebook;
using Firebase.Storage;
using Microsoft.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using Hangfire;
using Hibaza.CCP.Domain.Models.Report;

namespace Hibaza.CCP.Api.Controllers
{
    [Route("reports")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly AppSettings _appSettings;
        private readonly ILoggingService _logService;
        private readonly IFacebookConversationService _facebookService;
        private readonly IChannelService _channelService;
        private readonly IAgentService _agentService;
        private readonly IBusinessService _businessService;
        public ReportController(IBusinessService businessService, IAgentService agentService, IReportService reportService, IFacebookConversationService facebookService, IChannelService channelService, IOptions<AppSettings> appSettings, ILoggingService logService)
        {
            _reportService = reportService;
            _appSettings = appSettings.Value;
            _channelService = channelService;
            _facebookService = facebookService;
            _agentService = agentService;
            _businessService = businessService;
            _logService = logService;
        }


        [HttpGet("auto_report_update")]
        public int AutoReportUpdate([FromQuery]int minutes, [FromQuery]string access_token)
        {
            if (access_token != "@bazavietnam") return 0;
          // _reportService.UpsertReportAll();
         RecurringJob.AddOrUpdate<ReportService>("AutoReportUpdate", x => x.UpsertReportAll(), Cron.MinuteInterval(minutes));


            return 1;
        }
       

        [HttpGet("top_agents/{business_id}")]
        public TopAgentsModel GetTopAgents(string business_id)
        {
            TopAgentsModel model = new TopAgentsModel();
            model.lines = _reportService.GetTopAgents(business_id, new Paging { Limit = 10 }).Result.Select(a => new AgentLine { id = a.id, name = a.name, avatar = a.avatar });
            return model;

            //{ "lines": [{"id": 1664, "name": "Human MM", "avatar": "avatars/default/1.png"}, {"id": 1815, "name": "Agent1 Bz", "avatar": "avatars/default/9.png"}]}
            //reports/samurais_top
        }

        [HttpGet("db_agents/{business_id}")]
        public ReportChartDataModel GetAgentChartData(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportChartDataModel model = new ReportChartDataModel { lines = new List<ChartDataLine>() };
            IEnumerable<ReportAgentDataLine> list = _reportService.GetAgentChartData(business_id, filter).Result;
            foreach (var item in list)
            {
                //var data = string.Format("[{0}, {1}, {2}, {3}]", item.customers, item.conversations, item.messages, item.tickets);
                var data = new int[] { item.customers, item.conversations, item.messages, item.tickets };
                model.lines.Add(new ChartDataLine { id = item.id, name = item.name, data = data });
                model.total++;
            }
            return model;
            //db_agents?date=range&init=1483203600000&finish=1488034801447&agent=1664%2C1815&period=month
        }


        [HttpGet("agents/{business_id}")]
        public ReportAgentListModel GetAgents(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportAgentListModel model = new ReportAgentListModel { data = new List<ReportAgentDataLine>() };
            IEnumerable<ReportAgentDataLine> list = _reportService.GetAgents(business_id, filter).Result;
            foreach (var item in list)
            {
                model.data.Add(item);
            }
            return model;
            //{"last_page": 1.0952380952380953, "data": [{"last_login": 1488074399.0, "avg_first_response_time": 497, "id": 1664, "chats": 59, "first_name": "Human MM", "completed_orders": 0, "avg_response_time": 0, "avatar": "avatars/default/1.png"}, {"last_login": null, "avg_first_response_time": 0, "id": 1815, "chats": 0, "first_name": "Agent1 Bz", "completed_orders": 0, "avg_response_time": 0, "avatar": "avatars/default/9.png"}]}
            //reports/agents?date=range&init=1483203600000&finish=1485968399999&agent=1664%2C1815&page=1
        }

        [HttpGet("agents_bar/{business_id}")]
        public ReportChartDataModel GetAgentsBars(string business_id, [FromQuery]ReportDataFilter filter)
        {
            filter.limit = 50;
            ReportChartDataModel model = new ReportChartDataModel { lines = new List<ChartDataLine>() };
            IEnumerable<ReportAgentDataLine> list = _reportService.GetAgentChartData(business_id, filter).Result;
            foreach (var item in list)
            {
                var data = new int[] { item.customers, item.conversations, item.messages, item.tickets };
                model.lines.Add(new ChartDataLine { id = item.id, name = item.name, data = data });
                model.total++;
            }
            return model;
            //{"lines": [{"name": "Human MM", "data": [497, 0, 0, 59]}, {"name": "Agent1 Bz", "data": [0, 0, 0, 0]}]}
            //reports/agents_bar?date=range&init=1483203600000&finish=1485968399999&agent=1664%2C1815&page=1

        }


        [HttpGet("top_customers/{business_id}")]
        public TopCustomersModel GetTopCustomers(string business_id)
        {
            TopCustomersModel model = new TopCustomersModel();
            model.lines = _reportService.GetTopCustomers(business_id, new Paging { Limit = 10 }).Result.Select(a => new CustomerLine { id = a.id, name = a.owner_name, avatar = string.IsNullOrWhiteSpace(a.owner_avatar) ? "avatars/yexir.png" : a.owner_avatar });
            return model;
            //{"lines": [{"id": 43264, "name": "Baza Vietnam", "avatar": "avatars / yexir.png"}, {"id": 44410, "name": "Qu\u1ef3nh Nh\u01b0", "avatar": "avatars / yexir.png"}, {"id": 39751, "name": "Phan Nhung", "avatar": "avatars / yexir.png"}, {"id": 39754, "name": "\u0110o\u00e0n Nhi", "avatar": "avatars / yexir.png"}, {"id": 39755, "name": "B\u00ecnh Tr\u1ecbnh", "avatar": "avatars / yexir.png"}, {"id": 39759, "name": "Ti\u1ebfn Ph\u1ea1m", "avatar": "avatars / yexir.png"}, {"id": 39764, "name": "H\u01b0ng C\u00f2ii", "avatar": "avatars / yexir.png"}, {"id": 39791, "name": "Ha Nguyen", "avatar": "avatars / yexir.png"}, {"id": 39803, "name": "S\u1eefu Ca", "avatar": "avatars / yexir.png"}, {"id": 39821, "name": "\u0110inh Nghi\u1ec7p", "avatar": "avatars / yexir.png"}]}
            //reports/customers_top
        }


        [HttpGet("db_chats/{business_id}")]
        public ReportChartDataModel GetConversationChartData(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportChartDataModel model = new ReportChartDataModel { lines = new List<ChartDataLine>() };
            IEnumerable<ReportChatDataLine> list = _reportService.GetAgentChatChartData(business_id, filter).Result;
            var total = 0;
            foreach (var item in list)
            {
                var data = new int[] { item.customers };
                total += item.customers;
                model.lines.Add(new ChartDataLine { name = item.name, data = data });
            }
            model.total = total;
            return model;

            //{"total": 405, "lines": [{"name": "2016/12", "data": [0] }, {"name": "2017/1", "data": [182]}, {"name": "2017/2", "data": [223]}]}
            //reports/db_chats?date=range&init=1483203600000&finish=1488038268796&samurai=1664%2C1815&period=month

        }


        [HttpGet("customers/{business_id}")]
        public ReportCustomerListModel GetCustomers(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportCustomerListModel model = new ReportCustomerListModel();
            model.data = _reportService.GetCustomers(business_id, filter).Result.ToList();
            return model;
            //{"last_page": 20.285714285714285, "data": [{"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Phan Nhung", "avatar": "avatars/yexir.png", "phone": "", "id": 39751, "messages": 1, "date_registred": 1485429405.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "\u0110o\u00e0n Nhi", "avatar": "avatars/yexir.png", "phone": "", "id": 39754, "messages": 1, "date_registred": 1485429978.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "B\u00ecnh Tr\u1ecbnh", "avatar": "avatars/yexir.png", "phone": "", "id": 39755, "messages": 2, "date_registred": 1485430236.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Ti\u1ebfn Ph\u1ea1m", "avatar": "avatars/yexir.png", "phone": "", "id": 39759, "messages": 2, "date_registred": 1485431838.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "H\u01b0ng C\u00f2ii", "avatar": "avatars/yexir.png", "phone": "", "id": 39764, "messages": 2, "date_registred": 1485433939.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Ha Nguyen", "avatar": "avatars/yexir.png", "phone": "", "id": 39791, "messages": 1, "date_registred": 1485438226.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "S\u1eefu Ca", "avatar": "avatars/yexir.png", "phone": "", "id": 39803, "messages": 1, "date_registred": 1485439879.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "\u0110inh Nghi\u1ec7p", "avatar": "avatars/yexir.png", "phone": "", "id": 39821, "messages": 2, "date_registred": 1485442806.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Ngocdue Pham", "avatar": "avatars/yexir.png", "phone": "", "id": 39835, "messages": 2, "date_registred": 1485444786.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Li\u1ec1u V\u00e2n", "avatar": "avatars/yexir.png", "phone": "", "id": 39881, "messages": 2, "date_registred": 1485450317.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "H\u00f9ng D\u01b0\u01a1ng", "avatar": "avatars/yexir.png", "phone": "", "id": 39886, "messages": 2, "date_registred": 1485450554.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Dung Do", "avatar": "avatars/yexir.png", "phone": "", "id": 39906, "messages": 2, "date_registred": 1485451665.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Nhi Phn", "avatar": "avatars/yexir.png", "phone": "", "id": 39966, "messages": 4, "date_registred": 1485456865.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Khuong Nguyen", "avatar": "avatars/yexir.png", "phone": "", "id": 39997, "messages": 2, "date_registred": 1485460859.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Th\u1ee7y Nguy\u1ec5n", "avatar": "avatars/yexir.png", "phone": "", "id": 40034, "messages": 1, "date_registred": 1485465236.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Lac LeGia", "avatar": "avatars/yexir.png", "phone": "", "id": 40039, "messages": 3, "date_registred": 1485465971.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Tra Hen", "avatar": "avatars/yexir.png", "phone": "", "id": 40059, "messages": 1, "date_registred": 1485468356.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Nguyen Dem", "avatar": "avatars/yexir.png", "phone": "", "id": 40085, "messages": 1, "date_registred": 1485472661.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Nguy\u1ec5n Hu\u1ec7", "avatar": "avatars/yexir.png", "phone": "", "id": 40086, "messages": 2, "date_registred": 1485472677.0, "city": null}, {"completed_tickets": 0, "address": null, "channel_id": 2161, "email": null, "pending_tickets": 0, "name": "Do Hieu Dovan Hieu", "avatar": "avatars/yexir.png", "phone": "", "id": 40096, "messages": 1, "date_registred": 1485473823.0, "city": null}]}
            //reports/customers?date=range&init=1483203600000&finish=1485968399999&customer=43264%2C44410%2C39751%2C39754%2C39755%2C39759%2C39764%2C39791%2C39803%2C39821&page=1
        }

        [HttpGet("customers_bar/{business_id}")]
        public ReportChartDataModel GetCustomersBars(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportChartDataModel model = new ReportChartDataModel { lines = new List<ChartDataLine>() };
            IEnumerable<ReportChatDataLine> list = _reportService.GetCustomerChatChartData(business_id, filter).Result;
            var total = 0;
            var data = new int[] { list.Select(t => t.customers).Sum(), list.Select(t => t.inboxes).Sum(), list.Select(t => t.comments).Sum(), list.Select(t => t.inbox_replies).Sum(), list.Select(t => t.comment_replies).Sum() };
            model.lines.Add(new ChartDataLine { data = data });
            model.total = total;
            return model;
            //{"lines": [{"name": "Phan Nhung", "data": [1, 1, 0]}, {"name": "\u0110o\u00e0n Nhi", "data": [1, 1, 0]}, {"name": "B\u00ecnh Tr\u1ecbnh", "data": [2, 1, 0]}, {"name": "Ti\u1ebfn Ph\u1ea1m", "data": [2, 1, 0]}, {"name": "H\u01b0ng C\u00f2ii", "data": [2, 1, 0]}, {"name": "Ha Nguyen", "data": [1, 1, 0]}, {"name": "S\u1eefu Ca", "data": [1, 1, 0]}, {"name": "\u0110inh Nghi\u1ec7p", "data": [2, 1, 0]}, {"name": "Baza Vietnam", "data": [0, 0, 0]}, {"name": "Qu\u1ef3nh Nh\u01b0", "data": [0, 0, 0]}]}
            //reports/customers_bar?date=range&init=1483203600000&finish=1485968399999&customer=43264%2C44410%2C39751%2C39754%2C39755%2C39759%2C39764%2C39791%2C39803%2C39821&page=1
        }

        [HttpGet("customers_line/{business_id}")]
        public ReportChartDataModel GetCustomersLines(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportChartDataModel model = new ReportChartDataModel { lines = new List<ChartDataLine>() };
            IEnumerable<ReportChatDataLine> list = _reportService.GetCustomerChatChartData(business_id, filter).Result;
            var total = 0;
            foreach (var item in list)
            {
                var data = new int[] { item.customers, item.inboxes, item.comments, item.inbox_replies, item.comment_replies };
                total += item.customers;
                model.lines.Add(new ChartDataLine { name = item.name, data = data });
            }
            model.total = total;
            return model;
            //{"lines": [{"name": "Phan Nhung", "data": [1, 1, 0]}, {"name": "\u0110o\u00e0n Nhi", "data": [1, 1, 0]}, {"name": "B\u00ecnh Tr\u1ecbnh", "data": [2, 1, 0]}, {"name": "Ti\u1ebfn Ph\u1ea1m", "data": [2, 1, 0]}, {"name": "H\u01b0ng C\u00f2ii", "data": [2, 1, 0]}, {"name": "Ha Nguyen", "data": [1, 1, 0]}, {"name": "S\u1eefu Ca", "data": [1, 1, 0]}, {"name": "\u0110inh Nghi\u1ec7p", "data": [2, 1, 0]}, {"name": "Baza Vietnam", "data": [0, 0, 0]}, {"name": "Qu\u1ef3nh Nh\u01b0", "data": [0, 0, 0]}]}
            //reports/customers_bar?date=range&init=1483203600000&finish=1485968399999&customer=43264%2C44410%2C39751%2C39754%2C39755%2C39759%2C39764%2C39791%2C39803%2C39821&page=1
        }


        [HttpGet("db_tickets/{business_id}")]
        public ReportChartDataModel GetTicketChartData(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportChartDataModel model = new ReportChartDataModel { lines = new List<ChartDataLine>() };
            IEnumerable<ReportTicketDataLine> list = _reportService.GetTicketChartData(business_id, filter).Result;
            var total = 0;
            foreach (var item in list)
            {
                var data = new int[] { item.tickets };
                model.lines.Add(new ChartDataLine { id = item.id, name = item.name, data = data });
                total += item.tickets;
            }
            model.total = total;
            return model;

            //{ "total": 408, "lines": [{"name": "2016/12", "data": [0]}, {"name": "2017/1", "data": [182]}, {"name": "2017/2", "data": [226]}]}
            //reports/db_ticket_opened?date=range&init=1483203600000&finish=1488038268796&samurai=1664%2C1815&period=month

        }

        [HttpGet("tickets/{business_id}")]
        public ReportTicketListModel GetTickets(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportTicketListModel model = new ReportTicketListModel { data = new List<ReportTicketModel>() };
            IEnumerable<ReportTicketModel> list = _reportService.GetTickets(business_id, filter).Result;
            foreach (var item in list)
            {
                model.data.Add(item);
            }
            return model;

            //{"last_page": 9.666666666666666, "data": [{"status_display": "Attention", "customer_name": "Phan Nhung", "status": 1, "subcathegories": [], "customer_id": 39751, "samurais": [1664], "id": 29287, "channel_id": 2161, "created_at": 1485429407.0, "short_description": "#29287", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "\u0110o\u00e0n Nhi", "status": 0, "subcathegories": [], "customer_id": 39754, "samurais": [1664], "id": 29288, "channel_id": 2161, "created_at": 1485429979.0, "short_description": "#29288", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "B\u00ecnh Tr\u1ecbnh", "status": 0, "subcathegories": [], "customer_id": 39755, "samurais": [1664], "id": 29291, "channel_id": 2161, "created_at": 1485430237.0, "short_description": "#29291", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Ti\u1ebfn Ph\u1ea1m", "status": 0, "subcathegories": [], "customer_id": 39759, "samurais": [], "id": 29298, "channel_id": 2161, "created_at": 1485431840.0, "short_description": "#29298", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "H\u01b0ng C\u00f2ii", "status": 0, "subcathegories": [], "customer_id": 39764, "samurais": [1664], "id": 29303, "channel_id": 2161, "created_at": 1485433941.0, "short_description": "#29303", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Ha Nguyen", "status": 0, "subcathegories": [], "customer_id": 39791, "samurais": [1664], "id": 29334, "channel_id": 2161, "created_at": 1485438227.0, "short_description": "#29334", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "S\u1eefu Ca", "status": 0, "subcathegories": [], "customer_id": 39803, "samurais": [1664], "id": 29347, "channel_id": 2161, "created_at": 1485439880.0, "short_description": "#29347", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "\u0110inh Nghi\u1ec7p", "status": 0, "subcathegories": [], "customer_id": 39821, "samurais": [1664], "id": 29370, "channel_id": 2161, "created_at": 1485442807.0, "short_description": "#29370", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Ngocdue Pham", "status": 0, "subcathegories": [], "customer_id": 39835, "samurais": [1664], "id": 29390, "channel_id": 2161, "created_at": 1485444788.0, "short_description": "#29390", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Li\u1ec1u V\u00e2n", "status": 0, "subcathegories": [], "customer_id": 39881, "samurais": [1664], "id": 29451, "channel_id": 2161, "created_at": 1485450324.0, "short_description": "#29451", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "H\u00f9ng D\u01b0\u01a1ng", "status": 0, "subcathegories": [], "customer_id": 39886, "samurais": [1664], "id": 29456, "channel_id": 2161, "created_at": 1485450556.0, "short_description": "#29456", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Dung Do", "status": 0, "subcathegories": [], "customer_id": 39906, "samurais": [1664], "id": 29479, "channel_id": 2161, "created_at": 1485451667.0, "short_description": "#29479", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Nhi Phn", "status": 0, "subcathegories": [], "customer_id": 39966, "samurais": [1664], "id": 29549, "channel_id": 2161, "created_at": 1485456866.0, "short_description": "#29549", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Khuong Nguyen", "status": 0, "subcathegories": [], "customer_id": 39997, "samurais": [1664], "id": 29596, "channel_id": 2161, "created_at": 1485460861.0, "short_description": "#29596", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Th\u1ee7y Nguy\u1ec5n", "status": 0, "subcathegories": [], "customer_id": 40034, "samurais": [1664], "id": 29643, "channel_id": 2161, "created_at": 1485465237.0, "short_description": "#29643", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Lac LeGia", "status": 0, "subcathegories": [], "customer_id": 40039, "samurais": [1664], "id": 29652, "channel_id": 2161, "created_at": 1485465973.0, "short_description": "#29652", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Tra Hen", "status": 0, "subcathegories": [], "customer_id": 40059, "samurais": [1664], "id": 29680, "channel_id": 2161, "created_at": 1485468358.0, "short_description": "#29680", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Nguyen Dem", "status": 0, "subcathegories": [], "customer_id": 40085, "samurais": [1664], "id": 29708, "channel_id": 2161, "created_at": 1485472662.0, "short_description": "#29708", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Nguy\u1ec5n Hu\u1ec7", "status": 0, "subcathegories": [], "customer_id": 40086, "samurais": [1664], "id": 29709, "channel_id": 2161, "created_at": 1485472678.0, "short_description": "#29709", "customer_avatar": "avatars/yexir.png"}, {"status_display": "Pending", "customer_name": "Do Hieu Dovan Hieu", "status": 0, "subcathegories": [], "customer_id": 40096, "samurais": [1664], "id": 29719, "channel_id": 2161, "created_at": 1485473824.0, "short_description": "#29719", "customer_avatar": "avatars/yexir.png"}]}        
            //reports/orders?date=range&init=1483203600000&finish=1485968399999&period=week&page=1
        }


        [HttpGet("tickets_bar/{business_id}")]
        public ReportChartDataModel GetTicketsBar(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportChartDataModel model = new ReportChartDataModel { lines = new List<ChartDataLine>() };
            IEnumerable<ReportTicketDataLine> list = _reportService.GetTicketChartData(business_id, filter).Result;
            var total = 0;
            var data = new int[] { list.Select(t => t.tickets).Sum(), list.Select(t => t.tickets - (t.completed_tickets + t.pending_tickets + t.attention_tickets)).Sum(), list.Select(t => t.pending_tickets).Sum(), list.Select(t => t.attention_tickets).Sum() };
            model.lines.Add(new ChartDataLine
            {
                data = data
            });
            model.total = total;
            return model;
            //{"lines": [{"name": "2016-12-26", "data": [0, 0, 0]}, {"name": "2017-01-02", "data": [0, 0, 0]}, {"name": "2017-01-09", "data": [0, 0, 0]}, {"name": "2017-01-16", "data": [0, 0, 0]}, {"name": "2017-01-23", "data": [148, 0, 0]}, {"name": "2017-01-30", "data": [17, 0, 0]}]}
            //reports/orders_line?date=range&init=1483203600000&finish=1485968399999&period=week&page=1
        }

        [HttpGet("tickets_line/{business_id}")]
        public ReportChartDataModel GetTicketsLine(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportChartDataModel model = new ReportChartDataModel { lines = new List<ChartDataLine>() };
            IEnumerable<ReportTicketDataLine> list = _reportService.GetTicketChartData(business_id, filter).Result;
            var total = 0;
            foreach (var item in list)
            {
                var data = new int[] { item.tickets, item.completed_tickets, item.tickets - (item.completed_tickets + item.pending_tickets + item.attention_tickets), item.pending_tickets, item.attention_tickets };
                model.lines.Add(new ChartDataLine { name = item.name, data = data });
                total += item.tickets;
            }
            model.total = total;
            return model;
            //{"lines": [{"name": "2016-12-26", "data": [0, 0, 0]}, {"name": "2017-01-02", "data": [0, 0, 0]}, {"name": "2017-01-09", "data": [0, 0, 0]}, {"name": "2017-01-16", "data": [0, 0, 0]}, {"name": "2017-01-23", "data": [148, 0, 0]}, {"name": "2017-01-30", "data": [17, 0, 0]}]}
            //reports/orders_line?date=range&init=1483203600000&finish=1485968399999&period=week&page=1
        }

        [HttpGet("db_messages/{business_id}")]
        public ReportChartDataModel GetMessageChartData(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportChartDataModel model = new ReportChartDataModel { lines = new List<ChartDataLine>() };
            IEnumerable<ReportChatDataLine> list = _reportService.GetCustomerChatChartData(business_id, filter).Result;
            var total = 0;
            foreach (var item in list)
            {
                var data = new int[] { item.inboxes, item.comments };
                total += item.inboxes + item.comments;
                model.lines.Add(new ChartDataLine { name = item.name, data = data });
            }
            model.total = total;
            return model;

            // { "total": 408, "lines": [{"name": "2016/12", "data": [0] }, {"name": "2017/1", "data": [182]}, {"name": "2017/2", "data": [226]}]}

        }

        [HttpGet("db_agent_messages/{business_id}")]
        public ReportChartDataModel GetAgentMessageChartData(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportChartDataModel model = new ReportChartDataModel { lines = new List<ChartDataLine>() };
            IEnumerable<ReportChatDataLine> list = _reportService.GetAgentChatChartData(business_id, filter).Result;
            var total = 0;
            foreach (var item in list)
            {
                var data = new int[] { item.inboxes, item.comments };
                total += item.inboxes + item.comments;
                model.lines.Add(new ChartDataLine { name = item.name, data = data });
            }
            model.total = total;
            return model;

            // { "total": 408, "lines": [{"name": "2016/12", "data": [0] }, {"name": "2017/1", "data": [182]}, {"name": "2017/2", "data": [226]}]}

        }

        [HttpGet("db_first_response/{business_id}")]
        public ReportChartDataModel GetFirstResponseChartData(string business_id, [FromQuery]ReportDataFilter filter)
        {
            ReportChartDataModel model = new ReportChartDataModel { lines = new List<ChartDataLine>() };
            IEnumerable<ReportResponseModel> list = _reportService.GetFirstResponseChartData(business_id, filter).Result;
            foreach (var item in list)
            {
                var data = string.Format("[{0}]", string.Join(",", new int[item.first_response_time]));
                model.lines.Add(new ChartDataLine { id = item.id, name = item.name, data = data });
            }
            return model;


            //{"total": 0, "lines": [{"name": "2016/12", "data": [0] }, {"name": "2017/1", "data": [497]}, {"name": "2017/2", "data": [23]}]}
            //reports/db_first_response?date=range&init=1483203600000&finish=1488038268796&samurai=1664%2C1815&period=month

        }

    }
}