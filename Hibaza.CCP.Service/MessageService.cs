//using Hibaza.CCP.Data.Infrastructure;
//using Hibaza.CCP.Data.Repositories;
//using Hibaza.CCP.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Hibaza.CCP.Domain.Models;
//using Firebase.Database;
//using Hibaza.CCP.Data.Repositories.Firebase;
//using Newtonsoft.Json;
//using System.IO;
//using Hibaza.CCP.Data.Providers.Firebase;
//using Hangfire;

//namespace Hibaza.CCP.Service
//{
//    public class MessageService : IMessageService
//    {
//        private readonly IMessageRepository _sqlMessageRepository;
//        private readonly ICounterRepository _counterRepository;
//        private readonly IFirebaseMessageRepository _messageRepository;
//        private readonly IFirebaseStorageFactory _storageFactory;
//        private readonly IAttachmentRepository _attachmentRepository;
//        public MessageService(IFirebaseMessageRepository messageRepository, IAttachmentRepository attachmentRepository, IFirebaseStorageFactory storageFactory, ICounterRepository counterRepository, IMessageRepository sqlMessageRepository)
//        {
//            _messageRepository = messageRepository;
//            _counterRepository = counterRepository;
//            _attachmentRepository = attachmentRepository;
//            _sqlMessageRepository = sqlMessageRepository;
//            _storageFactory = storageFactory;
//        }

//        public static string FormatAttachmentId(string parent, string key)
//        {
//            return string.Format("{0}_{1}", parent, Core.Helpers.CommonHelper.FormatKey(key));
//        }

//        public static string FormatId(string parent, string key)
//        {
//            return Core.Helpers.CommonHelper.FormatKey("", key);
//        }

//        public bool MarkAsDeleted(string business_id, string id)
//        {
//            _sqlMessageRepository.MarkAsDeleted(business_id, id);
//            CopyMessageToRealtimeDB(business_id, id, 0);
//            return true;
//        }

//        public bool MarkAsReplied(string business_id, string id, long replied_at)
//        {
//            var message = GetById(business_id, id);
//            message.replied = true;
//            message.replied_at = replied_at;
//            _sqlMessageRepository.MarkAsReplied(business_id, id, replied_at);

//            BackgroundJob.Enqueue<MessageService>(x => x.CopyMessageToRealtimeDB(business_id, message.id, message.timestamp));
//            return true;
//        }

//        public Message GetById(string business_id, string id)
//        {
//            return _sqlMessageRepository.GetById(business_id, id);
//        }

//        public async Task<IEnumerable<Message>> All(string business_id, Paging page)
//        {
//            return await _sqlMessageRepository.GetAll(business_id, page);
//        }

//        public async Task<IEnumerable<Message>> GetByCustomer(string business_id, string customer_id, Paging page)
//        {
//            return  await _sqlMessageRepository.GetMessagesByCustomer(business_id, customer_id, page);
//        }

//        public async Task<IEnumerable<Message>> GetByCustomerExcludeCurrentThread(string business_id, string customer_id, string thread_id, Paging page)
//        {
//            return await _sqlMessageRepository.GetMessagesByCustomerExcludeCurrentThread(business_id, customer_id, thread_id, page);
//        }

//        public void DeleteByThread(string business_id, string thread_id)
//        {
//            _messageRepository.DeleteAllForThread(business_id, thread_id);
//            _sqlMessageRepository.DeleteAllForThread(business_id, thread_id);
//        }

//        public async Task<IEnumerable<Message>> GetStarredMesagesByCustomer(string business_id, Paging page, string id)
//        {
//            return await _sqlMessageRepository.GetStarredMessagesByCustomer(business_id, page, id);
//        }

//        public async Task<IEnumerable<Message>> GetNonDeletedByThread(string business_id, Paging page, string id)
//        {
//            var msgs = await _sqlMessageRepository.GetNonDeletedMessagesByThread(business_id, page, id);
//            return msgs;
//        }

