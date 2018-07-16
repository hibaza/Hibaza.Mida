using Dapper;
using Firebase.Database.Query;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Hibaza.CCP.Domain.Models;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public class FirebaseMessageRepository : IFirebaseMessageRepository
    {
        IFirebaseFactory _connectionFactory;
        private const string USERS_MESSAGES = "users-messages";
        private const string THREADS_MESSAGES = "threads-messages";
        private const string MESSAGES = "messages";
        private const string MESSAGES_AGENTS_MAP = "messages-agents-map";

        public FirebaseMessageRepository(IFirebaseFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public MessageModel GetById(string id)
        {
            return _connectionFactory.GetConnection
            .Child(MESSAGES).Child(id).OnceSingleAsync<MessageModel>().Result;
        }

        public MessageModel GetById(string business_id, string id)
        {
            return _connectionFactory.GetConnection.Child(business_id)
            .Child(MESSAGES).Child(id).OnceSingleAsync<MessageModel>().Result;
        }

        public void DeleteAllForThread(string business_id, string thread_id)
        {
            var r = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_MESSAGES).Child(thread_id).DeleteAsync();
        }

        public IEnumerable<T> GetAll<T>(string business_id, Paging page)
        {
            return _connectionFactory.GetConnection.Child(business_id).Child(MESSAGES).OrderBy("timestamp").EndAt(long.Parse(page.Next)).LimitToFirst(page.Limit).OnceAsync<T>().Result.Select(m => m.Object);
            //return _connectionFactory.GetConnection.Child(business_id).Child(MESSAGES).OrderBy("created_time").StartAt(page.Previous).EndAt(page.Next).OnceAsync<T>().Result.Select(m => m.Object);
        }

        public void Add(MessageModel message)
        {
            var rs = _connectionFactory.GetConnection.Child(MESSAGES).Child(message.id).PutAsync(message);
        }

        public void Upsert(string business_id, MessageModel message)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(MESSAGES).Child(message.id).PutAsync(message);
        }

        public void AddGroupedByUser(string business_id, MessageModel message, string userId)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(USERS_MESSAGES).Child(userId).Child(MESSAGES).Child(message.id).PutAsync(message);
        }

        public void AddGroupedByThread(string business_id, MessageModel message, string threadId)
        {
            try
            {
                //var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_MESSAGES).Child(threadId).Child(MESSAGES).Child(message.id).PutAsync(message);
                var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_MESSAGES).Child(threadId).Child(MESSAGES).Child(message.id).PutAsync(message);
                rs.Wait();
            }
            catch { }
        }


        public bool MoveAllUserMessagesTo(string business_id, string from_id, string to_id)
        {
            if (!string.IsNullOrWhiteSpace(to_id) && !string.IsNullOrWhiteSpace(from_id))
            {
                var node = _connectionFactory.GetConnection.Child(business_id).Child(USERS_MESSAGES).Child(from_id).OnceSingleAsync<dynamic>().Result;
                var copy = _connectionFactory.GetConnection.Child(business_id).Child(USERS_MESSAGES).Child(to_id).PutAsync(node);
            }
            if (!string.IsNullOrWhiteSpace(from_id)){
                var delete = _connectionFactory.GetConnection.Child(business_id).Child(USERS_MESSAGES).Child(from_id).DeleteAsync();
            }
            return true;
        }

        public async Task<dynamic> GetMessagesByUser(string business_id, Paging page, string userId)
        {
            var data = string.IsNullOrWhiteSpace(userId) ? await _connectionFactory.GetConnection.Child(business_id)
            .Child(MESSAGES).OrderBy("timestamp").LimitToLast(page.Limit).OnceAsync<Message>()
            :             await _connectionFactory.GetConnection.Child(business_id)
            .Child(USERS_MESSAGES).Child(userId).Child(MESSAGES).OrderBy("updated_time").LimitToLast(page.Limit).OnceAsync<Message>();
            return data;
        }

        public async Task<dynamic> GetMessagesByThread(string business_id, Paging page, string threadId)
        {
            var data = await _connectionFactory.GetConnection.Child(business_id)
            .Child(THREADS_MESSAGES).Child(threadId).Child(MESSAGES).OrderBy("timestamp").LimitToLast(page.Limit).OnceAsync<FbMessage>();
            return data;
        }

        public bool DeleteFolder()
        {
            var rs = _connectionFactory.GetConnection.Child(MESSAGES).DeleteAsync();
            return true;
        }

        public bool Delete(string business_id, string id)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(MESSAGES).Child(id).DeleteAsync();
            return true;
        }

        public bool Delete(string business_id, string thread_id, string id)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(THREADS_MESSAGES).Child(thread_id).Child(MESSAGES).Child(id).DeleteAsync();
            return true;
        }


        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public string CreateMessageAgentMap(string business_id, string message_id, string agent_id)
        {
            var data = _connectionFactory.GetConnection.Child(business_id)
                .Child(MESSAGES_AGENTS_MAP).Child(message_id).PutAsync(agent_id);
            return agent_id;
        }

        public string GetMessageAgentMap(string business_id, string message_id)
        {
            var data = _connectionFactory.GetConnection.Child(business_id)
                .Child(MESSAGES_AGENTS_MAP).Child(message_id).OnceSingleAsync<string>();
            return data.Result;
        }
    }
}
