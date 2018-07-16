using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hibaza.CCP.Domain.Models;
using Firebase.Database;
using Hibaza.CCP.Domain.Models.Facebook;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Service.Facebook;
using Newtonsoft.Json;
using Hangfire;

namespace Hibaza.CCP.Service.SQL
{
    public class ThreadService : IThreadService
    {
        private readonly IThreadRepository _threadRepository;
        private readonly ICounterRepository _counterRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IAgentService _agentService;
        private readonly ICounterService _counterService;
        private readonly IChannelService _channelService;
        private readonly IMessageService _messageService;
        private readonly IReferralService _referralService;
        private readonly IFirebaseThreadRepository _fbThreadRepository;

        public ThreadService(IThreadRepository threadRepository, IFirebaseThreadRepository fbThreadRepository, IReferralService referralService, IMessageService messageService, IChannelService channelService, IAgentService agentService, ICounterService counterService, ICounterRepository counterRepository, IMessageRepository messageRepository)
        {
            _threadRepository = threadRepository;
            _fbThreadRepository = fbThreadRepository;
            _messageRepository = messageRepository;
            _counterRepository = counterRepository;
            _counterService = counterService;
            _channelService = channelService;
            _agentService = agentService;
            _referralService = referralService;
            _messageService = messageService;
        }

        public static string FormatId(string parent, string key)
        {
            return Core.Helpers.CommonHelper.FormatKey("", key);
        }
        public static string FormatId01(string parent, string key)
        {
            return Core.Helpers.CommonHelper.FormatKey(parent, key);
        }

        public void BatchUpdateUnreadCounters(string business_id)
        {
            Dictionary<string, Counter> channelCounters = GetChannelCounters(business_id).Result.Where(c => !string.IsNullOrWhiteSpace(c.id)).ToDictionary(c => c.id, c => c);
            Dictionary<string, Counter> agentCounters = GetAgentCounters(business_id).Result.Where(a => !string.IsNullOrWhiteSpace(a.id)).ToDictionary(c => c.id, c => c);
            foreach (var a in _agentService.GetAgents(business_id, 0, 100).Result)

            {
                int count = agentCounters.ContainsKey(a.id) ? agentCounters[a.id].unread : 0;
                _counterService.SetAgentUnreadThreadsCount(business_id, a.id, count);
            }
            foreach (var c in _channelService.GetChannels(business_id, 0, 50).Result)
            {
                int count = channelCounters.ContainsKey(c.id) ? channelCounters[c.id].unread : 0;
                _counterService.SetChannelUnreadThreadsCount(business_id, c.id, count);
            }
        }

        public int UnAssignFromInActiveAgents(string business_id, Paging page)
        {
            int count = 0;
            var agents = _agentService.GetAgents(business_id, 0, 50).Result.Where(a => a.status != "online").ToDictionary(a => a.id, b => b);
            foreach (var agent in agents)
            {
                var threads = _threadRepository.GetActiveUnreadThreads(business_id, agent.Key, page).Result;
                foreach (var t in threads)
                {
                    t.agent_id = null;
                    t.status = "pending";
                    CreateThread(t, true);
                    count++;
                }
            }
            BatchUpdateUnreadCounters(business_id);
            return count;
        }



        public int AutoAssignToAvailableAgents(string business_id, Paging page)
        {
            UnAssignFromInActiveAgents(business_id, page);

            int count = 0;
            var agents = _agentService.GetAgents(business_id, 0, 50).Result.Where(a => a.status == "online").ToDictionary(a => a.id, b => b);
            List<string> agentKeys = agents.Select(a => a.Key).ToList();
            if (agentKeys.Count == 0) return count;

            Dictionary<string, Counter> counters = GetAgentCounters(business_id).Result.Where(a => !string.IsNullOrWhiteSpace(a.id)).ToDictionary(c => c.id, c => c);
            var threads = GetThreads(business_id, "", "", "pending", "unread", "", page).Result.OrderBy(t => t.timestamp);


            var total = 0;
            foreach (var agent in agents)
            {
                if (!counters.ContainsKey(agent.Key))
                {
                    counters.Add(agent.Key, new Counter());
                }
                total += counters[agent.Key].unread;
            }

            int everage = Math.Min((total + threads.Count()) / agentKeys.Count + 1, page.Limit); //TODO: Read from config

            int i = 0;
            foreach (var thread in threads.Where(t => string.IsNullOrWhiteSpace(t.agent_id)))
            {
                count++;
                while (i < agentKeys.Count && counters[agentKeys[i]].unread > everage) i++;
                if (i >= agentKeys.Count) break;
                counters[agentKeys[i]].unread++;
                thread.agent_id = agentKeys[i];
                thread.status = "active";
                CreateThread(thread, true);

            }


            BatchUpdateUnreadCounters(business_id);
            return count;
        }