//        public async Task<IEnumerable<Message>> GetCustomerOrAgentNonDeletedMessagesByThread(string business_id, Paging page, string id)
//        {
//            var msgs = await _sqlMessageRepository.GetCustomerOrAgentMessagesNonDeletedByThread(business_id, page, id);
//            return msgs;
//        }

//        public async Task<IEnumerable<Message>> GetByThread(string business_id, Paging page, string id)
//        {
//            var msgs = await _sqlMessageRepository.GetMessagesByThread(business_id, page, id);
//            return msgs;
//        }

//        public void Delete(string business_id, string id)
//        {
//            _messageRepository.Delete(business_id, id);
//            _sqlMessageRepository.Delete(business_id, id);
//        }

//        public void Delete(string business_id, string thread_id, string id)
//        {
//            _messageRepository.Delete(business_id, thread_id, id);
//            _sqlMessageRepository.Delete(business_id, id);
//        }

//        public int CopyFromRealtimeDB(string business_id, string thread_id, Paging page)
//        {
//            int count = 0;
//            foreach (var tt in _messageRepository.GetMessagesByThread(business_id, page, thread_id).Result)
//            {
//                FbMessage t = tt.Object;
//                count++;
//                Message m = new Message();
//                m.id = t.id;
//                m.created_time = t.created_time;
//                m.updated_time = t.updated_time <= DateTime.MinValue ? null : t.updated_time;
//                m.ext_id = t.ext_id;
//                m.url = t.url;
//                m.file_name = t.file_name;
//                m.size = t.size;
//                m.subject = t.subject;
//                m.message = t.message;
//                m.agent_id = t.agent_id;
//                m.thread_id = t.thread_id;
//                m.conversation_ext_id = t.conversation_ext_id;
//                m.sender_id = t.sender_id;
//                m.sender_ext_id = t.sender_ext_id;
//                m.sender_name = t.sender_name;
//                m.sender_avatar = t.sender_avatar;
//                m.recipient_id = t.recipient_id;
//                m.recipient_ext_id = t.recipient_ext_id;
//                m.recipient_avatar = t.recipient_avatar;
//                m.recipient_name = t.recipient_name;
//                m.author = t.author;
//                m.customer_id = t.customer_id;
//                m.type = t.type;
//                m.timestamp = t.timestamp > 9999999999 ? t.timestamp / 1000 : t.timestamp;
//                m.business_id = t.business_id;
//                m.channel_id = t.channel_id;
//                m.channel_ext_id = t.channel_ext_id;
//                m.channel_type = t.channel_type;
//                //m.urls = JsonConvert.SerializeObject(t.urls);
//                _sqlMessageRepository.Upsert(m);
//            }
//            return count;
//        }

//        public bool CreateMessage(string business_id, Domain.Entities.Message message, bool real_time_update)
//        {
//            _sqlMessageRepository.Upsert(message);
//            if (real_time_update)
//            {
//                try
//                {
//                    CopyMessageToRealtimeDB(business_id, message);
//                }
//                catch
//                {
//                    BackgroundJob.Enqueue<MessageService>(x => x.CopyMessageToRealtimeDB(business_id, message));
//                }
//            }
//            return true;
//        }

//        public bool CopyMessageToRealtimeDB(string business_id, string id, long timestamp)
//        {
//            var msg = GetById(business_id, id);
//            if (msg != null)
//            {
//                CopyMessageToRealtimeDB(business_id, msg);
//            }
//            return true;
//        }

//        private bool CopyMessageToRealtimeDB(string business_id, Message msg)
//        {
//            if (msg != null)
//            {
//                MessageModel message = new MessageModel(msg);
//                //_messageRepository.Upsert(business_id, message);
//                _messageRepository.AddGroupedByThread(business_id, message, message.thread_id);
//            }
//            return true;
//        }

//        public int UpdateCustomerId()
//        {
//            return _sqlMessageRepository.UpdateCustomerId();
//        }

