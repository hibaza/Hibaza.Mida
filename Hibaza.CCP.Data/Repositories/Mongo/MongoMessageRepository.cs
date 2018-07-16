using Hibaza.CCP.Core;
using Hibaza.CCP.Data.Providers.Mongo;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Mongo
{
    public class MongoMessageRepository : IMessageRepository
    {
        public  MongoFactory _mongoClient;
        IOptions<AppSettings> _appSettings = null;
        const string messages = "Messages";
        Common _cm = new Common();
        public MongoMessageRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }

        public void AddGroupedByThread(string business_id, Message entity, string threadId)
        {
            throw new NotImplementedException();
        }

        public void AddGroupedByUser(string business_id, Message entity, string userId)
        {
            throw new NotImplementedException();
        }

        public string CreateMessageAgentMap(string business_id, string message_id, string agent_id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string business_id, string id)
        {
            try
            {
                var key = "Messages_Delete" + id;
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";

                _mongoClient.excuteMongoLinqDelete<Message>(query,
            _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
             true, key, DateTime.Now.AddMinutes(10), true);
                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
            }
            catch
            {
                throw new NotImplementedException();
            }
            return true;
        }

        public bool Delete(string business_id, string thread_id, string id)
        {
            throw new NotImplementedException();
        }

        public void DeleteAllForThread(string business_id, string thread_id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Message>> GetAll(string business_id, Paging page)
        {
            List<Message> list = new List<Message>();
            var key = "Message_GetAll" + business_id;

            var options = new FindOptions<Message>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Message>.Sort.Ascending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\"}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public Message GetById(string business_id, string id)
        {
            var key = "Message_GetById" + business_id + id;

            var options = new FindOptions<Message>();
            options.Projection = "{'_id': 0}";
            options.Limit = 1;
            // options.Sort = Builders<Message>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";

            var rs = _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
true, key, DateTime.Now.AddMinutes(10), true).Result;
            if (rs != null && rs.Count > 0)
                return rs[0];
            return null;
        }

        public string GetMessageAgentMap(string business_id, string message_id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Message>> GetStarredMessagesByCustomer(string business_id, Paging page, string customer_id)
        {
            List<Message> list = new List<Message>();

            var key = "GetStarredMessagesByCustomer" + business_id + customer_id;

            var options = new FindOptions<Message>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Message>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\",customer_id:\"" + customer_id + "\",starred:true}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Message>> GetCustomerOrAgentMessagesNonDeletedByThread(string business_id, Paging page, string thread_id)
        {
            List<Message> list = new List<Message>();
            var key = "GetCustomerOrAgentMessagesNonDeletedByThread" + business_id + thread_id;

            var options = new FindOptions<Message>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Message>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{$and:[{business_id:\"" + business_id + "\"},{thread_id:\"" + thread_id + "\"}," +
                                "{deleted:false}," +
                                "{agent_id:{$ne:\"\"}}," +
                                "{timestamp:{$gte:" + long.Parse(page.Next) + "}}," +
                        "]}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<List<Message>> GetMessagesByThread(string business_id, Paging page, string thread_id)
        {
            List<Message> list = new List<Message>();
            var key = "GetMessagesByThread" + business_id + thread_id;

            var options = new FindOptions<Message>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Message>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\",thread_id:\"" + thread_id + "\",timestamp:{$lte:" + long.Parse(page.Next) + "}}";//,timestamp:{$lte:" + long.Parse(page.Next) + "}

            return await _mongoClient.excuteMongoLinqSelect(query, options,
 _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
 true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<List<Message>> GetStartMessagesByThread(string business_id, Paging page, string thread_id)
        {
            List<Message> list = new List<Message>();
            var key = "GetMessagesByThread" + business_id + thread_id;

            var options = new FindOptions<Message>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Message>.Sort.Ascending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\",thread_id:\"" + thread_id + "\",timestamp:{$lte:" + long.Parse(page.Next) + "}}";//

            return await _mongoClient.excuteMongoLinqSelect(query, options,
 _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
 true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Message>> GetNonDeletedMessagesByThread(string business_id, Paging page, string thread_id)
        {
            List<Message> list = new List<Message>();

            var key = "GetNonDeletedMessagesByThread" + business_id + thread_id;

            var options = new FindOptions<Message>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Message>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\",thread_id:\"" + thread_id + "\"," +
                       "deleted:false," +
                        "timestamp:{$lte:" + long.Parse(page.Next) + "}" +
                       "}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
true, key, DateTime.Now.AddMinutes(10), true);
        }


        public async Task<List<Message>> GetMessagesByCustomer(string business_id, string customer_id, Paging page)
        {
            List<Message> list = new List<Message>();
            var key = "GetMessagesByCustomer" + business_id + customer_id;

            var options = new FindOptions<Message>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Message>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\",customer_id:\"" + customer_id + "\"," +
                 "timestamp:{$lte:" + long.Parse(page.Next) + "}" +
                 "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Message>> GetMessagesByCustomerExcludeCurrentThread(string business_id, string customer_id, string channel_ext_id, Paging page)
        {
            List<Message> list = new List<Message>();
            var key = "GetMessagesByCustomerExcludeCurrentThread" + business_id + customer_id;

            var options = new FindOptions<Message>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Message>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\",customer_id:\"" + customer_id + "\"," +
                        "sender_ext_id:{$ne:\"" + channel_ext_id + "\"}," +
                        "timestamp:{$lte:" + long.Parse(page.Next) + "}" +
                        "}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public bool MoveAllUserMessagesTo(string business_id, string from_id, string to_id)
        {
            throw new NotImplementedException();
        }

        public bool Insert(Message message)
        {
            throw new NotImplementedException();
        }
        public bool Update(Message message)
        {
            throw new NotImplementedException();
        }

        public void Upsert(Message message)
        {
            try
            {
                var option = new UpdateOptions { IsUpsert = true };
                var filter = Builders<Message>.Filter.Where(x => x.ext_id == message.ext_id && x.business_id == message.business_id);

                _mongoClient.excuteMongoLinqUpdate<Message>(filter, message, option,
  _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
  true, "", DateTime.Now.AddMinutes(10)).Wait();

                CacheBase.cacheModifyAllKeyLinq(new List<string>() { message.customer_id });
                _mongoClient.MessagesHistory.ReplaceOne(filter, message, options: option);
            }
            catch { }

            try
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    if (!string.IsNullOrWhiteSpace(message.message))
                    {
                        var dicConfig = new Dictionary<string, string>();
                        dicConfig.Add("session_customer", message.customer_id);
                        dicConfig.Add("using_full_search", "0");
                        dicConfig.Add("page_id", message.channel_ext_id);
                        dicConfig.Add("auto_agents", message.channel_ext_id == message.sender_ext_id ? "autoAgents" : "ManualAgents");
                        dicConfig.Add("business_id", message.business_id);

                        var dicPara = new Dictionary<string, string>();
                        dicPara.Add("q", message.message);

                        var json = new Dictionary<string, string>();
                        json.Add("config", JsonConvert.SerializeObject(dicConfig));
                        json.Add("para", JsonConvert.SerializeObject(dicPara));

                        _cm.PostAsync(_appSettings.Value.BaseUrls.ApiAi + "api/AiExcuteAll/Excute", JsonConvert.SerializeObject(json));
                    }
                });
            }
            catch { }
        }

        public bool MarkAsDeleted(string business_id, string id)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            var option = new UpdateOptions { IsUpsert = false };
            var filter = Builders<Message>.Filter.Where(x => x.id == id && x.business_id == business_id);
            var update = Builders<Message>.Update.Set(x => x.deleted, true);

            _mongoClient.excuteMongoLinqUpdateColumns<Message>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
            return true;
            //});
            return true;
        }

        public bool MarkAsReplied(string business_id, string id, long replied_at)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            var option = new UpdateOptions { IsUpsert = false };
            var filter = Builders<Message>.Filter.Where(x => x.id == id && x.business_id == business_id);
            var update = Builders<Message>.Update.Set(x => x.replied, true).Set(x => x.replied_at, replied_at);

            _mongoClient.excuteMongoLinqUpdateColumns<Message>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
            ///});
            return true;
        }

        public int UpdateCustomerId()
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            //    try
            //    {
            //        var options = new FindOptions<BsonDocument>();
            //        options.Projection = "{'_id': 0}";
            //        options.Limit = 10;
            //        var queryMg = "{customer_id:null}";
            //        var key = "Messages_UpdateCustomerId";
            //        var mess = _mongoClient.Messages.FindAsync<BsonDocument>(queryMg, options).Result.ToList();
            //        if (mess != null && mess.Count > 0)
            //        {
            //            foreach (var m in mess)
            //            {
            //                try
            //                {
            //                    var queryTh = "{id:\"" + m["thread_id"] + "\"}";
            //                    var thread = _mongoClient.Threads.FindAsync<BsonDocument>(queryTh).Result.ToList();
            //                    if (thread != null && thread.Count > 0)
            //                    {
            //                        foreach (var val in thread)
            //                        {
            //                            var para = new BsonDocument();
            //                            para.Add("customer_id", val["customer_id"]);

            //                            BsonDocument document = new BsonDocument();
            //                            document.Add("$set", para);

            //                            var filter = new BsonDocument();
            //                            filter.Add("id", m["id"]);

            //                            var option = new UpdateOptions { IsUpsert = false };

            //                            _mongoClient.excuteMongoLinqUpdate<Message>(filter, document, option,
            //    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
            //    true, key, DateTime.Now.AddMinutes(10));

            //                            _mongoClient.MessagesHistory.UpdateManyAsync(filter: filter, update: document, options: option);
            //                        }
            //                    }
            //                }
            //                catch (Exception ex) { }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //});
            return 1;
        }

        public async Task<IEnumerable<Message>> GetMessagesWhereCustomerIsNull(string business_id, int limit)
        {
            List<Message> list = new List<Message>();

            var options = new FindOptions<Message>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Message>.Sort.Descending("timestamp");
            options.Limit = limit;
            var query = "{customer_id:null }";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
false, "", DateTime.Now.AddMinutes(10), false);
        }


        public async Task<bool> deleteMessageidIfNull(string business_id, string message_id)
        {
            try
            {
                var key = "deleteMessageidIfNull" + message_id;
                var query = "{business_id:\""+business_id+"\", id:\"" + message_id + "\"}";

                await _mongoClient.excuteMongoLinqDelete<Message>(query,
              _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, messages,
               false, key, DateTime.Now.AddMinutes(10), false);
                return true;
            }
            catch (Exception ex) { return false; }
        }

    }
}
