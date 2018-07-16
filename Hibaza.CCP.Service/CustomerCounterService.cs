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
    public class CustomerCounterService : ICustomerCounterService
    {
        private readonly ICustomerCounterRepository _counterRepository;
        public CustomerCounterService(ICustomerCounterRepository counterRepository)
        {
            _counterRepository = counterRepository;
        }

        public void AddSingleCustomerToAgent(string business_id, string agent_id, Counter counter)
        {
            _counterRepository.AddCustomerToAgents(business_id, agent_id, counter);
        }

        public void AddSingleUnreadCustomerToAgent(string business_id, string agent_id, Counter counter)
        {
            _counterRepository.AddCustomerToAgentsUnread(business_id, agent_id, counter);
        }

        public void DeleteAllUnreadCountersFromAgent(string business_id, string agent_id)
        {
            throw new NotImplementedException();
        }

        public void DeleteSingleCustomerFromAgentAll(string business_id, string agent_id, string customer_id)
        {
            try
            {
                _counterRepository.DeleteCustomerFromAgentsAll(business_id, agent_id, customer_id);
            }
            catch { }
        }

        public void DeleteSingleUnreadCustomerFromAgent(string business_id, string agent_id, string customer_id)
        {
            try
            {
                _counterRepository.DeleteCustomerFromAgentsUnread(business_id, agent_id, customer_id);
            }
            catch { }
        }

        public bool DeleteAll(string business_id)
        {
            return _counterRepository.DeleteAll(business_id);
        }

        public bool UpdateCustomerCounters(string business_id, Customer customer, bool remove)
        {
            var counter = new Counter { id = customer.id, count = (customer.unread ? 1 : 0) * (remove ? -1 : 1) };
            try
            {
                _counterRepository.AddCustomerToChannelsUnread(business_id, customer.channel_id, counter);
            }
            catch { }
            if (!string.IsNullOrWhiteSpace(customer.agent_id))
            {
                try
                {
                    _counterRepository.AddCustomerToAgentsUnread(business_id, customer.agent_id, counter);
                }
                catch
                {

                }
            }

            return true;
        }

        private bool UpdateCustomerCounters(string business_id, string channel_id, string customer_id, string agent_id, bool unread)
        {
            var counter = new Counter { id = customer_id, count = 1 };
            _counterRepository.AddCustomerToChannels(business_id, channel_id, counter);
            if (!string.IsNullOrWhiteSpace(agent_id))
            {
                _counterRepository.AddCustomerToAgents(business_id, agent_id, counter);
            }
            if (unread)
            {
                _counterRepository.AddCustomerToChannelsUnread(business_id, channel_id, counter);
                if (string.IsNullOrWhiteSpace(agent_id))
                {
                    _counterRepository.AddCustomerToChannelsUnassignedUnread(business_id, channel_id, counter);
                }
                else
                {
                    _counterRepository.AddCustomerToAgentsUnread(business_id, agent_id, counter);
                }
            }
            else
            {
                try
                {
                    _counterRepository.DeleteCustomerFromChannelsUnread(business_id, channel_id, counter.id);
                }
                catch { }
                if (string.IsNullOrWhiteSpace(agent_id))
                {
                    try
                    {
                        _counterRepository.DeleteCustomerFromChannelsUnassignedUnread(business_id, channel_id, counter.id);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        _counterRepository.DeleteCustomerFromAgentsUnread(business_id, agent_id, counter.id);
                    }
                    catch { }
                }
            }

            return true;
        }

        public bool AddUnreadCustomerCountersData(string business_id, Customer customer)
        {
            var counter = new Counter { id = customer.id, count = 1 };
            _counterRepository.AddCustomerToChannelsUnread(business_id, customer.channel_id, counter);
            if (string.IsNullOrWhiteSpace(customer.agent_id))
            {
                _counterRepository.AddCustomerToChannelsUnassignedUnread(business_id, customer.channel_id, counter);
            }
            else
            {
                _counterRepository.AddCustomerToAgentsUnread(business_id, customer.agent_id, counter);
            }
            return true;
        }

        public bool RefreshChannelUnreadCustomersCount(string business_id, string channel_id)
        {
            _counterRepository.UpdateChannelsUnreadCustomersCount(business_id , channel_id);
            return true;
        }

        public bool RefreshAgentUnreadCustomersCount(string business_id, string agent_id)
        {
            _counterRepository.UpdateAgentsUnreadCustomersCount(business_id, agent_id);
            return true;
        }

        public void SetChannelUnreadCustomersCount(string business_id, string channel_id, int count)
        {
            _counterRepository.SetChannelsUnreadCustomersCount(business_id, channel_id, count);
        }

        public void SetAgentUnreadCustomersCount(string business_id, string agent_id, int count)
        {
            _counterRepository.SetAgentsUnreadCustomersCount(business_id, agent_id, count);
        }

        public bool DeleteAllUnreadCountersData(string business_id)
        {
            return _counterRepository.DeleteAllUnreadCountersData(business_id);
        }
    }
}