//        //public bool AssignMessageToAgent(string business_id, Message message, string agent_id)
//        //{
//        //    message.agent_id = agent_id;
//        //    _messageRepository.AddGroupedByUser(business_id, message, agent_id);
//        //    if (message.sender_id != message.channel_id && message.sender_id != message.agent_id && !string.IsNullOrWhiteSpace(agent_id) && agent_id != "ALL")
//        //    {
//        //        var counter = new Counter { id = message.sender_id, count = 1};
//        //        _counterRepository.AddThreadToAgents(business_id, agent_id, counter);
//        //        _counterRepository.AddTheadToAgentsUnread(business_id, agent_id, counter);
//        //    }
//        //    return true;
//        //}

//        //public bool SendMessageToCustomer(string business_id, Message message, Domain.Models.Facebook.FacebookUserFeed to)
//        //{
//        //    if (to != null && to.Data != null)
//        //    {
//        //        foreach (var u in to.Data)
//        //        {
//        //            _messageRepository.AddGroupedByUser(business_id, message, u.id);
//        //        }
//        //    }
//        //    return true;
//        //}

//        public bool SendMessageToThread(string business_id, Message message, string thread_id)
//        {
//            message.thread_id = thread_id;
//            _sqlMessageRepository.Upsert(message);
//            BackgroundJob.Enqueue<MessageService>(x=>x.CopyMessageToRealtimeDB(business_id, message.id, message.timestamp));
//            return true;
//        }

//        public bool SendMessageToThreadWithCountersUpdate(string business_id, Message message, Thread thread)
//        {
//            message.thread_id = thread.id;
//            _sqlMessageRepository.Upsert(message);
//            BackgroundJob.Enqueue<MessageService>(x => x.CopyMessageToRealtimeDB(business_id, message.id, message.timestamp));
//            var counter = new Counter { id = thread.id, count = 1 };
//            _counterRepository.AddThreadToChannels(business_id, message.channel_id, counter);
//            if (!string.IsNullOrWhiteSpace(thread.agent_id))
//            {
//                _counterRepository.AddThreadToAgents(business_id, thread.agent_id, counter);
//            }
//            if (thread.unread)
//            {
//                _counterRepository.AddThreadToChannelsUnread(business_id, message.channel_id, counter);
//                if (string.IsNullOrWhiteSpace(thread.agent_id))
//                {
//                    _counterRepository.AddThreadToChannelsUnassignedUnread(business_id, message.channel_id, counter);
//                }
//                else
//                {
//                    _counterRepository.AddTheadToAgentsUnread(business_id, thread.agent_id, counter);
//                }
//            }
//            else
//            {
//                try
//                {
//                    _counterRepository.DeleteThreadFromChannelsUnread(business_id, message.channel_id, counter.id);
//                }
//                catch { }
//                if (string.IsNullOrWhiteSpace(thread.agent_id))
//                {
//                    try
//                    {
//                        _counterRepository.DeleteThreadFromChannelsUnassignedUnread(business_id, message.channel_id, counter.id);
//                    }
//                    catch { }
//                }
//                else
//                {
//                    try
//                    {
//                        _counterRepository.DeleteThreadFromAgentsUnread(business_id, thread.agent_id, counter.id);
//                    }
//                    catch { }
//                }
//            }

//            return true;
//        }

//        public string GetFirebaseStorageAttachmentUrl(string business_id, string folder, string attachment_id)
//        {
//            var url = _storageFactory.GetStorageRef(business_id).Child(folder).Child(attachment_id).GetFullDownloadUrl();
//            return url;
//        }

//        public async Task<string> UploadAttachmentToFirebaseStorage(string business_id, string folder, string attachment_id, Stream stream)
//        {
//            var task = _storageFactory.GetStorageRef(business_id).Child(folder).Child(attachment_id).PutAsync(stream);
//            return await task;
//        }

//        public Attachment GetAttachmentById(string business_id, string channel_id, string id)
//        {
//            return _attachmentRepository.GetById(business_id, channel_id, id);
//        }

//        public bool SaveAttachment(Attachment attachment)
//        {
//            if (string.IsNullOrWhiteSpace(attachment.business_id) || string.IsNullOrWhiteSpace(attachment.channel_id) || string.IsNullOrWhiteSpace(attachment.id)) return false;
//            return _attachmentRepository.Upsert(attachment);
//        }

//    }

//}
