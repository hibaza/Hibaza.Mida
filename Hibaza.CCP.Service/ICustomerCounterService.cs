using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface ICustomerCounterService
    {
        void DeleteAllUnreadCountersFromAgent(string business_id, string agent_id);
        void DeleteSingleCustomerFromAgentAll(string business_id, string agent_id, string customer_id);
        void DeleteSingleUnreadCustomerFromAgent(string business_id, string agent_id, string customer_id);
        //bool UpdateCustomerCounters(string business_id, string channel_id, string customer_id, string agent_id , bool unread);
        bool UpdateCustomerCounters(string business_id, Customer customer, bool remove);

        void AddSingleUnreadCustomerToAgent(string business_id, string agent_id, Counter counter);
        void AddSingleCustomerToAgent(string business_id, string agent_id, Counter counter);

        bool AddUnreadCustomerCountersData(string business_id, Customer customer);
        bool RefreshChannelUnreadCustomersCount(string business_id, string channel_id);
        bool RefreshAgentUnreadCustomersCount(string business_id, string agent_id);

        void SetAgentUnreadCustomersCount(string business_id, string agent_id, int count);
        void SetChannelUnreadCustomersCount(string business_id, string channel_id, int count);


        bool DeleteAll(string business_id);
        bool DeleteAllUnreadCountersData(string business_id);
    }
}
