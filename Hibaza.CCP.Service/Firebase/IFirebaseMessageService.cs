using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service.Firebase
{
    public interface IFirebaseMessageService
    {
        string CreateMessageAgentMap(string business_id, string message_id, string agent_id);
        string GetMessageAgentMap(string business_id, string message_id);
        bool CreateMessage(string business_id, MessageModel message);
        bool AssignMessageToAgent(string business_id, MessageModel message, string agent_id);
        bool SendMessageToCustomer(string business_id, MessageModel message, Domain.Models.Facebook.FacebookUserFeed to);
        bool SendMessageToThreadWithCountersUpdate(string business_id, MessageModel message, Thread thread);
        bool SendMessageToThread(string business_id, MessageModel message, string thread_id);
        MessageModel GetById(string business_id, string id);
        IEnumerable<T> All<T>(string business_id, Paging page);
        IEnumerable<MessageModel> GetByUser(string business_id, Paging page, string id);
        Task<dynamic> GetByThread(string business_id, Paging page, string id);
        void DeleteByThread(string business_id, string thread_id);
        void Delete(string business_id, string id);
        void Delete(string business_id, string thread_id, string id);
    }
}
