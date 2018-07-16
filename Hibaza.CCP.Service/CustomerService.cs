//using Hibaza.CCP.Data.Infrastructure;
//using Hibaza.CCP.Data.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Hibaza.CCP.Domain.Models;
//using Firebase.Database;
//using Hibaza.CCP.Domain.Models.Facebook;
//using Hibaza.CCP.Domain.Entities;
//using Hibaza.CCP.Service.Facebook;
//using Newtonsoft.Json;
//using Hibaza.CCP.Data.Repositories.Firebase;
//using Hangfire;

//namespace Hibaza.CCP.Service
//{
//    public class CustomerService : ICustomerService
//    {
//        private readonly ICustomerRepository _customerRepository;
//        private readonly ICustomerCounterService _counterService;
//        private readonly IFirebaseCustomerRepository _fbCustomerRepository;
//        private readonly IChannelService _channelService;
//        private readonly IAgentService _agentService;
//        private readonly IThreadRepository _threadRepository;
//        private readonly ITicketService _ticketService;
//        public CustomerService(ICustomerRepository customerRepository, ITicketService ticketService, IThreadRepository threadRepository, IFirebaseCustomerRepository fbCustomerRepository, IAgentService agentService, IChannelService channelService, ICustomerCounterService counterService)
//        {
//            _customerRepository = customerRepository;
//            _threadRepository = threadRepository;
//            _ticketService = ticketService;
//            _fbCustomerRepository = fbCustomerRepository;
//            _counterService = counterService;
//            _agentService = agentService;
//            _channelService = channelService;
//        }

//        public static string FormatId(string parent, string key)
//        {
//            return Core.Helpers.CommonHelper.FormatKey("", key);
//        }

//        public Domain.Entities.Customer GetById(string business_id, string id)
//        {
//            return _customerRepository.GetById(business_id, id);
//        }

//        public async Task<int> AutoAssignToAvailableAgents(string business_id, Paging page)
//        {
//            var agents = (await _agentService.GetAgents(business_id, 0, 50)).ToDictionary(a => a.id, b => b);
//            var channels = (await _channelService.GetChannels(business_id, 0, 50)).ToDictionary(a => a.id, b => b);

//            UnAssignFromInActiveAgents(business_id, channels, agents, new Paging { Limit = 1000 });


//            Dictionary<string, Counter> counters = GetAgentCounters(business_id).Result.Where(a => !string.IsNullOrWhiteSpace(a.id)).ToDictionary(c => c.id, c => c);
//            var customers = (await GetPendingUnreadCustomers(business_id, new Paging { Limit = 1000 })).Where(c => channels.ContainsKey(c.channel_id));

//            int count = 0;
//            List<string> agentKeys = agents.Where(a => a.Value.status == "online").OrderBy(b => counters.ContainsKey(b.Key) ? counters[b.Key].unread : 0).Select(a => a.Key).ToList();

//            if (agentKeys.Count == 0) return count;

//            var total = 0;
//            foreach (var agent in agents.Where(a => a.Value.status == "online"))
//            {
//                if (!counters.ContainsKey(agent.Key))
//                {
//                    counters.Add(agent.Key, new Counter());
//                }
//                total += counters[agent.Key].unread;
//            }
//            int everage = Math.Min((total + customers.Count()) / agentKeys.Count + 1, page.Limit); //TODO: Read from config

//            int i = 0;
//            foreach (var customer in customers.Where(t => string.IsNullOrWhiteSpace(t.agent_id)))
//            {
//                count++;
//                while (i < agentKeys.Count && counters[agentKeys[i]].unread > everage) i++;
//                if (i >= agentKeys.Count) break;
//                counters[agentKeys[i]].unread++;
//                customer.agent_id = agentKeys[i];
//                customer.status = "active";
//                customer.assigned_by = null;
//                customer.assigned_at = null;
//                CreateCustomer(customer, true);

//            }
//            return count;
//        }


//        private string FindTheBestAvailableAgent(string business_id, int limit)
//        {
//            var agents = _agentService.GetOnlineAgents(business_id, 0, 50).Result;
//            Dictionary<string, Counter> counters = GetAgentCounters(business_id).Result.Where(a => !string.IsNullOrWhiteSpace(a.id)).ToDictionary(c => c.id, c => c);
//            List<string> agentKeys = agents.Where(a => counters.ContainsKey(a.id)).OrderBy(b => counters.ContainsKey(b.id) ? counters[b.id].unread : 0).Select(a => a.id).ToList();
//            return agentKeys.Count > 0 && counters[agentKeys[0]].unread <= limit ? agentKeys[0] : null;
//        }