        public bool AddLastVisit(string business_id, string thread_id, string url, string agent_id)
        {
            var thread = GetById(business_id, thread_id);
            if (thread == null) return false;
            List<string> last_visits = new List<string>();
            if (!string.IsNullOrWhiteSpace(thread.last_visits))
            {
                last_visits = JsonConvert.DeserializeObject<List<string>>(thread.last_visits);
            }
            if (last_visits.Count == 0 || last_visits.First() != url)
            {
                last_visits.Insert(0, url);
                if (last_visits.Count > 20) last_visits = last_visits.Take(20).ToList();

                return _threadRepository.UpdateLastVisits(business_id, thread_id, JsonConvert.SerializeObject(last_visits));
            }
            return false;
        }

        public bool AssignToAgent(string business_id, string thread_id, string agent_id)
        {
            var thread = UnAssignFromAgent(business_id, thread_id);

            var agent = !string.IsNullOrWhiteSpace(agent_id) ? _agentService.GetById(business_id, agent_id) : null;
            if (agent == null) return false;

            thread = GetById(business_id, thread_id);

            business_id = thread.business_id;
            var oldAgentId = thread.agent_id;
            thread.agent_id = agent_id;
            thread.status = "active";
            CreateThread(thread, true);

            var counter = new Counter { id = thread_id, count = 1 };
            _counterRepository.AddThreadToAgents(business_id, agent_id, counter);
            if (thread.unread) { _counterRepository.AddTheadToAgentsUnread(business_id, agent_id, counter); };


            return true;
        }


        public Thread UnAssignFromAgent(string business_id, string thread_id)
        {
            var thread = GetById(business_id, thread_id);
            if (thread == null) return thread;

            string agent_id = thread.agent_id;
            business_id = thread.business_id;
            thread.agent_id = "";
            thread.status = "pending";
            CreateThread(thread, true);

            if (string.IsNullOrWhiteSpace(agent_id)) return thread;

            //update counters
            var counter = new Counter { id = thread_id, count = 1 };
            try
            {
                _counterRepository.DeleteThreadFromAgentsAll(business_id, agent_id, thread_id);
            }
            catch { }
            try
            {
                _counterRepository.DeleteThreadFromAgentsUnread(business_id, agent_id, thread_id);
            }
            catch { }
            return thread;
        }

        //public void InsertConversation(string business_id, string conversation_id, string owner_ext_id, string owner_app_id, string link)
        //{
        //    _threadRepository.InsertConversation(business_id, conversation_id, owner_ext_id, owner_app_id, link);
        //}

        //public void UpsertConversation(string business_id, string conversation_id, string owner_ext_id, string owner_app_id, string link)
        //{
        //    _threadRepository.UpsertConversation(business_id, conversation_id, owner_ext_id, owner_app_id, link);
        //}

        //public FbConversation GetConversation(string business_id, string conversation_id)
        //{
        //    return _threadRepository.GetConversation(business_id, conversation_id);
        //}

        //public FbConversation GetConversationByOwnerExtId(string business_id, string owner_ext_id)
        //{
        //    return _threadRepository.GetConversationOwnerExtId(business_id, owner_ext_id);
        //}

        public Thread MarkAsRead(string business_id, string thread_id, string agent_id)
        {
            var thread = GetById(business_id, thread_id);
            business_id = thread.business_id;
            string channel_id = thread.channel_id;
            var previous_unread = thread.unread;
            if (!previous_unread) return null;
            if (thread.unread)
            {
                thread.unread = false;
                thread.read_by = agent_id;
                thread.read_at = Core.Helpers.CommonHelper.DateTimeToUnixTimestamp(DateTime.UtcNow);
                CreateThread(thread, true);

                //update counters
                var counter = new Counter { id = thread_id, count = 1 };
                if (!string.IsNullOrWhiteSpace(agent_id))
                {
                    try
                    {
                        _counterRepository.DeleteThreadFromAgentsUnread(business_id, agent_id, thread_id);
                    }
                    catch { }
                }
                try
                {
                    _counterRepository.DeleteThreadFromChannelsUnassignedUnread(business_id, thread.channel_id, thread_id);
                }
                catch { }
                try
                {
                    _counterRepository.DeleteThreadFromChannelsUnread(business_id, thread.channel_id, thread_id);
                }
                catch { }
            }
            return thread;
        }

