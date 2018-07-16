using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface ICustomerCounterRepository : IGenericRepository<Counter>
    {
        void AddCustomerToChannels(string business_id, string channel_id, Counter entity);
        void AddCustomerToChannelsUnread(string business_id, string channel_id, Counter entity);
        void AddCustomerToChannelsUnassignedUnread(string business_id, string channel_id, Counter entity);
        void AddCustomerToAgents(string business_id, string agent_id, Counter entity);
        void AddCustomerToAgentsUnread(string business_id, string agent_id, Counter entity);

        void UpdateChannelsUnreadCustomersCount(string business_id, string channel_id);
        void UpdateAgentsUnreadCustomersCount(string business_id, string agent_id);


        void DeleteCustomerFromChannels(string business_id, string channel_id, string id);
        void DeleteCustomerFromChannelsUnread(string business_id, string channel_id, string id);
        void DeleteCustomerFromChannelsUnassignedUnread(string business_id, string channel_id, string id);
        void DeleteCustomerFromAgentsAll(string business_id, string agent_id, string id);
        void DeleteCustomerFromAgentsUnread(string business_id, string agent_id, string id);

        bool DeleteAll(string business_id);
        bool DeleteAllUnreadCountersData(string business_id);

        Task<Dictionary<string, int>> GetAgentsUnreadCount(string business_id);
        IEnumerable<Counter> GetCustomerUnreadCountByAgent(string business_id, string agent_id);
        System.Threading.Tasks.Task DeleteAllCustomerCountersFromAgents(string business_id, string agent_id);
        System.Threading.Tasks.Task DeleteAllUnreadCustomerCountersFromAgent(string business_id, string agent_id);

        void SetChannelsUnreadCustomersCount(string business_id, string channel_id, int count);
        void SetAgentsUnreadCustomersCount(string business_id, string agent_id, int count);
    }
}
