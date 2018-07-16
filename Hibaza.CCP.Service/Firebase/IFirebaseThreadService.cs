using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service.Firebase
{
    public interface IFirebaseThreadService
    {
        bool CreateThread(Domain.Entities.Thread thread);
        Thread CreateThread(string business_id, string thread_id, Domain.Entities.Message message);
        Thread CreateThread(string business_id, Thread thread, Message message);
        Task<IEnumerable<Domain.Entities.Thread>> All(string business_id);
        Thread GetById(string business_id, string id);
        Task<IEnumerable<Domain.Entities.Thread>> GetThreads(string business_id, Paging page, string channelId, string agentId, string status, string flag);
        bool AssignToAgent(string business_id, string thread_id, string agent_id);
        string MarkAsUnRead(string business_id, string thread_id);
        string MarkAsRead(string business_id, string thread_id);
        void UnAssignAllUnreadThreadsFromAgent(string business_id, string agent_id);
        Thread UnAssignFromAgent(string business_id, string thread_id);
        void BatchUpdateUnreadCounters(string business_id);
        int Delete(string business_id, string id);

        bool CreateReferral(string business_id, FacebookMessagingEvent referralEvent);

        void MapThreadWithConversation(string business_id, string thread_id, string conversation_id, bool done);
        string GetThreadByConversation(string business_id, string conversation_id);

        Task<IEnumerable<Domain.Entities.Thread>> SearchThreadsByCustomerName(string business_id, string channelId, string agentId, string status, string flag, string keywords, Paging page);
    }
}
