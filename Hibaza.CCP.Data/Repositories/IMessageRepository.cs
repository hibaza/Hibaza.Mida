using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IMessageRepository
    {
        Message GetById(string business_id, string id);
        Task<IEnumerable<Message>> GetAll(string business_id, Paging page);
        void Upsert(Message message);
        bool Insert(Message message);
        bool Delete(string business_id, string id);
        bool Update(Message message);

        Task<IEnumerable<Message>> GetMessagesByCustomerExcludeCurrentThread(string business_id, string customer_id, string thread_id, Paging page);
        Task<List<Message>> GetMessagesByCustomer(string business_id, string customer_id, Paging page);
        Task<List<Message>> GetMessagesByThread(string business_id, Paging page, string threadId);
        Task<IEnumerable<Message>> GetCustomerOrAgentMessagesNonDeletedByThread(string business_id, Paging page, string thread_id);
        Task<IEnumerable<Message>> GetNonDeletedMessagesByThread(string business_id, Paging page, string thread_id);
        Task<IEnumerable<Message>> GetStarredMessagesByCustomer(string business_id, Paging page, string thread_id);

        void AddGroupedByUser(string business_id, Message entity, string userId);
        void AddGroupedByThread(string business_id, Message entity, string threadId);
        bool MoveAllUserMessagesTo(string business_id, string from_id, string to_id);

        void DeleteAllForThread(string business_id, string thread_id);
        bool Delete(string business_id, string thread_id, string id);
        bool MarkAsReplied(string business_id, string id, long replied_at);
        bool MarkAsDeleted(string business_id, string id);
        int UpdateCustomerId();
        Task<IEnumerable<Message>> GetMessagesWhereCustomerIsNull(string business_id,int limit);
        Task<List<Message>> GetStartMessagesByThread(string business_id, Paging page, string threadId);

        Task<bool> deleteMessageidIfNull(string business_id, string message_id);
    }
}
