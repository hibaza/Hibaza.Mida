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

namespace Hibaza.CCP.Service.Firebase
{
    public class FirebaseThreadService : IFirebaseThreadService
    {
        private readonly IFirebaseThreadRepository _threadRepository;
        private readonly ICounterRepository _counterRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IAgentService _agentService;
        private readonly ICounterService _counterService;
        private readonly IChannelService _channelService;
        private readonly IFirebaseMessageService _mesageService;
        private readonly IReferralService _referralService;
        public FirebaseThreadService(IFirebaseThreadRepository threadRepository, IReferralService referralService, IFirebaseMessageService messageService, IChannelService channelService, IAgentService agentService, ICounterService counterService, ICounterRepository counterRepository, IMessageRepository messageRepository)
        {
            _threadRepository = threadRepository;
            _messageRepository = messageRepository;
            _counterRepository = counterRepository;
            _counterService = counterService;
            _channelService = channelService;
            _agentService = agentService;
            _referralService = referralService;
            _mesageService = messageService;
        }

        public static string FormatId(string parent, string key)
        {
            return Core.Helpers.CommonHelper.FormatKey("", key);
        }

        public void BatchUpdateUnreadCounters(string business_id)
        {
            _counterService.DeleteAllUnreadCountersData(business_id);
            foreach (var t in _threadRepository.GetUnreadThreads(business_id, new Paging { Limit = 1000 }))
            {
                _counterService.AddUnreadThreadCountersData(business_id, t);
            }
            foreach (var a in _agentService.GetAgents(business_id, 0, 100).Result)
            {
                _counterService.RefreshAgentUnreadThreadsCount(business_id, a.id);
            }
            foreach (var c in _channelService.GetChannels(business_id, 0, 50).Result)
            {
                _counterService.RefreshChannelUnreadThreadsCount(business_id, c.id);
            }
        }


        //public string FindAndAssignToBestAgent(string business_id, string thread_id, string old_agent_id)
        //{
        //    var agent = _agentService.FindBestAgent(business_id, old_agent_id);
        //    var new_agent_id = agent == null ? old_agent_id : agent.id;
        //    if (!string.IsNullOrWhiteSpace(new_agent_id) && new_agent_id != old_agent_id)
        //    {
        //        var thread = GetById(business_id, thread_id);
        //        thread.agent_id = new_agent_id;
        //        thread.status = "active";
        //        CreateThread(thread);

        //        var counter = new Counter { id = thread_id, count = 1 };
        //        if (!string.IsNullOrWhiteSpace(old_agent_id))
        //            try
        //            {
        //                _counterRepository.DeleteThreadFromAgentsAll(business_id, old_agent_id, thread_id);
        //            }
        //            catch { }

        //        _counterRepository.AddThreadToAgents(business_id, new_agent_id, counter);

        //        if (thread.unread)
        //        {
        //            if (!string.IsNullOrWhiteSpace(old_agent_id))
        //                try
        //                {
        //                    _counterRepository.DeleteThreadFromAgentsUnread(business_id, old_agent_id, thread_id);
        //                }
        //                catch { }
        //            _counterRepository.AddTheadToAgentsUnread(business_id, new_agent_id, counter);
        //        }

        //    }
        //    return new_agent_id;
        //}