        public Thread MarkAsUnRead(string business_id, string thread_id, string agent_id)
        {
            var thread = GetById(business_id, thread_id);
            business_id = thread.business_id;
            string channel_id = thread.channel_id;
            var previous_unread = thread.unread;
            if (previous_unread) return null;
            if (!thread.unread)
            {
                thread.unread = true;
                thread.read_at = 0;
                thread.read_by = null;
                CreateThread(thread, true);

                //update counters
                var counter = new Counter { id = thread_id, count = 1 };
                if (!string.IsNullOrWhiteSpace(agent_id))
                {
                    //AssignToAgent(business_id, thread_id, agent_id);
                    try
                    {
                        _counterRepository.AddTheadToAgentsUnread(business_id, agent_id, counter);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        _counterRepository.AddThreadToChannelsUnassignedUnread(business_id, thread.channel_id, counter);
                    }
                    catch { }
                }
                try
                {
                    _counterRepository.AddThreadToChannelsUnread(business_id, thread.channel_id, counter);
                }
                catch { }
            }
            return thread;
        }

        public Domain.Entities.Thread GetById(string business_id, string id)
        {
            return _threadRepository.GetById(business_id, id);
        }
        public Domain.Entities.Thread GetByIdFromCustomerId(string business_id, string customerId)
        {
            return _threadRepository.GetByIdFromCustomerId(business_id,customerId);
        }


        public string GetCustomerId(string business_id, string id)
        {
            string customerId = null;
            var thread = _threadRepository.GetById(business_id, id);
            if (thread != null)
            {
                customerId = thread.customer_id;
            }
            return customerId;
        }

        public int DeleteFromFirebaseByCustomer(string business_id, string customer_id)
        {
            int count = 0;
            foreach (var t in GetByCustomer(business_id, customer_id, new Paging { Limit = 1000 }).Result)
                if (!string.IsNullOrWhiteSpace(t.id))
                {
                    try
                    {
                        count++;
                        _fbThreadRepository.Delete(business_id, t.id);
                    }
                    catch { }
                }
            return count;
        }

        public int Delete(string business_id, string id)
        {
            //_mesageService.DeleteByThread(business_id, id);
            _fbThreadRepository.Delete(business_id, id);
            _threadRepository.Delete(business_id, id);
            return 1;
        }

        public bool CreateReferral(string business_id, FacebookMessagingEvent referralEvent)
        {
            var thread_id = ThreadService.FormatId(business_id, referralEvent.sender.id);
            var threads = _threadRepository.GetById(business_id, thread_id);
            string data = referralEvent.postback != null && referralEvent.postback.referral != null && !string.IsNullOrWhiteSpace(referralEvent.postback.referral.Ref) ? referralEvent.postback.referral.Ref : referralEvent.referral != null && !string.IsNullOrWhiteSpace(referralEvent.referral.Ref) ? referralEvent.referral.Ref : "";
            data = data ?? "";
            data = data.Trim();
            if (!string.IsNullOrWhiteSpace(thread_id) && !string.IsNullOrWhiteSpace(business_id) && !string.IsNullOrWhiteSpace(data))
            {
                var referral = _referralService.CreateReferral(business_id, threads, referralEvent.timestamp.ToString(), referralEvent.sender.id, referralEvent.recipient.id, data, thread_id);
            }
            return true;
        }

        public bool CopyThreadToRealTimeDB(string business_id, string id, long timestamp)
        {
            var thread = GetById(business_id, id);
            CopyThreadToRealTimeDB(business_id, thread);
            return true;
        }

        private bool CopyThreadToRealTimeDB(string business_id, Thread thread)
        {
            if (thread != null) _fbThreadRepository.Upsert(business_id, thread);
            return true;
        }

        public bool CreateThread(Domain.Entities.Thread thread, bool real_time_update)
        {
            var rs = _threadRepository.Upsert(thread.business_id, thread);
            if (real_time_update)
            {
                try
                {
                    CopyThreadToRealTimeDB(thread.business_id, thread);
                }
                catch
                {
                    BackgroundJob.Enqueue<ThreadService>(x => x.CopyThreadToRealTimeDB(thread.business_id, thread.id, thread.timestamp));
                }
            }
            if (rs > 0)
                return true;
            else
                return false;
        }

