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
using Hibaza.CCP.Data.Providers.Mongo;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Core;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Hibaza.CCP.Data.Repositories
{
    public class MongoThreadRepository : IThreadRepository
    {
        private const string THREADS = "threads";
        private const string THREADS_ATTACHMENTS = "threads-attachments";
        private const string REFERRALS = "referrals";
        public MongoFactory _mongoClient;
        const string threads = "Threads";
        IOptions<AppSettings> _appSettings = null;
        Common _cm = new Common();
        public MongoThreadRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }

        public Thread GetById(string business_id, string id)
        {
            var key = "Thread_GetById" + business_id + id;

            var options = new FindOptions<Thread>();
            options.Projection = "{'_id': 0}";
            options.Limit = 1;
            // options.Sort = Builders<Thread>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\", id:\"" + id + "\"}";

            var rs = _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
true, key, DateTime.Now.AddMinutes(10), true).Result;
            if (rs != null && rs.Count > 0)
                return rs[0];
            return null;
        }

        public bool UpdateLastVisits(string business_id, string id, string last_visits)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            var option = new UpdateOptions { IsUpsert = false };
            var filter = Builders<Thread>.Filter.Where(x => x.id == id && x.business_id == business_id);
            var update = Builders<Thread>.Update.Set(p => p.last_visits, last_visits);

            _mongoClient.excuteMongoLinqUpdateColumns<Thread>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