        public bool AssignToAgent(string business_id, string thread_id, string agent_id)
        {
            var thread = UnAssignFromAgent(business_id, thread_id);

            var agent = !string.IsNullOrWhiteSpace(agent_id) ? _agentService.GetById(business_id, agent_id) : null;
            if (agent == null || agent.status != "online") return false;

            thread = GetById(business_id, thread_id);

            business_id = thread.business_id;
            var oldAgentId = thread.agent_id;
            thread.agent_id = agent_id;
            thread.status = "active";
            CreateThread(thread);


            //update counters
            //if (!string.IsNullOrWhiteSpace(oldAgentId) && oldAgentId != agent_id)
            //{
            //try
            //{
            //    _counterRepository.DeleteThreadFromAgentsAll(business_id, oldAgentId, thread_id);
            //}
            //catch { }
            //try
            //{
            //    _counterRepository.DeleteThreadFromAgentsUnread(business_id, oldAgentId, thread_id);
            //}
            //catch { }
            //}

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
            CreateThread(thread);

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

        public void MapThreadWithConversation(string business_id, string thread_id, string conversation_id, bool done)
        {
            _threadRepository.MapThreadWithConversation(business_id, thread_id, conversation_id, done);
        }

        public string GetThreadByConversation(string business_id, string conversation_id)
        {
            return _threadRepository.GetThreadByConversation(business_id, conversation_id);
        }

        public string MarkAsRead(string business_id, string thread_id)
        {
            var thread = GetById(business_id, thread_id);
            business_id = thread.business_id;
            var agent_id = thread.agent_id;
            string channel_id = thread.channel_id;
            var previous_unread = thread.unread;
            if (!previous_unread) return channel_id + "|" + agent_id;
            thread.unread = false;
            CreateThread(thread);

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

            return channel_id + "|" + agent_id;
        }

        public string MarkAsUnRead(string business_id, string thread_id)
        {
            var thread = GetById(business_id, thread_id);
            business_id = thread.business_id;
            var agent_id = thread.agent_id;
            string channel_id = thread.channel_id;
            var previous_unread = thread.unread;
            if (previous_unread) return channel_id + "|" + agent_id;
            thread.unread = true;
            CreateThread(thread);

            //update counters
            var counter = new Counter { id = thread_id, count = 1 };
            if (!string.IsNullOrWhiteSpace(agent_id))
            {
                AssignToAgent(business_id, thread_id, agent_id);
                //_counterRepository.AddTheadToAgentsUnread(business_id, agent_id, counter);
            }
            else
            {
                _counterRepository.AddThreadToChannelsUnassignedUnread(business_id, thread.channel_id, counter);
            }
            _counterRepository.AddThreadToChannelsUnread(business_id, thread.channel_id, counter);

            return channel_id + "|" + agent_id;
        }

        public Domain.Entities.Thread GetById(string business_id, string id)
        {
            return _threadRepository.GetById(business_id, id);
        }


        public int Delete(string business_id, string id)
        {
            _mesageService.DeleteByThread(business_id, id);
            _threadRepository.Delete(business_id, id);
            return 1;
        }

        public bool CreateReferral(string business_id, FacebookMessagingEvent referralEvent)
        {
            var thread_id = FirebaseThreadService.FormatId(business_id, referralEvent.sender.id);
            var threads = _threadRepository.GetById(business_id, thread_id);
            string data = referralEvent.postback != null && referralEvent.postback.referral != null && !string.IsNullOrWhiteSpace(referralEvent.postback.referral.Ref) ? referralEvent.postback.referral.Ref : referralEvent.referral != null && !string.IsNullOrWhiteSpace(referralEvent.referral.Ref) ? referralEvent.referral.Ref : "";
            data = data ?? "";
            data = data.Trim();
            if (!string.IsNullOrWhiteSpace(thread_id) && !string.IsNullOrWhiteSpace(business_id) && !string.IsNullOrWhiteSpace(data))
            {
                var referral = _referralService.CreateReferral(business_id, threads, referralEvent.timestamp.ToString(), referralEvent.sender.id, referralEvent.recipient.id, data,thread_id);
                //_threadRepository.CreateReferral(business_id, thread_id, referral.id, referral.product_sku);
                //var thread = _threadRepository.GetById(business_id, thread_id);
                //thread.referral_id = referral.id;
                //_threadRepository.Add(business_id, thread);
            }
            return true;
        }

        public bool CreateThread(Domain.Entities.Thread thread)
        {
            string strTimestamp = thread.timestamp.ToString();
            string strUnread = (thread.unread ? "unread" : "read");
            var status = string.IsNullOrWhiteSpace(thread.agent_id) ? "pending" : "active";
            thread.status = status;
            //thread.flag_timestamp = strUnread + strTimestamp;
            //thread.channel_flag_timestamp = thread.channel_id + strUnread + strTimestamp;
            //thread.agent_flag_timestamp = thread.agent_id + strUnread + strTimestamp;
            //thread.channel_agent_flag_timestamp = thread.channel_id + thread.agent_id + strUnread + strTimestamp;
            //thread.channel_timestamp = thread.channel_id + strTimestamp;
            //thread.channel_agent_timestamp = thread.channel_id + thread.agent_id + strTimestamp;
            //thread.channel_status_timestamp = thread.channel_id + status + strTimestamp;
            //thread.channel_agent_status_timestamp = thread.channel_id + thread.agent_id + status + strTimestamp;
            //thread.agent_status_timestamp = thread.agent_id + status + strTimestamp;
            //thread.agent_timestamp = thread.agent_id + strTimestamp;
            //thread.status_timestamp = status + strTimestamp;
            _threadRepository.Upsert(thread.business_id, thread);
            return true;
        }

        public async Task<IEnumerable<Domain.Entities.Thread>> GetThreads(string business_id, Paging page, string channelId, string agentId, string status, string flag)
        {
            if (string.IsNullOrWhiteSpace(channelId))
            {
                if (string.IsNullOrWhiteSpace(agentId))
                {
                    if (string.IsNullOrWhiteSpace(status))
                    {
                        if (!string.IsNullOrWhiteSpace(flag))
                        {
                            return await _threadRepository.GetThreadsByFlag(business_id, page, flag);
                        }
                        else
                        {
                            return await _threadRepository.GetThreads(business_id, page);
                        }
                    }
                    else
                    {
                        return await _threadRepository.GetThreadsByStatus(business_id, page, status);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(status))
                    {
                        if (!string.IsNullOrWhiteSpace(flag))
                        {
                            return await _threadRepository.GetThreadsByAgentAndFlag(business_id, page, agentId, flag);
                        }
                        else
                        {
                            return await _threadRepository.GetThreadsByAgent(business_id, page, agentId);
                        }
                    }
                    else
                    {
                        return await _threadRepository.GetThreadsByAgentAndStatus(business_id, page, agentId, status);
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(agentId))
                {
                    if (string.IsNullOrWhiteSpace(status))
                    {
                        if (!string.IsNullOrWhiteSpace(flag))
                        {
                            return await _threadRepository.GetThreadsByChannelAndFlag(business_id, page, channelId, flag);
                        }
                        else
                        {
                            return await _threadRepository.GetThreadsByChannel(business_id, page, channelId);
                        }
                    }
                    else
                    {
                        return await _threadRepository.GetThreadsByChannelAndStatus(business_id, page, channelId, status);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(status))
                    {
                        if (!string.IsNullOrWhiteSpace(flag))
                        {
                            return await _threadRepository.GetThreadsByChannelAndAgentAndFlag(business_id, page, channelId, agentId, flag);
                        }
                        else
                        {
                            return await _threadRepository.GetThreadsByChannelAndAgent(business_id, page, channelId, agentId);
                        }
                    }
                    else
                    {
                        return await _threadRepository.GetThreadsByChannelAndAgentAndStatus(business_id, page, channelId, agentId, status);
                    }
                }

            }
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


        public Thread CreateThread(string business_id, string thread_id, Message message)
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
            return CreateThread(business_id, thread, message);
        }

        public Thread CreateThread(string business_id, Thread thread, Message message)
        {
            thread.channel_ext_id = message.channel_ext_id;
            thread.channel_id = message.channel_id;
            thread.business_id = business_id;
            thread.channel_type = message.channel_type;
            thread.updated_time = message.created_time;
            if (thread.timestamp < message.timestamp)
            {
                thread.last_message_ext_id = message.id;
                thread.last_message = message.message;
                thread.owner_timestamp = message.sender_id == thread.owner_id || thread.owner_timestamp <= 0 ? message.timestamp : thread.owner_timestamp;
                thread.sender_id = message.sender_id;
                thread.sender_ext_id = message.sender_ext_id;
                thread.sender_name = message.sender_name;
                thread.sender_avatar = message.sender_avatar;
                thread.timestamp = message.timestamp;
            }
            thread.status = string.IsNullOrWhiteSpace(thread.agent_id) ? "pending" : "active";
            thread.unread = thread.owner_id == thread.sender_id;

            CreateThread(thread);

            return thread;
        }

        public Task<IEnumerable<Thread>> SearchThreadsByCustomerName(string business_id, string channelId, string agentId, string status, string flag, string keywords, Paging page)
        {
            throw new NotImplementedException();
        }

        //public async Task<IEnumerable<Thread>> SearchThreadsByCustomerName(string business_id, string channelId, string agentId, string status, string flag, string keywords, Paging page)
        //{
        //    keywords = keywords ?? "";
        //    keywords = keywords.Trim().Replace("  ", " ").ToLower();
        //    Dictionary<string, ThreadSearchResultItem> list = new Dictionary<string, ThreadSearchResultItem>();
        //    List<string> keys = new List<string>();
        //    int order = 0;
        //    foreach (var word in keywords.Split(' '))
        //    {
        //        order++;
        //        int count = 0;
        //        for (int i = 0; i < word.Length; i++)
        //        {
        //            string key = word.Substring(0, word.Length - i);
        //            var rs = await _threadRepository.SearchThreadsByCustomerName(business_id, "w" + order + "l" + key.Length, key, page);
        //            foreach (var item in rs)
        //            {
        //                if (list.ContainsKey(item.id))
        //                {
        //                    list[item.id].Weight += key.Length;
        //                }
        //                else
        //                {
        //                    count++;
        //                    list.Add(item.id, new ThreadSearchResultItem { Weight = key.Length, Thread = item });
        //                }
        //            }
        //            if (count > page.Limit) break;

        //        }
        //    }
        //    return list.OrderByDescending(t => t.Value.Weight).Select(t => t.Value.Thread).Take(page.Limit);
        //}

    }
}