//        public int UnAssignFromInActiveAgents(string business_id, Dictionary<string, Channel> channels, Dictionary<string, Agent> agents, Paging page)
//        {
//            int count = 0;
//            List<string> agentKeys = agents.Where(a => a.Value.status == "online" || a.Value.status == "busy").Select(a => a.Key).ToList();
//            var customers = GetActiveUnreadCustomers(business_id, page).Result;
//            foreach (var t in customers)
//            {
//                //t.agent_id = (agentKeys.Contains(t.agent_id) && (agents[t.agent_id].status == "online" || t.agent_id != t.assigned_by)) ? t.agent_id : null;
//                t.agent_id = (agentKeys.Contains(t.agent_id) && (agents[t.agent_id].status == "online" || t.agent_id == t.assigned_by)) ? t.agent_id : null;
//                t.status = channels.ContainsKey(t.channel_id) ? (string.IsNullOrWhiteSpace(t.agent_id) ? "pending" : "active") : null;
//                if (t.status != "active")
//                {
//                    t.assigned_by = null;
//                    t.assigned_at = null;
//                    CreateCustomer(t, true);
//                    count++;
//                }

//            }
//            return count;
//        }



//        //public int UpdateUserId(string business_id, int limit)
//        //{
//        //    int count = 0;
//        //    foreach (var t in _customerRepository.GetCustomers(business_id, new Paging { Limit = limit }).Result)
//        //    {
//        //        var user_id = FacebookConversationService.GetUniqueUserId(t.avatar, t.ext_id);
//        //        if (!string.IsNullOrWhiteSpace(user_id))
//        //        {
//        //            count++;
//        //            _customerRepository.UpdateUserId(business_id, t.key, user_id);
//        //        }
//        //    }
//        //    return count;
//        //}

//        public void UpdateContactInfo(string business_id, string customer_id, CustomerContactInfoModel data)
//        {
//            var customer = GetById(business_id, customer_id);
//            IEnumerable<string> phoneList = string.IsNullOrWhiteSpace(customer.phone_list) ? new List<string>() : JsonConvert.DeserializeObject<IEnumerable<string>>(customer.phone_list);

//            if (!string.IsNullOrWhiteSpace(data.phone))
//            {
//                var list = new List<string>();
//                list.Add(data.phone);
//                phoneList = phoneList.Union(list);
//                data.phone_list = JsonConvert.SerializeObject(phoneList);
//            }
//            _customerRepository.UpdateContactInfo(business_id, customer_id, data);
//        }

//        public void UpdatePhoneNumber(string business_id, string customer_id, IEnumerable<string> phoneList, string lastPhoneNumber)
//        {
//            _customerRepository.UpdatePhoneNumber(business_id, customer_id, JsonConvert.SerializeObject(phoneList), lastPhoneNumber);
//        }

//        public bool Block(string business_id, string customer_id, bool blocked)
//        {
//            return _customerRepository.Block(business_id, customer_id, blocked);
//        }

//        public async Task<int> UpdateCustomerStatus(string business_id, string status, int limit)
//        {
//            int count = 0;
//            Paging page = new Paging { Limit = limit };
//            foreach (var t in await SearchCustomers(business_id, "", "", status, "", "", new Paging { Limit = limit }))
//            {
//                count++;
//                RefreshCustomer(business_id, t.id);
//            }
//            return count;
//        }

//        public Customer RefreshCustomer(string business_id, string customer_id)
//        {
//            var threads = _threadRepository.GetThreadsByCustomer(business_id, customer_id, new Paging { Limit = 1 }).Result;
//            Thread thread = null;
//            if (threads != null && threads.Count() > 0) thread = threads.OrderByDescending(t => t.timestamp).First();
//            return RefreshCustomer(business_id, customer_id, thread);
//        }

//        private Customer RefreshCustomer(string business_id, string customer_id, Thread thread)
//        {
//            var customer = GetById(business_id, customer_id);
//            if (thread != null)
//            {
//                customer.active_thread = null;
//                return CreateCustomer(customer, thread, true).Result;
//            }
//            else
//            {
                
