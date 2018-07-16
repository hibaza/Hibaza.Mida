//using Hibaza.CCP.Data.Infrastructure;
//using Hibaza.CCP.Data.Repositories;
//using Hibaza.CCP.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Hibaza.CCP.Domain.Models;
//using Hibaza.CCP.Domain.Models.Report;

//namespace Hibaza.CCP.Service
//{
//    public class ReportService : IReportService
//    {
//        private readonly IAgentService _agentService;
//        private readonly IThreadService _threadService;
//        private readonly ICustomerService _customerService;
//        private readonly ITicketService _ticketService;
//        private readonly IReportRepository _reportRepository;
//        public ReportService(IReportRepository reportRepository, ITicketService ticketService, ICustomerService customerService, IAgentService agentService, IThreadService threadService)
//        {
//            _reportRepository = reportRepository;
//            _agentService = agentService;
//            _threadService = threadService;
//            _customerService = customerService;
//            _ticketService = ticketService;
//        }

//        public async Task<IEnumerable<ReportAgentDataLine>> GetAgentChartData(string business_id, ReportDataFilter filter)
//        {
//            Paging page = new Paging { Limit = filter.limit, Previous = (filter.init_utc > 99999999999 ? filter.init_utc / 1000 : filter.init_utc).ToString(), Next = (filter.finish_utc > 99999999999 ? filter.finish_utc / 1000 : filter.finish_utc).ToString() };
//            IEnumerable<ReportAgentDataLine> list = await _reportRepository.GetAgentChartData(business_id, page);
//            var agents = (await _agentService.GetAgents(business_id, 0, 100)).Where(a=>!a.locked).ToDictionary(a => a.id, b => b.first_name + ' ' + b.last_name);
//            foreach (var item in list)
//            {
//                if (string.IsNullOrWhiteSpace(item.name) && agents.ContainsKey(item.id)) item.name = agents[item.id];
//            }
//            return list;
//        }

//        public async Task<IEnumerable<ReportAgentDataLine>> GetAgents(string business_id, ReportDataFilter filter)
//        {
//            Paging page = new Paging { Limit = 50, Previous = (filter.init_utc > 99999999999 ? filter.init_utc / 1000 : filter.init_utc).ToString(), Next = (filter.finish_utc > 99999999999 ? filter.finish_utc / 1000 : filter.finish_utc).ToString() };
//            IEnumerable<ReportAgentDataLine> list = await _reportRepository.GetAgentChartData(business_id, page);
//            var agents = (await _agentService.GetAgents(business_id, 0, 100)).Where(a => !a.locked).ToDictionary(a => a.id, b => b);
//            foreach (var item in list)
//            {
//                if (string.IsNullOrWhiteSpace(item.name) && agents.ContainsKey(item.id)) item.name = agents[item.id].first_name + ' ' + agents[item.id].last_name;
//                item.last_acted = agents[item.id].last_acted_time == null ? 0 : Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(agents[item.id].last_acted_time);
//                item.last_login = agents[item.id].last_loggedin_time == null ? 0 : Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(agents[item.id].last_loggedin_time);
//                item.avatar = agents[item.id].avatar;
//            }
//            return list;
//        }

//        public async Task<IEnumerable<ReportTicketModel>> GetTickets(string business_id, ReportDataFilter filter)
//        {
//            Paging page = new Paging { Limit = filter.limit, Previous = Core.Helpers.CommonHelper.UnixTimestampToDateTime(filter.init_utc > 99999999999 ? filter.init_utc / 1000 : filter.init_utc).ToString(), Next = Core.Helpers.CommonHelper.UnixTimestampToDateTime(filter.finish_utc > 99999999999 ? filter.finish_utc / 1000 : filter.finish_utc).ToString() };
//            List<ReportTicketModel> list = new List<ReportTicketModel>();

