using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface ICounterRepository : IGenericRepository<Counter>
    {
        void AddThreadToChannels(string business_id, string channel_id, Counter entity);
        void AddThreadToChannelsUnread(string business_id, string channel_id, Counter entity);
        void AddThreadToChannelsUnassignedUnread(string business_id, string channel_id, Counter entity);
        void AddThreadToAgents(string business_id, string agent_id, Counter entity);
        void AddTheadToAgentsUnread(string business_id, string agent_id, Counter entity);

        void UpdateChannelsUnreadThreadsCount(string business_id, string channel_id);
        void UpdateAgentsUnreadThreadsCount(string business_id, string agent_id);


        void DeleteThreadFromChannels(string business_id, string channel_id, string id);
        void DeleteThreadFromChannelsUnread(string business_id, string channel_id, string id);
        void DeleteThreadFromChannelsUnassignedUnread(string business_id, string channel_id, string id);
        void DeleteThreadFromAgentsAll(string business_id, string agent_id, string id);
        void DeleteThreadFromAgentsUnread(string business_id, string agent_id, string id);

        bool DeleteAll(string business_id);
        bool DeleteAllUnreadCountersData(string business_id);

        Task<Dictionary<string, int>> GetAgentsUnreadCount(string business_id);
        IEnumerable<Counter> GetThreadUnreadCountByAgent(string business_id, string agent_id);
        System.Threading.Tasks.Task DeleteAllThreadCountersFromAgents(string business_id, string agent_id);
        System.Threading.Tasks.Task DeleteAllUnreadThreadCountersFromAgent(string business_id, string agent_id);

        void SetChannelsUnreadThreadsCount(string business_id, string channel_id, int count);
        void SetAgentsUnreadThreadsCount(string business_id, string agent_id, int count);
    }
}