//                customer = UpdateCustomerStatus(customer).Result;
//                CreateCustomer(customer, true);
//                return customer;
//            }
//        }

//        public Customer CreateCustomer(string business_id, string customer_id, Thread thread, bool real_time_update)
//        {
//            var customer = GetById(business_id, customer_id);
//            if (customer == null)
//            {
//                customer = new Domain.Entities.Customer
//                {
//                    id = customer_id,
//                    ext_id = thread.owner_ext_id,
//                    app_id = thread.owner_app_id,
//                    name = thread.owner_name,
//                    avatar = thread.owner_avatar,
//                    timestamp = thread.timestamp,
//                    business_id = business_id,
//                    channel_id = thread.channel_id,
//                    created_time = thread.created_time,
//                    updated_time = thread.updated_time
//                };
//            }
//            return CreateCustomer(customer, thread, real_time_update).Result;
//        }


//        private async Task<Customer> UpdateCustomerStatus(Customer customer)
//        {
//            var threads = await _threadRepository.GetUnreadOrNonReplyThreadsByCustomer(customer.business_id, customer.id, new Paging { Limit = 100 });
//            customer.nonreply = false;
//            customer.unread = false;
//            customer.open = false;
//            var ticket = await _ticketService.GetCustomerLastActiveTicket(customer.business_id, customer.id);
//            if (ticket!=null)
//            {
//                customer.open = ticket.status == 0 || ticket.status == 1; //pending or attention
//                customer.active_ticket = JsonConvert.SerializeObject(ticket);
//            }

//            foreach (var t in threads)
//            {
//                customer.unread = customer.unread || t.unread;
//                customer.nonreply = customer.nonreply || t.nonreply;
//            }

//            if (customer.unread && string.IsNullOrWhiteSpace(customer.agent_id)){
//                customer.agent_id = FindTheBestAvailableAgent(customer.business_id, 3);
//            }
//            customer.status = string.IsNullOrWhiteSpace(customer.agent_id) ? "pending" : "active";
//            return customer;
//        }

//        public async Task<Customer> UpdateCustomerLastTicket(string business_id, string customer_id, bool real_time_update)
//        {
//            var customer = GetById(business_id, customer_id);
//            customer.open = false;
//            var ticket = await _ticketService.GetCustomerLastActiveTicket(customer.business_id, customer.id);
//            if (ticket != null)
//            {
//                customer.open = ticket.status == 0 || ticket.status == 1; //pending or attention
//                customer.active_ticket = JsonConvert.SerializeObject(ticket);
//            }

//            CreateCustomer(customer, real_time_update);
//            return customer;
//        }

//        public async Task<Customer> UpdateCustomerStatusAccordingToThread(string business_id, string customer_id, string thread_id, bool real_time_update)
//        {
//            var thread = _threadRepository.GetById(business_id, thread_id);
//            var customer = GetById(business_id, customer_id);
//            if (thread != null && customer != null)
//            {
//                return await CreateCustomer(customer, thread, real_time_update);
//            }
//            return null;
//        }

//        public async Task<Customer> CreateCustomer(Customer customer, Thread thread, bool real_time_update)
//        {
//            if (real_time_update)
//                customer = await UpdateCustomerStatus(customer);

//            if (customer.timestamp <= thread.timestamp || customer.active_thread == null)
//            {
//                customer.active_thread = JsonConvert.SerializeObject(thread);
//                customer.channel_id = thread.channel_id;
//                customer.timestamp = thread.timestamp;
//            }


//            CreateCustomer(customer, real_time_update);
//            return customer;
//        }

//        public bool CopyCustomerToRealTimeDB(string business_id, string id)
//        {
//            var customer = GetById(business_id, id);
//            CopyCustomerToRealTimeDB(business_id, customer);
//            return true;
//        }

//        private bool CopyCustomerToRealTimeDB(string business_id, Customer customer)
//        {
//            if (customer != null)
//            {
//                _fbCustomerRepository.Upsert(customer.business_id, customer);
//            }
//            return true;
//        }

//        public bool CreateCustomer(Domain.Entities.Customer customer, bool real_time_update)
//        {
//            _customerRepository.Upsert(customer);

