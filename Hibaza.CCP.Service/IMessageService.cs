using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface IMessageService
    {
        bool CreateMessage(string business_id, Domain.Entities.Message message, bool real_time_update);
        bool SendMessageToThreadWithCountersUpdate(string business_id, Domain.Entities.Message message, Thread thread);
        bool SendMessageToThread(string business_id, Message message, string thread_id);
        Message GetById(string business_id, string id);
        Task<IEnumerable<Message>> All(string business_id, Paging page);
        Task<IEnumerable<Message>> GetByThread(string business_id, Paging page, string id);
        Task<IEnumerable<Message>> GetCustomerOrAgentNonDeletedMessagesByThread(string business_id, Paging page, string id);
        Task<IEnumerable<Message>> GetNonDeletedByThread(string business_id, Paging page, string id);
        Task<IEnumerable<Message>> GetStarredMesagesByCustomer(string business_id, Paging page, string id);
        void DeleteByThread(string business_id, string thread_id);
        void Delete(string business_id, string id);
        void Delete(string business_id, string thread_id, string id);

        bool CopyMessageToRealtimeDB(string business_id, string id, long timestamp);
        int CopyFromRealtimeDB(string business_id, string thread_id, Paging page);
        Task<List<Message>> GetByCustomer(string business_id, string customer_id, Paging paging);
        Task<IEnumerable<Message>> GetByCustomerExcludeCurrentThread(string business_id, string customer_id, string channel_ext_id, Paging page);
        bool MarkAsReplied(string business_id, string id, long replied_at);
        bool MarkAsDeleted(string business_id, string id);
        int UpdateCustomerId();
        string GetFirebaseStorageAttachmentUrl(string business_id, string folder, string attachment_id);
        Task<string> UploadAttachmentToFirebaseStorage(string business_id, string folder, string attachment_id, Stream stream);

        Attachment GetAttachmentById(string business_id, string channel_id, string id);
        bool SaveAttachment(Attachment attachment);

        Task<IEnumerable<Message>> GetMessagesWhereCustomerIsNull(string business_id,int limit);
        Task<List<Message>> GetMessagesByThread(string business_id, Paging page, string threadId);
        Task<List<Message>> GetStartMessagesByThread(string business_id, Paging page, string threadId);

        Task<bool> deleteMessageidIfNull(string business_id, string message_id);
    }
}
