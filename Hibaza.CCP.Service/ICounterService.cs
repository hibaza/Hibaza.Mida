using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface ICounterService
    {
        void DeleteAllUnreadCountersFromAgent(string business_id, string agent_id);
        void DeleteSingleThreadFromAgentAll(string business_id, string agent_id, string thread_id);
        void AddSingleUnreadThreadToAgent(string business_id, string agent_id, string thread_id);
        void DeleteSingleUnreadThreadFromAgent(string business_id, string agent_id, string thread_id);
        bool UpdateThreadCounters(string business_id, string channel_id, string thread_id, string agent_id , bool unread);
        bool UpdateThreadCounters(string business_id, Thread thread);

        bool AddUnreadThreadCountersData(string business_id, Thread thread);
        bool RefreshChannelUnreadThreadsCount(string business_id, string channel_id);
        bool RefreshAgentUnreadThreadsCount(string business_id, string agent_id);

        void SetAgentUnreadThreadsCount(string business_id, string agent_id, int count);
        void SetChannelUnreadThreadsCount(string business_id, string channel_id, int count);


        bool DeleteAll(string business_id);
        bool DeleteAllUnreadCountersData(string business_id);
    }
}