//            foreach (var item in await _ticketService.GetTickets(business_id, page))
//            {
//                list.Add(new ReportTicketModel
//                {
//                    id = item.id,
//                    customer_name = item.customer_name,
//                    customer_avatar = item.customer_avatar,
//                    customer_id = item.customer_id,
//                    channel_id = item.channel_id,
//                    created_at = item.timestamp,
//                    status = item.status,
//                    status_display = item.getStatusName(item.status),
//                    short_description = item.short_description,
//                    tags = string.IsNullOrWhiteSpace(item.tags) ? new string[] { } :  item.tags.Split(','),
//                    agents = new string[] { item.sender_id }
//                });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<ReportChatDataLine>> GetAgentChatChartData(string business_id, ReportDataFilter filter)
//        {
//            Paging page = new Paging { Limit = 50, Previous = (filter.init_utc > 99999999999 ? filter.init_utc / 1000 : filter.init_utc).ToString(), Next = (filter.finish_utc > 99999999999 ? filter.finish_utc / 1000 : filter.finish_utc).ToString() };
//            IEnumerable<ReportChatDataLine> list = await _reportRepository.GetAgentChatChartData(business_id, page);
//            if (list != null && list.Count() > 0)
//                switch (filter.period)
//                {
//                    case "week":
//                        var first = list.Count() > 0 ? list.First().date : DateTime.MinValue;
//                        list = list.GroupBy(a => (a.date - first).TotalDays / 7).Select(g => new ReportChatDataLine
//                        {
//                            name = g.First().date.ToString("dd/MM"),
//                            customers = g.Select(b => b.customers).Sum(),
//                            conversations = g.Select(b => b.conversations).Sum(),
//                            comments = g.Select(b => b.comments).Sum(),
//                            inboxes = g.Select(b => b.inboxes).Sum()
//                        }).ToList();
//                        break;
//                    case "month":
//                        list = list.GroupBy(a => a.date.Month).Select(g => new ReportChatDataLine
//                        {
//                            name = g.First().date.ToString("MM/yy"),
//                            customers = g.Select(b => b.customers).Sum(),
//                            conversations = g.Select(b => b.conversations).Sum(),
//                            comments = g.Select(b => b.comments).Sum(),
//                            inboxes = g.Select(b => b.inboxes).Sum()
//                        }).ToList();
//                        break;
//                    default:
//                        foreach (var item in list)
//                        {
//                            item.name = item.date.ToString("dd/MM");
//                        }
//                        break;
//                }
//            return list;
//        }

//        public async Task<IEnumerable<ReportChatDataLine>> GetCustomerChatChartData(string business_id, ReportDataFilter filter)
//        {
//            Paging page = new Paging { Limit = 50, Previous = (filter.init_utc > 99999999999 ? filter.init_utc / 1000 : filter.init_utc).ToString(), Next = (filter.finish_utc > 99999999999 ? filter.finish_utc / 1000 : filter.finish_utc).ToString() };
//            IEnumerable<ReportChatDataLine> list = await _reportRepository.GetCustomerChatChartData(business_id, page);
//            if (list != null && list.Count() > 0)
//                switch (filter.period)
//                {
//                    case "week":
//                        var first = list.Count() > 0 ? list.First().date : DateTime.MinValue;
//                        list = list.GroupBy(a => (a.date - first).TotalDays / 7).Select(g => new ReportChatDataLine
//                        {
//                            name = g.First().date.ToString("dd/MM"),
//                            customers = g.Select(b => b.customers).Sum(),
//                            conversations = g.Select(b => b.conversations).Sum(),
//                            comments = g.Select(b => b.comments).Sum(),
//                            inboxes = g.Select(b => b.inboxes).Sum()
//                        }).ToList();
//                        break;
//                    case "month":
//                        list = list.GroupBy(a => a.date.Month).Select(g => new ReportChatDataLine
//                        {
//                            name = g.First().date.ToString("MM/yy"),
//                            customers = g.Select(b => b.customers).Sum(),
//                            conversations = g.Select(b => b.conversations).Sum(),
//                            comments = g.Select(b => b.comments).Sum(),
//                            inboxes = g.Select(b => b.inboxes).Sum()
//                        }).ToList();
//                        break;
//                    default:
//                        foreach (var item in list)
//                        {
//                            item.name = item.date.ToString("dd/MM");
//                        }
//                        break;
//                }
//            return list;
//        }