true, "", DateTime.Now.AddMinutes(10)).Wait();
            CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
            // });
            return true;
        }

        public int UpdateCustomerId()
        {
            return 0;
        }

        public bool UpdateCustomerId(string business_id, string id, string customer_id)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            var option = new UpdateOptions { IsUpsert = false };
            var filter = Builders<Thread>.Filter.Where(x => x.id == id && x.business_id == business_id);
            var update = Builders<Thread>.Update.Set(x => x.customer_id, customer_id);

            _mongoClient.excuteMongoLinqUpdateColumns<Thread>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { customer_id });
            //  });
            return true;
        }



        public bool Update(Thread thread)
        {
            throw new NotImplementedException();
        }


        public bool Insert(Thread thread)
        {
            throw new NotImplementedException();
        }

        public long Upsert(string business_id, Thread thread)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Thread>.Filter.Where(x => x.id == thread.id && x.business_id == thread.business_id);

            _mongoClient.excuteMongoLinqUpdate<Thread>(filter, thread, option,
 _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
 true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { thread.customer_id });
            return 1;
        }

        public void UpsertSP(string business_id, Thread thread)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Thread>.Filter.Where(x => x.id == thread.id && x.business_id == thread.business_id);

            _mongoClient.excuteMongoLinqUpdate<Thread>(filter, thread, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { thread.customer_id });
            // });
        }

        public async Task<IEnumerable<Thread>> SearchThreadsDistinctCustomerByKeywords(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page)
        {
            long end;
            if (!long.TryParse(page.Next, out end))
            {
                end = 9999999999999;
            }

            List<Thread> list = new List<Thread>();
            var key = "SearchThreadsDistinctCustomerByKeywords" + business_id + channel_id + agent_id + status + flag + keywords;

            var options = new FindOptions<Thread>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Thread>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{$and:[{business_id:\"" + business_id + "\"},";
            if (channel_id != null && channel_id != "")
                query += "{channel_id:\"" + channel_id + "\"},";
            if (agent_id != null && agent_id != "")
                query += "{agent_id:\"" + agent_id + "\"},";
            if (status != null && status != "")
                query += "{status:\"" + status + "\"},";
            query += "{$or:[{unread:true},{nonreply:true}]},";
            if (keywords != null && keywords != "")
                query += "{owner_name:/" + keywords + "/},";

            // queryMg += "{timestamp:{$lte:" + end + "}}" +
            query += "]}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
 _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
 true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Thread>> SearchThreadsByKeywords(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page)
        {
            var key = "SearchThreadsByKeywords" + business_id + channel_id + agent_id + status + flag + keywords;
            List<Thread> list = new List<Thread>();

            long end;
            if (!long.TryParse(page.Next, out end))
            {
                end = 9999999999999;
            }
            var options = new FindOptions<Thread>();
            options.Projection = "{'_id': 0}";
            // options.Sort = Builders<BsonDocument>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{$and:[{business_id:\"" + business_id + "\"},";
            if (channel_id != null && channel_id != "")
                query += "{channel_id:\"" + channel_id + "\"},";
            if (agent_id != null && agent_id != "")
                query += "{agent_id:\"" + agent_id + "\"},";
            if (status != null && status != "")
                query += "{status:\"" + status + "\"},";
            query += "{$or:[{unread:true},{nonreply:true}]},";
            if (keywords != null && keywords != "")
                query += "{owner_name:/" + keywords + "/},";

            // queryMg += "{timestamp:{$lte:" + end + "}}" +
            query += "]}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Thread>> GetThreadsByCustomer(string business_id, string customer_id, Paging page)
        {
            List<Thread> list = new List<Thread>();
            var key = "GetThreadsByCustomer" + business_id + customer_id;

            var options = new FindOptions<Thread>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Thread>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\"," +
                            "customer_id:\"" + customer_id + "\"" +
                            "}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Thread>> GetUnreadOrNonReplyThreadsByCustomer(string business_id, string customer_id, Paging page)
        {
            List<Thread> list = new List<Thread>();
            var key = "GetUnreadOrNonReplyThreadsByCustomer" + business_id + customer_id;

            var options = new FindOptions<Thread>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Thread>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{$and:[{business_id:\"" + business_id + "\"}," +
                            "{customer_id:\"" + customer_id + "\"}," +
                            "{$or:[{unread:true},{nonreply:true}]}" +
                            "]}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Thread>> GetThreadsWhereCustomerIsNull(string business_id, Paging page)
        {
            List<Thread> list = new List<Thread>();
            var key = "GetThreadsWhereCustomerIsNull" + business_id;

            var options = new FindOptions<Thread>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Thread>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\""+business_id+"\",customer_id:null}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
 _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
 true, key, DateTime.Now.AddMinutes(10), true);
        }


        public Task<IEnumerable<Thread>> GetThreads(string businessId, Paging page)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> GetThreadsByChannel(string businessId, Paging page, string channelId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> GetThreadsByChannelAndAgent(string businessId, Paging page, string channelId, string agentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> GetThreadsByChannelAndStatus(string businessId, Paging page, string channelId, string status)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> GetThreadsByChannelAndAgentAndStatus(string businessId, Paging page, string channelId, string agentId, string status)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> GetThreadsByAgentAndStatus(string businessId, Paging page, string agentId, string status)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> GetThreadsByAgent(string businessId, Paging page, string agentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> GetThreadsByStatus(string businessId, Paging page, string status)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> GetThreadsByFlag(string businessId, Paging page, string flag)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> GetThreadsByAgentAndFlag(string businessId, Paging page, string agentId, string flag)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> GetThreadsByChannelAndFlag(string businessId, Paging page, string channelId, string flag)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Thread>> GetThreadsByChannelAndAgentAndFlag(string businessId, Paging page, string channelId, string agentId, string flag)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Thread> GetUnreadThreads(string businessId, Paging page)
        {
            throw new NotImplementedException();
        }

        public string GetPageUIDByAppUID(string auid)
        {
            throw new NotImplementedException();
        }

        public string GetBusinessUIDByPageUID(string puid)
        {
            throw new NotImplementedException();
        }

        public void UpdatePageBusinessMapping(string buid, string puid, string @ref)
        {
            throw new NotImplementedException();
        }

        public void UpdateAppPageMapping(string puid, string auid, string @ref)
        {
            throw new NotImplementedException();
        }

        public string GetAppRefParam(string auid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Thread> GetAll(string business_id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string business_id, string id)
        {
            var document = new BsonDocument();
            try
            {
                var key = "Threads_Delete" + id;
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";

                _mongoClient.excuteMongoLinqDelete<Customer>(query,
            _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
             true, key, DateTime.Now.AddMinutes(10), true).Wait();
                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
            }
            catch (Exception)
            {
            }
            return true;
        }

        public async Task<IEnumerable<Thread>> GetActiveUnreadThreads(string business_id, string agent_id, Paging page)
        {
            List<Thread> list = new List<Thread>();
            var key = "GetActiveUnreadThreads" + business_id + agent_id;

            var options = new FindOptions<Thread>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Thread>.Sort.Ascending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\"," +
                            "agent_id:\"" + agent_id + "\"," +
                            "status:\"active\"," +
                            "unread:true," +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
 _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
 true, key, DateTime.Now.AddMinutes(10), true);
        }


        public async Task<IEnumerable<Thread>> GetNoReponseThreads(string business_id, string channel_id, Paging page)
        {
            List<Thread> list = new List<Thread>();
            var key = "GetNoReponseThreads" + business_id + channel_id;

            long until;
            if (!long.TryParse(page.Next, out until))
            {
                until = 9999999999999;
            }

            long since;
            if (!long.TryParse(page.Previous, out since))
            {
                since = 0;
            }

            var options = new FindOptions<Thread>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Thread>.Sort.Ascending("timestamp");
            options.Limit = page.Limit;
            var query = "{$and:[{business_id:\"" + business_id + "\"}," +
                            "{$or:[{channel_id:\"" + channel_id + "\"},{channel_id:\"\"},{channel_id:null}]}," +
                                    "{type:\"message\"}," +
                                    "{nonreply:false}," +
                                    "{unread:false}," +
                                    "{timestamp:{$gte:" + since + "}}," +
                                    "{timestamp:{$lte:" + until + "}}," +
                            "]}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
 _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
 true, key, DateTime.Now.AddMinutes(10), true);
        }


        public async Task<IEnumerable<Counter>> GetChannelCounters(string business_id)
        {
            List<Counter> list = new List<Counter>();
            var key = "GetChannelCounters" + business_id;

            var match = "{$match:{business_id:\"" + business_id + "\"}}";
            var group = "{$group:{_id:{channel_id:\"$channel_id\"}, " +
                                "unread :{ $first: '$unread' }," +
                                "id: { $first: '$channel_id' }" +
                                ",count:{$sum:1}}}";
            var project = "{$project:{_id:0}}";
            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project) };

            var result = await _mongoClient.Threads.AggregateAsync<BsonDocument>(Pipeline);

            var d = result.ToList();
            list = await _mongoClient.DeserializeBsonToEntity<Counter>(d);
            return list;
        }

        public async Task<IEnumerable<Counter>> GetAgentCounters(string business_id)
        {
            List<Counter> list = new List<Counter>();
            var key = "GetAgentCounters" + business_id;

            var match = "{$match:{business_id:\"" + business_id + "\"}}";
            var group = "{$group:{_id:{agent_id:\"$agent_id\"}, " +
                                "unread :{ $first: '$unread' }," +
                                "id: { $first: '$agent_id' }" +
                                ",count:{$sum:1}}}";
            var project = "{$project:{_id:0}}";
            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project) };

            var result = await _mongoClient.Threads.AggregateAsync<BsonDocument>(Pipeline);

            var d = result.ToList();
            list = await _mongoClient.DeserializeBsonToEntity<Counter>(d);
            return list;
        }
        public Thread GetByIdFromCustomerId(string business_id, string customerId)
        {
            var key = "GetByIdFromCustomerId" + customerId;

            var options = new FindOptions<Thread>();
            options.Projection = "{'_id': 0}";
            options.Limit = 1;
            options.Sort = Builders<Thread>.Sort.Descending("timestamp");
            var query = "{business_id : \""+business_id+"\",customer_id:\"" + customerId + "\"}";

            var rs = _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, threads,
true, key, DateTime.Now.AddMinutes(10), true).Result;
            if (rs != null && rs.Count > 0)
                return rs[0];
            return null;
        }

        public async Task<int> Job_AutoCallProcedure(string procedure)
        {
            var client = new MongoClient(_appSettings.Value.MongoDB.ConnectionString);
            var database = client.GetDatabase(_appSettings.Value.MongoDB.Database);

            var cmd = await _cm.formatCommandProcedure(procedure, null);
            var rs = await database.RunCommandAsync<BsonDocument>(cmd);
            return 1;
        }


    }
}
