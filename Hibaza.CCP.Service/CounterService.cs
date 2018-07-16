using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public class CounterService : ICounterService
    {
        private readonly ICounterRepository _counterRepository;
        private readonly IThreadRepository _threadRepository;
        public CounterService(ICounterRepository counterRepository, IThreadRepository threadRepository)
        {
            _threadRepository = threadRepository;
            _counterRepository = counterRepository;
        }

        public void AddSingleUnreadThreadToAgent(string business_id, string agent_id, string thread_id)
        {
            _counterRepository.AddTheadToAgentsUnread(business_id, agent_id, new Counter { id = thread_id, count = 1 });
        }

        public void DeleteAllUnreadCountersFromAgent(string business_id, string agent_id)
        {
            throw new NotImplementedException();
        }

        public void DeleteSingleThreadFromAgentAll(string business_id, string agent_id, string thread_id)
        {
            try
            {
                _counterRepository.DeleteThreadFromAgentsAll(business_id, agent_id, thread_id);
            }
            catch { }
        }

        public void DeleteSingleUnreadThreadFromAgent(string business_id, string agent_id, string thread_id)
        {
            try
            {
                _counterRepository.DeleteThreadFromAgentsUnread(business_id, agent_id, thread_id);
            }
            catch { }
        }

        public bool DeleteAll(string business_id)
        {
            return _counterRepository.DeleteAll(business_id);
        }

        public bool UpdateThreadCounters(string business_id, Thread thread)
        {
            var counter = new Counter { id = thread.id, count = 1 };
            _counterRepository.AddThreadToChannels(business_id, thread.channel_id, counter);
            if (!string.IsNullOrWhiteSpace(thread.agent_id))
            {
                _counterRepository.AddThreadToAgents(business_id, thread.agent_id, counter);
            }
            if (thread.unread)
            {
                _counterRepository.AddThreadToChannelsUnread(business_id, thread.channel_id, counter);
                if (string.IsNullOrWhiteSpace(thread.agent_id))
                {
                    _counterRepository.AddThreadToChannelsUnassignedUnread(business_id, thread.channel_id, counter);
                }
                else
                {
                    _counterRepository.AddTheadToAgentsUnread(business_id, thread.agent_id, counter);
                }
            }
            else
            {
                try
                {
                    _counterRepository.DeleteThreadFromChannelsUnread(business_id, thread.channel_id, counter.id);
                }
                catch { }
                if (string.IsNullOrWhiteSpace(thread.agent_id))
                {
                    try
                    {
                        _counterRepository.DeleteThreadFromChannelsUnassignedUnread(business_id, thread.channel_id, counter.id);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        _counterRepository.DeleteThreadFromAgentsUnread(business_id, thread.agent_id, counter.id);
                    }
                    catch { }
                }
            }

            return true;
        }

        public bool UpdateThreadCounters(string business_id, string channel_id, string thread_id, string agent_id, bool unread)
        {
            var counter = new Counter { id = thread_id, count = 1 };
            _counterRepository.AddThreadToChannels(business_id, channel_id, counter);
            if (!string.IsNullOrWhiteSpace(agent_id))
            {
                _counterRepository.AddThreadToAgents(business_id, agent_id, counter);
            }
            if (unread)
            {
                _counterRepository.AddThreadToChannelsUnread(business_id, channel_id, counter);
                if (string.IsNullOrWhiteSpace(agent_id))
                {
                    _counterRepository.AddThreadToChannelsUnassignedUnread(business_id, channel_id, counter);
                }
                else
                {
                    _counterRepository.AddTheadToAgentsUnread(business_id, agent_id, counter);
                }
            }
            else
            {
                try
                {
                    _counterRepository.DeleteThreadFromChannelsUnread(business_id, channel_id, counter.id);
                }
                catch { }
                if (string.IsNullOrWhiteSpace(agent_id))
                {
                    try
                    {
                        _counterRepository.DeleteThreadFromChannelsUnassignedUnread(business_id, channel_id, counter.id);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        _counterRepository.DeleteThreadFromAgentsUnread(business_id, agent_id, counter.id);
                    }
                    catch { }
                }
            }

            return true;
        }

        public bool AddUnreadThreadCountersData(string business_id, Thread thread)
        {
            var counter = new Counter { id = thread.id, count = 1 };
            _counterRepository.AddThreadToChannelsUnread(business_id, thread.channel_id, counter);
            if (string.IsNullOrWhiteSpace(thread.agent_id))
            {
                _counterRepository.AddThreadToChannelsUnassignedUnread(business_id, thread.channel_id, counter);
            }
            else
            {
                _counterRepository.AddTheadToAgentsUnread(business_id, thread.agent_id, counter);
            }
            return true;
        }

        public bool RefreshChannelUnreadThreadsCount(string business_id, string channel_id)
        {
            _counterRepository.UpdateChannelsUnreadThreadsCount(business_id , channel_id);
            return true;
        }

        public bool RefreshAgentUnreadThreadsCount(string business_id, string agent_id)
        {
            _counterRepository.UpdateAgentsUnreadThreadsCount(business_id, agent_id);
            return true;
        }

        public void SetChannelUnreadThreadsCount(string business_id, string channel_id, int count)
        {
            _counterRepository.SetChannelsUnreadThreadsCount(business_id, channel_id, count);
        }

        public void SetAgentUnreadThreadsCount(string business_id, string agent_id, int count)
        {
            _counterRepository.SetAgentsUnreadThreadsCount(business_id, agent_id, count);
        }

        public bool DeleteAllUnreadCountersData(string business_id)
        {
            return _counterRepository.DeleteAllUnreadCountersData(business_id);
        }
    }
}