//            if (real_time_update)
//            {
//                //var current = GetById(customer.business_id, customer.id);
//                //if (current != null) _counterService.UpdateCustomerCounters(customer.business_id, current, true);
//                //_counterService.UpdateCustomerCounters(customer.business_id, customer, false);
//                try {
//                    CopyCustomerToRealTimeDB(customer.business_id, customer);
//                }
//                catch
//                {
//                    BackgroundJob.Enqueue<CustomerService>(x => x.CopyCustomerToRealTimeDB(customer.business_id, customer.id));
//                }
//            }

//            return true;
//        }

//        public async Task<IEnumerable<Customer>> All(string business_id)
//        {
//            return _customerRepository.GetAll(business_id);
//        }

//        public async Task<IEnumerable<Customer>> GetPendingUnreadCustomers(string business_id, Paging page)
//        {
//            return await _customerRepository.GetPendingUnreadCustomers(business_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetActiveUnreadCustomers(string business_id, Paging page)
//        {
//            return await _customerRepository.GetActiveUnreadCustomers(business_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetPendingUnreadCustomers(string business_id, string channel_id, Paging page)
//        {
//            return await _customerRepository.GetPendingUnreadCustomers(business_id, channel_id, page);
//        }

//        public async Task<IEnumerable<Customer>> SearchCustomers(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page)
//        {
//            keywords = keywords ?? "";
//            keywords = keywords.Trim().Replace("  ", " ").ToLower();
//            if (status == "unread")
//            {
//                flag = "unread";
//                status = "";
//            }
//            if (status == "nonreply")
//            {
//                flag = "nonreply";
//                status = "";
//            }
//            if (status == "open")
//            {
//                flag = "open";
//                status = "";
//            }
            
//            return await _customerRepository.SearchCustomersByKeywords(business_id, channel_id, agent_id, status, flag, keywords, page);
//        }

//        public async Task<IEnumerable<Counter>> GetChannelCounters(string business_id)
//        {
//            return await _customerRepository.GetChannelCounters(business_id);
//        }

//        public async Task<IEnumerable<Counter>> GetAgentCounters(string business_id)
//        {
//            return await _customerRepository.GetAgentCounters(business_id);
//        }


//        public void BatchUpdateUnreadCounters(string business_id)
//        {
//            var channels = _channelService.GetChannels(business_id, 0, 50).Result.ToDictionary(a => a.id, b => b);
//            var agents = _agentService.GetAgents(business_id, 0, 100).Result.ToDictionary(a => a.id, b => b);
//            BatchUpdateUnreadCounters(business_id, channels, agents);
//        }

//        public void BatchUpdateUnreadCounters(string business_id, Dictionary<string, Channel> channels, Dictionary<string, Agent> agents)
//        {
//            Dictionary<string, Counter> channelCounters = GetChannelCounters(business_id).Result.Where(c => !string.IsNullOrWhiteSpace(c.id)).ToDictionary(c => c.id, c => c);
//            Dictionary<string, Counter> agentCounters = GetAgentCounters(business_id).Result.Where(a => !string.IsNullOrWhiteSpace(a.id)).ToDictionary(c => c.id, c => c);
//            foreach (var a in agents)
//            {
//                int count = agentCounters.ContainsKey(a.Key) ? agentCounters[a.Key].unread : 0;
//                _counterService.SetAgentUnreadCustomersCount(business_id, a.Key, count);
//            }
//            foreach (var c in channels)
//            {
//                int count = channelCounters.ContainsKey(c.Key) ? channelCounters[c.Key].unread : 0;
//                _counterService.SetChannelUnreadCustomersCount(business_id, c.Key, count);
//            }
//        }

//        public bool AssignToAgent(string business_id, string customer_id, string agent_id, string assigned_by)
//        {
//            var customer = UnAssignFromAgent(business_id, customer_id);

//            var agent = !string.IsNullOrWhiteSpace(agent_id) ? _agentService.GetById(business_id, agent_id) : null;
//            if (agent == null) return false;

//            customer = GetById(business_id, customer_id);

//            business_id = customer.business_id;
//            var oldAgentId = customer.agent_id;
//            customer.agent_id = agent_id;
//            customer.status = "active";
//            customer.assigned_by = assigned_by;
//            customer.assigned_at = DateTime.UtcNow;
//            CreateCustomer(customer, true);