//        public async Task<IEnumerable<ReportCustomerModel>> GetCustomers(string business_id, ReportDataFilter filter)
//        {
//            Paging page = new Paging { Limit = filter.limit, Previous = (filter.init_utc > 99999999999 ? filter.init_utc / 1000 : filter.init_utc).ToString(), Next = (filter.finish_utc > 99999999999 ? filter.finish_utc / 1000 : filter.finish_utc).ToString() };
//            List<ReportCustomerModel> list = new List<ReportCustomerModel>();
//            foreach (var item in await _customerService.SearchCustomers(business_id,"","","","","", page))
//            {
//                list.Add(new ReportCustomerModel
//                {
//                    id = item.id,
//                    address = item.address,
//                    avatar = item.avatar,
//                    channel_id = item.channel_id,
//                    city = item.city,
//                    email = item.email,
//                    name = item.name,
//                    phone = item.phone,
//                    date_registered = item.timestamp
//                });
//            }
//            return list;

//        }

//        public async Task<IEnumerable<ReportResponseModel>> GetFirstResponseChartData(string business_id, ReportDataFilter filter)
//        {
//            List<ReportResponseModel> list = new List<ReportResponseModel>();
//            return list;

//        }

//        public async Task<IEnumerable<ReportMessageModel>> GetMessageChartData(string business_id, ReportDataFilter filter)
//        {
//            List<ReportMessageModel> list = new List<ReportMessageModel>();
//            return list;

//        }

//        public async Task<IEnumerable<ReportTicketDataLine>> GetTicketChartData(string business_id, ReportDataFilter filter)
//        {
//            Paging page = new Paging { Limit = filter.limit, Previous = Core.Helpers.CommonHelper.UnixTimestampToDateTime(filter.init_utc > 99999999999 ? filter.init_utc / 1000 : filter.init_utc).ToString(), Next = Core.Helpers.CommonHelper.UnixTimestampToDateTime(filter.finish_utc > 99999999999 ? filter.finish_utc / 1000 : filter.finish_utc).ToString() };
//            IEnumerable<ReportTicketDataLine> list = await _reportRepository.GetTicketChartData(business_id, page);
//            if (list != null && list.Count() > 0)
//                switch (filter.period)
//                {
//                    case "week":
//                        var first = list.Count() > 0 ? list.First().date : DateTime.MinValue;
//                        list = list.GroupBy(a => (a.date - first).TotalDays / 7).Select(g => new ReportTicketDataLine
//                        {
//                            name = g.First().date.ToString("dd/MM"),
//                            completed_tickets = g.Select(b => b.completed_tickets).Sum(),
//                            attention_tickets = g.Select(b => b.attention_tickets).Sum(),
//                            pending_tickets = g.Select(b => b.pending_tickets).Sum(),
//                            tickets = g.Select(b => b.tickets).Sum()
//                        }).ToList();
//                        break;
//                    case "month":
//                        list = list.GroupBy(a => a.date.Month).Select(g => new ReportTicketDataLine
//                        {
//                            name = g.First().date.ToString("MM/yy"),
//                            completed_tickets = g.Select(b => b.completed_tickets).Sum(),
//                            attention_tickets = g.Select(b => b.attention_tickets).Sum(),
//                            pending_tickets = g.Select(b => b.pending_tickets).Sum(),
//                            tickets = g.Select(b => b.tickets).Sum()
//                        }).ToList();
//                        break;
//                    default:
//                        foreach (var item in list)
//                        {
//                            item.name = item.date.ToString("dd/MM");
//                        }
//                        break;
//                }
//            return list;


//        }

//        public async Task<IEnumerable<Agent>> GetTopAgents(string business_id, Paging page)
//        {
//            return await _agentService.GetAgents(business_id, 0, page.Limit);
//        }

//        public async Task<IEnumerable<Thread>> GetTopCustomers(string business_id, Paging page)
//        {
//            return await _threadService.GetThreads(business_id, "", "", "", "", "", page);
//        }

//    }
//}
