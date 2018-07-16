using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface IThreadService
    {
        bool CreateThread(Domain.Entities.Thread thread, bool real_time_update);
        Thread CreateThread(string business_id, string thread_id, Domain.Entities.Message message, bool real_time_update);
        Thread CreateThread(string business_id, Thread thread, Message message, bool real_time_update);
        Task<IEnumerable<Domain.Entities.Thread>> All(string business_id);
        Thread GetById(string business_id, string id);
        bool AssignToAgent(string business_id, string thread_id, string agent_id);
        int AutoAssignToAvailableAgents(string business_id, Paging page);
        int UnAssignFromInActiveAgents(string business_id, Paging page);
        Thread MarkAsUnRead(string business_id, string thread_id, string agent_id);
        Thread MarkAsRead(string business_id, string thread_id, string agent_id);
        void UnAssignAllUnreadThreadsFromAgent(string business_id, string agent_id);
        Thread UnAssignFromAgent(string business_id, string thread_id);
        void BatchUpdateUnreadCounters(string business_id);
        int Delete(string business_id, string id);

        bool CreateReferral(string business_id, FacebookMessagingEvent referralEvent);

        Task<IEnumerable<Domain.Entities.Thread>> GetThreads(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page);
        Task<IEnumerable<Counter>> GetChannelCounters(string business_id);
        Task<IEnumerable<Counter>> GetAgentCounters(string business_id);

        Thread RefreshThread(string business_id, string thread_id, string message_id);
        Thread RefreshThread(string business_id, string thread_id, bool force_update, bool real_time_update);

        Task<IEnumerable<Thread>> GetByCustomer(string business_id, string customer_id, Paging page);

        string GetCustomerId(string business_id, string id);        
        int UpdateCustomerId(string business_id, int limit);
        bool UpdateCustomerId(string business_id, string thread_id);
        int DeleteFromFirebaseByCustomer(string business_id, string customer_id);

        bool AddLastVisit(string business_id, string thread_id, string url, string agent_id);
        Task<IEnumerable<Thread>> GetNoReponseThreads(string business_id, string channel_id, Paging page);

        Task<int> UpdateThreadStatus(string business_id, string status, int limit);
        bool CopyThreadToRealTimeDB(string business_id, string id, long timestamp);

        Task<IEnumerable<Thread>> GetThreadsWhereCustomerIsNull(string business_id, Paging page);
        Thread GetByIdFromCustomerId(string business_id, string customerId);
    }
}