        public async Task<IEnumerable<Thread>> GetNoReponseThreads(string business_id, string channel_id, Paging page)
        {
            try
            {
                return await _threadRepository.GetNoReponseThreads(business_id, channel_id, page);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Domain.Entities.Thread>> GetThreads(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page)
        {
            keywords = keywords ?? "";
            keywords = keywords.Trim().Replace("  ", " ").ToLower();
            if (status == "unread")
            {
                flag = "unread";
                status = "";
            }
            if (status == "nonreply")
            {
                flag = "nonreply";
                status = "";
            }
            return await _threadRepository.SearchThreadsByKeywords(business_id, channel_id, agent_id, status, flag, keywords, page);
        }


        public async void UnAssignAllUnreadThreadsFromAgent(string business_id, string agent_id)
        {
            var list = _counterRepository.GetThreadUnreadCountByAgent(business_id, agent_id);
            if (list != null)
            {
                foreach (var t in list)
                {
                    try
                    {
                        UnAssignFromAgent(business_id, t.id);
                    }
                    catch { }

                }
            }
            await _counterRepository.DeleteAllUnreadThreadCountersFromAgent(business_id, agent_id);

        }

        public async Task<IEnumerable<Thread>> All(string business_id)
        {
            return _threadRepository.GetAll(business_id);
        }



        public int UpdateCustomerId()
        {
            return _threadRepository.UpdateCustomerId();
        }

        public bool UpdateCustomerId(string business_id, string thread_id)
        {
            var t = _threadRepository.GetById(business_id, thread_id);
            var user_id = FacebookConversationService.GetUniqueUserId(t.owner_avatar, t.owner_ext_id, t.owner_app_id);
            if (!string.IsNullOrWhiteSpace(user_id))
            {
                _threadRepository.UpdateCustomerId(business_id, t.id, user_id);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Thread>> GetThreadsWhereCustomerIsNull(string business_id, Paging page)
        {
            return await _threadRepository.GetThreadsWhereCustomerIsNull(business_id, page);
        }

        public int UpdateCustomerId(string business_id, int limit)
        {
            int count = 0;
            foreach (var t in _threadRepository.GetThreadsWhereCustomerIsNull(business_id, new Paging { Limit = limit }).Result)
            {
                var user_id = FacebookConversationService.GetUniqueUserId(t.owner_avatar, t.owner_ext_id, t.owner_app_id);
                if (!string.IsNullOrWhiteSpace(user_id))
                {
                    count++;
                    _threadRepository.UpdateCustomerId(business_id, t.id, user_id);
                }
            }
            return count;
        }


        public async Task<int> UpdateThreadStatus(string business_id, string status, int limit)
        {
            int count = 0;
            Paging page = new Paging { Limit = limit };
            foreach (var t in await GetThreads(business_id, "", "", status, "", "", page))
            {
                count++;
                RefreshThread(business_id, t.id, null);
            }
            return count;
        }

        public Thread RefreshThread(string business_id, string thread_id, string message_id)
        {
            Message m = null;
            if (!string.IsNullOrWhiteSpace(message_id))
            {
                m = _messageService.GetById(business_id, message_id);
            }
            if (m == null)
            {
                m = _messageService.GetCustomerOrAgentNonDeletedMessagesByThread(business_id, new Paging { Limit = 1, Next = "9999999999" }, thread_id).Result.FirstOrDefault();
            }
            if (m != null)
            {
                return CreateThread(business_id, thread_id, m, false);
            }
            return null;
        }

        public Thread RefreshThread(string business_id, string thread_id, bool force_update, bool real_time_update)
        {
            var thread = GetById(business_id, thread_id);
            var message = _messageService.GetCustomerOrAgentNonDeletedMessagesByThread(business_id, new Paging { Limit = 1, Next = "9999999999" }, thread_id).Result.FirstOrDefault();
            if (thread != null && message != null)
            {
                if (force_update)
                {
                    thread.timestamp = 0;
                }
                return CreateThread(business_id, thread, message, real_time_update);
            }
            return null;
        }

        public Thread CreateThread(string business_id, string thread_id, Message message, bool real_time_update)
        {
            var thread = GetById(business_id, thread_id);
            if (thread == null)
            {
                thread = new Domain.Entities.Thread
                {
                    id = thread_id,
                    owner_id = message.sender_ext_id == message.channel_ext_id ? message.recipient_id : message.sender_id,
                    owner_ext_id = message.sender_ext_id == message.channel_ext_id ? message.recipient_ext_id : message.sender_ext_id,
                    owner_avatar = message.sender_ext_id == message.channel_ext_id ? message.recipient_avatar : message.sender_avatar,
                    owner_name = message.sender_ext_id == message.channel_ext_id ? message.recipient_name : message.sender_name
                };
            }
            return CreateThread(business_id, thread, message, real_time_update);
        }

        public Thread CreateThread(string business_id, Thread thread, Message message, bool real_time_update)
        {
            thread.business_id = business_id;
            thread.channel_ext_id = !string.IsNullOrWhiteSpace(message.channel_ext_id) ? message.channel_ext_id : thread.channel_ext_id;
            thread.channel_id = !string.IsNullOrWhiteSpace(message.channel_id) ? message.channel_id : thread.channel_id;
            thread.channel_type = !string.IsNullOrWhiteSpace(message.channel_type) ? message.channel_type : thread.channel_type;
            thread.type = !string.IsNullOrWhiteSpace(message.thread_type) ? message.thread_type : thread.type;

            //if (thread.timestamp <= message.timestamp && !message.deleted && (!string.IsNullOrWhiteSpace(message.agent_id) || message.sender_ext_id!=message.channel_ext_id))
            if (thread.timestamp <= message.timestamp && !message.deleted)
            {
                thread.updated_time = message.created_time;
                thread.last_message_ext_id = message.ext_id;
                thread.last_message_parent_ext_id = message.parent_ext_id;
                thread.owner_last_message_ext_id = message.sender_ext_id == message.channel_ext_id ? thread.owner_last_message_ext_id : message.ext_id;
                thread.owner_last_message_parent_ext_id = message.sender_ext_id == message.channel_ext_id ? thread.owner_last_message_parent_ext_id : message.parent_ext_id;
                //thread.last_message = (string.IsNullOrWhiteSpace(thread.last_message) || message.sender_ext_id != message.channel_ext_id || !string.IsNullOrWhiteSpace(message.agent_id)) ? message.message : thread.last_message;
                thread.last_message = message.message;
                thread.owner_timestamp = message.sender_id == thread.owner_id ? message.timestamp : thread.owner_timestamp;
                thread.sender_id = message.sender_id;
                thread.sender_ext_id = message.sender_ext_id;
                thread.sender_name = thread.owner_name;
                thread.sender_avatar = thread.sender_avatar;
                thread.timestamp = message.timestamp;
                thread.link_ext_id = message.root_ext_id;

                //thread.unread = thread.channel_ext_id != message.sender_ext_id || (string.IsNullOrWhiteSpace(message.agent_id) && thread.type == "message");
                thread.unread = thread.channel_ext_id != message.sender_ext_id;
                // thread.nonreply = thread.channel_ext_id != message.sender_ext_id || (string.IsNullOrWhiteSpace(message.agent_id) && thread.type == "message");
                thread.nonreply = thread.channel_ext_id != message.sender_ext_id;
            }

            var oavatar = (message.sender_ext_id == message.channel_ext_id ? message.recipient_avatar : message.sender_avatar);
            thread.owner_avatar = string.IsNullOrWhiteSpace(oavatar) ? thread.owner_avatar : oavatar;
            var oname = message.sender_ext_id == message.channel_ext_id ? message.recipient_name : message.sender_name;
            thread.owner_name = string.IsNullOrWhiteSpace(oname) ? thread.owner_name : oname;

            thread.ext_id = string.IsNullOrWhiteSpace(thread.ext_id) ? message.conversation_ext_id : thread.ext_id;
            thread.status = string.IsNullOrWhiteSpace(thread.agent_id) ? "pending" : "active";
            thread.sender_name = thread.owner_name;

            CreateThread(thread, real_time_update);

            return thread;
        }

        public async Task<IEnumerable<Counter>> GetChannelCounters(string business_id)
        {
            return await _threadRepository.GetChannelCounters(business_id);
        }

        public async Task<IEnumerable<Counter>> GetAgentCounters(string business_id)
        {
            return await _threadRepository.GetAgentCounters(business_id);
        }

        public async Task<IEnumerable<Thread>> GetByCustomer(string business_id, string customer_id, Paging page)
        {
            return await _threadRepository.GetThreadsByCustomer(business_id, customer_id, page);
        }

        public async Task<int> Job_AutoCallProcedure(string procedure)
        {
            return await _threadRepository.Job_AutoCallProcedure(procedure);
        }
    }
}