//            //var counter = new Counter { id = customer_id, count = 1 };
//            //_counterService.AddSingleCustomerToAgent(business_id, agent_id, counter);
//            //if (customer.unread) { _counterService.AddSingleUnreadCustomerToAgent(business_id, agent_id, counter); };


//            return true;
//        }
//        public Customer UnAssignFromAgent(string business_id, string customer_id)
//        {
//            var customer = GetById(business_id, customer_id);
//            if (customer == null) return customer;

//            string agent_id = customer.agent_id;
//            business_id = customer.business_id;
//            customer.agent_id = null;
//            customer.status = "pending";
//            customer.assigned_by = null;
//            customer.assigned_at = null;
//            CreateCustomer(customer, true);

//            //if (string.IsNullOrWhiteSpace(agent_id)) return customer;

//            ////update counters
//            //var counter = new Counter { id = customer_id, count = 1 };
//            //try
//            //{
//            //   _counterService.DeleteSingleCustomerFromAgentAll(business_id, agent_id, customer_id);
//            //}
//            //catch { }
//            //try
//            //{
//            //   _counterService.DeleteSingleUnreadCustomerFromAgent(business_id, agent_id, customer_id);
//            //}
//            //catch { }
//            return customer;
//        }



//        public bool UpdateReferral(FacebookMessagingEvent referralEvent)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetAppIdByExtId(string business_id, string ext_id)
//        {
//            throw new NotImplementedException();
//        }

//        public void MapExtIdAndAppId(string business_id, string ext_id, string app_id, string @ref)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<IEnumerable<Customer>> GetUnreadCustomers(string business_id, Paging page)
//        {
//            return await _customerRepository.GetUnreadCustomers(business_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetUnreadCustomersByChannel(string business_id, string channel_id, Paging page)
//        {
//            return await _customerRepository.GetUnreadCustomersByChannel(business_id, channel_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetUnreadCustomersByAgent(string business_id, string agent_id, Paging page)
//        {
//            return await _customerRepository.GetUnreadCustomersByAgent(business_id, agent_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetUnreadCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
//        {
//            return await _customerRepository.GetUnreadCustomersByChannelAndAgent(business_id, channel_id, agent_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetCustomersWhereExtIdIsNull(string business_id, Paging page)
//        {
//            return await _customerRepository.GetCustomersWhereExtIdIsNull(business_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetNonReplyCustomers(string business_id, Paging page)
//        {
//            return await _customerRepository.GetNonReplyCustomers(business_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetNonReplyCustomersByChannel(string business_id, string channel_id, Paging page)
//        {
//            return await _customerRepository.GetNonReplyCustomersByChannel(business_id, channel_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetNonReplyCustomersByAgent(string business_id, string agent_id, Paging page)
//        {
//            return await _customerRepository.GetNonReplyCustomersByAgent(business_id, agent_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetNonReplyCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
//        {
//            return await _customerRepository.GetNonReplyCustomersByChannelAndAgent(business_id, channel_id, agent_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetAllCustomers(string business_id, Paging page)
//        {
//            return await _customerRepository.GetAllCustomers(business_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetOpenCustomers(string business_id, Paging page)
//        {
//            return await _customerRepository.GetOpenCustomers(business_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetAllCustomersByChannel(string business_id, string channel_id, Paging page)
//        {
//            return await _customerRepository.GetAllCustomersByChannel(business_id, channel_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetOpenCustomersByChannel(string business_id, string channel_id, Paging page)
//        {
//            return await _customerRepository.GetOpenCustomersByChannel(business_id, channel_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetAllCustomersByAgent(string business_id, string agent_id, Paging page)
//        {
//            return await _customerRepository.GetAllCustomersByAgent(business_id, agent_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetOpenCustomersByAgent(string business_id, string agent_id, Paging page)
//        {
//            return await _customerRepository.GetOpenCustomersByAgent(business_id, agent_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetAllCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
//        {
//            return await _customerRepository.GetAllCustomersByChannelAndAgent(business_id, channel_id, agent_id, page);
//        }

//        public async Task<IEnumerable<Customer>> GetOpenCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
//        {
//            return await _customerRepository.GetOpenCustomersByChannelAndAgent(business_id, channel_id, agent_id, page);
//        }
//    }
//}




