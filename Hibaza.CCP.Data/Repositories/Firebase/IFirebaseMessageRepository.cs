using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public interface IFirebaseMessageRepository
    {

        MessageModel GetById(string business_id, string id);
        IEnumerable<T> GetAll<T>(string business_id, Paging page);
        void Upsert(string business_id, MessageModel message);
        bool Delete(string business_id, string id);
        bool DeleteFolder();
        Task<dynamic> GetMessagesByUser(string business_id, Paging page, string key);
        Task<dynamic> GetMessagesByThread(string business_id, Paging page, string threadId);
        void AddGroupedByUser(string business_id, MessageModel entity, string userId);
        void AddGroupedByThread(string business_id, MessageModel entity, string threadId);
        bool MoveAllUserMessagesTo(string business_id, string from_id, string to_id);
        string CreateMessageAgentMap(string business_id, string message_id, string agent_id);
        string GetMessageAgentMap(string business_id, string message_id);

        void DeleteAllForThread(string business_id, string thread_id);
        bool Delete(string business_id, string thread_id, string id);
    }
}
