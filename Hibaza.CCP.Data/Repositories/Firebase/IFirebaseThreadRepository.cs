using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public interface IFirebaseThreadRepository : IGenericRepository<Domain.Entities.Thread>
    {
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

        void CreateReferral(string business_id, string thread_id, string referral_id, string referral_name);

        void MapThreadWithConversation(string business_id, string threadId, string conversationId, bool done);
        string GetThreadByConversation(string business_id, string conversationId);

        Task<IEnumerable<Thread>> SearchThreadsByCustomerName(string business_id, string child, string key, Paging page);

        Task<IEnumerable<Thread>> SearchThreadsByCustomer(string business_id, string keywords, Paging page);
    }
}
