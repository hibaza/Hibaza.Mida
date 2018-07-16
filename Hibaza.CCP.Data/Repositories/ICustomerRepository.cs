using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface ICustomerRepository
    {

        Customer GetById(string business_id, string id);
        Task<CustomerContactInfoModel> GetCustomerId(string business_id, string id);
        IEnumerable<Customer> GetAll(string business_id);
        bool Insert(Customer customer);
        bool Update(Customer customer);
        bool Delete(string business_id, string id);
        void Upsert(Customer customer);
        bool UpdateUserId(string business_id, int key, string user_id);
        Task<bool> UpdatePhoneNumber(string business_id, string customer_id, string phone_list, string phone);
        Task<IEnumerable<Customer>> SearchCustomersByKeywords(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page);
        Task<IEnumerable<Counter>> GetAgentCounters(string business_id);
        Task<IEnumerable<Counter>> GetChannelCounters(string business_id);
        Task<IEnumerable<Customer>> GetActiveUnreadCustomers(string business_id, string agent_id, Paging page);
        Task<IEnumerable<Customer>> GetActiveUnreadCustomers(string business_id, Paging page);
        Task<IEnumerable<Customer>> GetActiveNonReplyCustomers(string business_id, Paging page);
        
        Task<IEnumerable<Customer>> GetPendingUnreadCustomers(string business_id, Paging page);
        Task<IEnumerable<Customer>> GetPendingNonReplyCustomers(string business_id, Paging page);
        Task<IEnumerable<Customer>> GetPendingUnreadCustomers(string business_id, string channel_id, Paging page);
       
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

        bool UpdateContactInfo(string business_id, string customer_id, CustomerContactInfoModel data);
        bool Block(string business_id, string customer_id, bool blocked);
        Task<IEnumerable<Customer>> GetCustomerActiveThread(string business_id);
        Task<List<Customer>> GetCustomersActiveThreadLikeThread(string business_id, string thread_id, Paging page);
        Task<List<Customer>> GetCustomersAppId(string business_id,string appId);
        Task<List<Customer>> GetCustomerFromPhone(string business_id, string phone);
        Task<List<Customer>> GetCustomerFromPhone(string phone);
        Task<bool> UpdateRealName(string business_id, string customer_id, string real_name);
    }
}
