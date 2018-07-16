using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IThreadRepository
    {
        Thread GetById(string business_id, string id);        
        IEnumerable<Thread> GetAll(string business_id);
        long Upsert(string business_id, Thread thread);
        bool Delete(string business_id, string id);
        bool Insert(Thread thread);
        bool Update(Thread thread);
        bool UpdateCustomerId(string business_id, string id, string customer_id);
        int UpdateCustomerId();

        Task<IEnumerable<Thread>> GetThreads(string businessId, Paging page);
        Task<IEnumerable<Thread>> GetThreadsByChannel(string businessId, Paging page, string channelId);
        Task<IEnumerable<Thread>> GetThreadsByChannelAndAgent(string businessId, Paging page, string channelId, string agentId);
        Task<IEnumerable<Thread>> GetThreadsByChannelAndStatus(string businessId, Paging page, string channelId, string status);
        Task<IEnumerable<Thread>> GetThreadsByChannelAndAgentAndStatus(string businessId, Paging page, string channelId, string agentId, string status);
        Task<IEnumerable<Thread>> GetThreadsByAgentAndStatus(string businessId, Paging page, string agentId, string status);
        Task<IEnumerable<Thread>> GetThreadsByAgent(string businessId, Paging page, string agentId);
        Task<IEnumerable<Thread>> GetThreadsByStatus(string businessId, Paging page, string status);

        Task<IEnumerable<Thread>> GetThreadsByFlag(string businessId, Paging page, string flag);
        Task<IEnumerable<Thread>> GetThreadsByAgentAndFlag(string businessId, Paging page, string agentId, string flag);
        Task<IEnumerable<Thread>> GetThreadsByChannelAndFlag(string businessId, Paging page, string channelId, string flag);
        Task<IEnumerable<Thread>> GetThreadsByChannelAndAgentAndFlag(string businessId, Paging page, string channelId, string agentId, string flag);

        IEnumerable<Thread> GetUnreadThreads(string businessId, Paging page);

        string GetPageUIDByAppUID(string auid);
        string GetBusinessUIDByPageUID(string puid);
        void UpdatePageBusinessMapping(string buid, string puid, string @ref);
        void UpdateAppPageMapping(string puid, string auid, string @ref);
        string GetAppRefParam(string auid);

        bool UpdateLastVisits(string business_id, string id, string last_visits);

        Task<IEnumerable<Thread>> SearchThreadsByKeywords(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page);
        Task<IEnumerable<Thread>> SearchThreadsDistinctCustomerByKeywords(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page);
        Task<IEnumerable<Counter>> GetChannelCounters(string business_id);
        Task<IEnumerable<Counter>> GetAgentCounters(string business_id);
        Task<IEnumerable<Thread>> GetActiveUnreadThreads(string business_id, string agent_id, Paging page);

        Task<IEnumerable<Thread>> GetThreadsByCustomer(string business_id, string customer_id, Paging page);
        Task<IEnumerable<Thread>> GetThreadsWhereCustomerIsNull(string business_id, Paging page);

        Task<IEnumerable<Thread>> GetUnreadOrNonReplyThreadsByCustomer(string business_id, string customer_id, Paging page);

        Task<IEnumerable<Thread>> GetNoReponseThreads(string business_id, string channel_id, Paging page);
        Thread GetByIdFromCustomerId(string business_id, string customerId);
        Task<int> Job_AutoCallProcedure(string procedure);
    }
}
