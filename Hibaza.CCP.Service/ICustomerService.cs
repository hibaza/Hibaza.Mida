using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> SearchCustomers(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page);
        Task<IEnumerable<Customer>> GetActiveUnreadCustomers(string business_id, Paging page);
        Task<IEnumerable<Customer>> GetActiveNonReplyCustomers(string business_id, Paging page);
        
        Task<IEnumerable<Customer>> GetPendingUnreadCustomers(string business_id, Paging page);
        Task<IEnumerable<Customer>> GetPendingNonReplyCustomers(string business_id, Paging page);
        
        Task<IEnumerable<Customer>> GetUnreadCustomers(string business_id, Paging page);
        Task<IEnumerable<Customer>> GetUnreadCustomersByChannel(string business_id, string channel_id, Paging page);
        Task<IEnumerable<Customer>> GetUnreadCustomersByAgent(string business_id, string agent_id, Paging page);
        Task<IEnumerable<Customer>> GetUnreadCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page);

        Task<IEnumerable<Customer>> GetNonReplyCustomers(string business_id, Paging page);
        Task<IEnumerable<Customer>> GetNonReplyCustomersByChannel(string business_id, string channel_id, Paging page);
        Task<IEnumerable<Customer>> GetNonReplyCustomersByAgent(string business_id, string agent_id, Paging page);
        Task<IEnumerable<Customer>> GetNonReplyCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page);

        Task<IEnumerable<Customer>> GetAllCustomers(string business_id, Paging page);
        Task<IEnumerable<Customer>> GetAllCustomersByAgent(string business_id, string agent_id, Paging page);
        Task<IEnumerable<Customer>> GetAllCustomersByChannel(string business_id, string channel_id, Paging page);
        Task<IEnumerable<Customer>> GetAllCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page);

        Task<IEnumerable<Customer>> GetOpenCustomers(string business_id, Paging page);
        Task<IEnumerable<Customer>> GetOpenCustomersByChannel(string business_id, string channel_id, Paging page);
        Task<IEnumerable<Customer>> GetOpenCustomersByAgent(string business_id, string agent_id, Paging page);
        Task<IEnumerable<Customer>> GetOpenCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page);

        Task<IEnumerable<Customer>> GetCustomersWhereExtIdIsNull(string business_id, Paging page);

        Task<IEnumerable<Domain.Entities.Customer>> All(string business_id);
        Customer GetById(string business_id, string id);
        Task<Domain.Models.CustomerContactInfoModel> GetCustomerId(string business_id, string id);
        void UpdatePhoneNumber(string business_id, string customer_id, IEnumerable<string> phoneList, string lastPhoneNumber);
        void UpdateContactInfo(string business_id, string customer_id, CustomerContactInfoModel data);
        Task<Customer> CreateCustomer(Customer customer, Thread thread, bool real_time_update);
        bool CreateCustomer(Domain.Entities.Customer customer, bool real_time_update);
        Customer RefreshCustomer(string business_id, string customer_id);
        Task<int> AutoAssignToAvailableAgents(string business_id, Paging page);
        bool AssignToAgent(string business_id, string customer_id, string agent_id, string assigned_by);
        Customer UnAssignFromAgent(string business_id, string customer_id);
        bool Block(string business_id, string customer_id, bool blocked);
        Task<Customer> UpdateCustomerLastTicket(string business_id, string customer_id, bool real_time_update);
        Task<int> UpdateCustomerStatus(string business_id, string status, int limit);
        Task<Customer> UpdateCustomerStatusAccordingToThread(string business_id, string customer_id, string thread_id, bool real_time_update);

        Task<IEnumerable<Counter>> GetAgentCounters(string business_id);

        void BatchUpdateUnreadCounters(string business_id);
        int UnAssignFromInActiveAgents(string business_id, Dictionary<string, Channel> channels, Dictionary<string, Agent> agents, Paging page);
        Dictionary<string, Counter> UnAssignFromOverLoadAgents(string business_id, Dictionary<string, Channel> channels, Dictionary<string, Agent> agents, Dictionary<string, Counter> counters, int limit, Paging page);
        
        bool CopyCustomerToRealTimeDB(string business_id, string id);
        Task<IEnumerable<Customer>> GetCustomerActiveThread(string business_id);
        Task<List<Customer>> GetCustomersActiveThreadLikeThread(string business_id, string thread_id, Paging page);
        Task<List<Customer>> GetCustomersAppId(string business_id,string appId);
        Task<List<Customer>> GetCustomerFromPhone(string business_id, string phone);

        Task<List<Customer>> GetCustomerFromPhone(string phone);
        Task<bool> UpdateRealName(string business_id, string customer_id, string real_name);
    }
}
