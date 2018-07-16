using Dapper;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Hibaza.CCP.Domain.Models;
using Firebase.Database.Query;
using Firebase.Database;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public class FirebaseThreadRepository : IFirebaseThreadRepository
    {
        private const string THREADS = "threads";
        private const string THREADS_ATTACHMENTS = "threads-attachments";
        private const string REFERRALS = "referrals";
        FirebaseClient _client;
        public FirebaseThreadRepository(IFirebaseFactory connectionFactory)
        {
            _client = connectionFactory.GetConnection;
        }

        public Thread GetById(string business_id, string id)
        {
            var c= _client.Child(business_id)
            .Child(THREADS).Child(id).OnceSingleAsync<Thread>().Result;

            return c;
        }

        public Thread GetById(string id)
        {
            var c = _client.Child(THREADS).Child(id).OnceSingleAsync<Thread>().Result;
            return c;
        }

        public void MapThreadWithConversation(string business_id, string threadId, string conversationId, bool done)
        {
            var rs = _client.Child(business_id).Child("thread-conversation-map").Child(conversationId).PutAsync(new { id = threadId, done });
        }

        public string GetThreadByConversation(string business_id, string conversationId)
        {
            var rs = _client.Child(business_id).Child("thread-conversation-map").Child(conversationId).OnceSingleAsync<dynamic>().Result;
            return rs == null ? "" : rs.id;
        }

        public string GetAppRefParam(string auid)
        {
            var rs = _client.Child("app-page-usermap").Child(auid).OnceSingleAsync<dynamic>().Result;
            return rs == null ? "" : (rs.@ref ?? "");
        }

        public string GetPageUIDByAppUID(string auid)
        {
            var rs = _client.Child("app-page-usermap").Child(auid).OnceSingleAsync<dynamic>().Result;
            return rs == null ? "" : (rs.puid ?? "");
        }

        public void UpdateAppPageMapping(string puid, string auid, string @ref)
        {
            //var rs = _client.Child("app-page-usermap").Child(auid).PostAsync(new { puid = puid, @ref = @ref }).Result;
            var rs = _client.Child("app-page-usermap").Child(auid).PutAsync(new { puid = puid, @ref = @ref });
        }


        public void UpdatePageBusinessMapping(string buid, string puid, string @ref)
        {
            var rs = _client.Child("page-business-usermap").Child(puid).PutAsync(new { buid = buid, @ref = @ref });
        }


        public string GetBusinessUIDByPageUID(string puid)
        {
            var rs = _client.Child("page-business-usermap").Child(puid).OnceSingleAsync<dynamic>().Result;
            return rs == null ? "" : (rs.buid ?? "");
        }


        public void CreateReferral(string business_id, string thread_id, string referral_id, string referral_name)
        {
            var rs = _client.Child(business_id).Child(THREADS_ATTACHMENTS).Child(thread_id).Child(REFERRALS).Child(referral_id).PutAsync(referral_name);
        }


        public void Update(string business_id, Thread entity)
        {
            var rs = _client.Child(business_id).Child(THREADS).Child(entity.id).PutAsync(entity);
        }

        public async Task<IEnumerable<Thread>> SearchThreadsByCustomerName(string business_id, string child, string key, Paging page)
        {
            var list = new List<Domain.Entities.Thread>();
            var data = await _client.Child(business_id).Child(THREADS).OrderBy(child).EqualTo(key).LimitToLast(page.Limit).OnceAsync<dynamic>();
            if (data != null)
                foreach (var c in data)
                {
                    list.Add(c.Object);
                }
            return list.OrderByDescending(n => n.owner_name);

        }

        public async Task<IEnumerable<Thread>> GetThreads(string business_id, Paging page)
        {
            long endAt = long.Parse(page.Next ?? "99999999999");
            return _client.Child(business_id)
            .Child(THREADS).OrderBy("timestamp").EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t => t.Object);
        }

        public async Task<IEnumerable<Thread>> GetThreadsByFlag(string business_id, Paging page, string flag)
        {
            string startAt = flag + "0000000000";
            string endAt = flag + (page.Next ?? "9999999999");

            return _client.Child(business_id)
            .Child(THREADS).OrderBy("flag_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t=>t.Object);
        }

        public async Task<IEnumerable<Thread>> GetThreadsByChannelAndFlag(string business_id, Paging page, string channelId, string flag)
        {
            string startAt = channelId + flag + "0000000000";
            string endAt = channelId + flag + (page.Next ?? "9999999999");

            return _client.Child(business_id)
            .Child(THREADS).OrderBy("channel_flag_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t => t.Object);
        }

        public async Task<IEnumerable<Thread>> GetThreadsByChannelAndAgentAndFlag(string business_id, Paging page, string channelId, string agentId, string flag)
        {
            string startAt = channelId + agentId + flag + "0000000000";
            string endAt = channelId + agentId + flag + (page.Next ?? "9999999999");

            return _client.Child(business_id)
            .Child(THREADS).OrderBy("channel_agent_flag_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t => t.Object); 
        }

        public async Task<IEnumerable<Thread>> GetThreadsByAgentAndFlag(string business_id, Paging page, string agentId, string flag)
        {
            string startAt = agentId + flag + "0000000000";
            string endAt = agentId + flag + (page.Next ?? "9999999999");

            return _client.Child(business_id)
            .Child(THREADS).OrderBy("agent_flag_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t => t.Object); 
        }


        public async Task<IEnumerable<Thread>> GetThreadsByChannel(string business_id, Paging page, string channelId)
        {

            string startAt = channelId + "0000000000";
            string endAt = channelId + (page.Next ?? "9999999999");

            return _client.Child(business_id)
            .Child(THREADS).OrderBy("channel_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t => t.Object); 
        }

        public async Task<IEnumerable<Thread>> GetThreadsByChannelAndAgent(string business_id, Paging page, string channelId, string agentId)
        {
            string startAt = channelId + agentId + "0000000000";
            string endAt = channelId + agentId + (page.Next ?? "9999999999");
            return _client.Child(business_id)
            .Child(THREADS).OrderBy("channel_agent_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t => t.Object);
        }

        public async Task<IEnumerable<Thread>> GetThreadsByChannelAndStatus(string business_id, Paging page, string channelId, string status)
        {
            string startAt = channelId + status + "0000000000";
            string endAt = channelId + status + (page.Next ?? "9999999999");
            return _client.Child(business_id)
            .Child(THREADS).OrderBy("channel_status_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t => t.Object);
        }

        public async Task<IEnumerable<Thread>> GetThreadsByAgent(string business_id, Paging page, string agentId)
        {
            string startAt = agentId + "0000000000";
            string endAt = agentId + (page.Next ?? "9999999999");
            return _client.Child(business_id)
            .Child(THREADS).OrderBy("agent_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t => t.Object);
        }

        public async Task<IEnumerable<Thread>> GetThreadsByStatus(string business_id, Paging page, string status)
        {
            string startAt = status + "0000000000";
            string endAt = status + (page.Next ?? "9999999999");
            return _client.Child(business_id)
            .Child(THREADS).OrderBy("status_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t => t.Object);
        }


        public async Task<IEnumerable<Thread>> GetThreadsByChannelAndAgentAndStatus(string business_id, Paging page, string channelId, string agentId, string status)
        {
            string startAt = channelId + agentId + status + "0000000000";
            string endAt = channelId + agentId + status + (page.Next ?? "9999999999");
            return _client.Child(business_id)
            .Child(THREADS).OrderBy("channel_agent_status_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t => t.Object);
        }

        public async Task<IEnumerable<Thread>> GetThreadsByAgentAndStatus(string business_id, Paging page, string agentId, string status)
        {
            string startAt = agentId +  status + "0000000000";
            string endAt = agentId + status + (page.Next ?? "9999999999");
            return _client.Child(business_id)
            .Child(THREADS).OrderBy("agent_status_timestamp").StartAt(startAt).EndAt(endAt).LimitToLast(page.Limit).OnceAsync<Thread>().Result.Select(t => t.Object);
        }


        public IEnumerable<Thread> GetAll(string business_id)
        {
            return _client.Child(business_id).Child(THREADS).OnceAsync<Thread>().Result.Select(t=>t.Object);
        }

        public IEnumerable<Thread> GetAll()
        {
            return _client.Child(THREADS).OnceAsync<Thread>().Result.Select(t => t.Object);
        }

        public void Add(Thread entity)
        {
            var rs = _client.Child(THREADS).Child(entity.id).PutAsync(entity);
        }

        public void Upsert(string business_id, Thread entity)
        {
            var rs = _client.Child(business_id).Child(THREADS).Child(entity.id).PutAsync(entity);
        }

        public void Update(Thread entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Thread> GetUnreadThreads(string businessId, Paging page)
        {
            return GetThreadsByFlag(businessId, page, "unread").Result;
        }

        public bool Delete(string business_id, string id)
        {
            var rs = _client.Child(business_id).Child(THREADS).Child(id).DeleteAsync();
            return true;
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> SearchThreadsByCustomer(string business_id, string keywords, Paging page)
        {
            throw new NotImplementedException();
        }
    }
}
