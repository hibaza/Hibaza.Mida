using Dapper;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Data.Providers.Mongo;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Core;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Hibaza.CCP.Data.Repositories
{
    public class MongoConversationRepository : IConversationRepository
    {
        public MongoFactory _mongoClient;
        IOptions<AppSettings> _appSettings = null;
        const string conversations = "Conversations";
        public MongoConversationRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }

        public bool Insert(Conversation conversation)
        {
            throw new NotImplementedException();
        }

        public bool UpdateOwner(string business_id, string id, string owner_ext_id, string owner_app_id)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                var option = new UpdateOptions { IsUpsert = false };
                var filter = Builders<Conversation>.Filter.Where(x => x.id == id && x.business_id == business_id);
                var update = Builders<Conversation>.Update.Set(x => x.owner_app_id, owner_app_id)
                .Set(x => x.owner_ext_id, owner_ext_id).Set(x => x.updated_time, DateTime.Now);

                _mongoClient.excuteMongoLinqUpdateColumns<Conversation>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, conversations,
true, "", DateTime.Now.AddMinutes(10)).Wait();

                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
            //});
            return true;
        }

        private bool Update(Conversation conversation)
        {
            throw new NotImplementedException();
        }

        private bool UpdateTimestamp(string business_id, string id, long timestamp)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                var option = new UpdateOptions { IsUpsert = false };
                var filter = Builders<Conversation>.Filter.Where(x => x.id == id && x.business_id == business_id);
                var update = Builders<Conversation>.Update.Set(x => x.timestamp, timestamp);

                _mongoClient.excuteMongoLinqUpdateColumns<Conversation>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, conversations,
true, "", DateTime.Now.AddMinutes(10));

                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
            //});
            return true;
        }


        public void Upsert(Conversation conversation)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Conversation>.Filter.Where(x => x.id == conversation.id && x.business_id == conversation.business_id);

          _mongoClient.excuteMongoLinqUpdate<Conversation>(filter, conversation, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, conversations,
true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { conversation.id });

        }

        public void UpsertTimestamp(Conversation conversation)
        {
            UpdateTimestamp(conversation.business_id, conversation.id, conversation.timestamp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="business_id"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Conversation GetById(string business_id, string id)
        {
            var key = "Conversation_GetById" + business_id + id;

            var options = new FindOptions<Conversation>();
            options.Projection = "{'_id': 0}";
            options.Limit = 1;
            //options.Sort = Builders<Conversation>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";

            var rs = _mongoClient.excuteMongoLinqSelect(query, options,
              _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, conversations,
               true, key, DateTime.Now.AddMinutes(10), true).Result;
            if (rs != null && rs.Count > 0)
                return rs[0];
            return null;
        }


        public bool UpdateStatus(string business_id, string id, string status)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
                var option = new UpdateOptions { IsUpsert = false };
                var filter = Builders<Conversation>.Filter.Where(x => x.id == id && x.business_id == business_id);
                var update = Builders<Conversation>.Update.Set(x => x.status, status).Set(x => x.updated_time, DateTime.Now);

                _mongoClient.excuteMongoLinqUpdateColumns<Conversation>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, conversations,
true, "", DateTime.Now.AddMinutes(10)).Wait();

                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
           // });
            return true;
        }

        public async Task<IEnumerable<Conversation>> GetConversations(string business_id, string channel_id, Paging page)
        {
            List<Conversation> list = new List<Conversation>();
            try
            {               
                var key = "GetConversations" + business_id + channel_id;

                //var options = new FindOptions<Conversation>();
                //options.Projection = "{'_id': 0,timestamp:0,created_time:0,updated_time:0}";
                //options.Limit = page.Limit;
                //options.Sort = Builders<Conversation>.Sort.Descending("timestamp");
                //var query = "{business_id:\"" + business_id + "\"" +
                //            ",channel_id:\"" + channel_id + "\"" +
                //           // ",timestamp:{$lte:" + (long.Parse(page.Next ?? "9999999999")) + "}" +
                //           // ",timestamp:{$gte:" + (long.Parse(page.Previous ?? "0")) + "}" +

                //    "}";

                //list = await _mongoClient.excuteMongoLinqSelect(query, options,
                // _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, conversations,
                //  true, key, DateTime.Now.AddMinutes(10), true);


                var options = new FindOptions<Conversation>();
                options.Projection = "{'_id': 0}";
                options.Sort = Builders<Conversation>.Sort.Descending("timestamp");
                options.Limit = page.Limit;
                var query = "{$and:[{business_id:\"" + business_id + "\"},{channel_id:\"" + channel_id + "\"},"+
                    "{timestamp:{$lt: " + (long.Parse(page.Next ?? "9999999999")) + "}}"+
                    ",{timestamp:{$gte:" + (long.Parse(page.Previous ?? "0")) + "}}]}";

                list = await _mongoClient.excuteMongoLinqSelect(query, options,
    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, conversations,
    true, key, DateTime.Now.AddMinutes(10), true);


                return list;
            }
            catch { return list; }
        }

        public async Task<IEnumerable<Conversation>> GetConversationsWhereExtIdIsNull(string business_id, string channel_id, int limit)
        {
            List<Conversation> list = new List<Conversation>();
            var key = "GetConversationsWhereExtIdIsNull" + business_id + channel_id;

            var options = new FindOptions<Conversation>();
            options.Projection = "{'_id': 0}";
            options.Limit = limit;
            options.Sort = Builders<Conversation>.Sort.Descending("timestamp");
            var query = "{$and:[{business_id:\"" + business_id + "\"}," +
                "{channel_id:\"" + channel_id + "\"}," +
                "{$or:[{owner_ext_id:null},{owner_ext_id:\"\"}]}," +
                "]}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
             _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, conversations,
              true, key, DateTime.Now.AddMinutes(10), true);
        }

        public Conversation GetByOwnerExtId(string business_id, string owner_ext_id)
        {
            var key = "GetByOwnerExtId" + business_id + owner_ext_id;
            var options = new FindOptions<Conversation>();
            options.Projection = "{'_id': 0}";
            options.Limit = 1;
            options.Sort = Builders<Conversation>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                "owner_ext_id:\"" + owner_ext_id + "\"," +
                "}";

            var rs = _mongoClient.excuteMongoLinqSelect(query, options,
            _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, conversations,
             true, key, DateTime.Now.AddMinutes(10), true).Result;
            if (rs != null && rs.Count > 0)
                return rs[0];
            return null;
        }
        public Conversation GetByOwnerAppId(string business_id, string channel_id, string owner_app_id)
        {
            var key = "GetByOwnerAppId" + business_id + channel_id + owner_app_id;
            var options = new FindOptions<Conversation>();
            options.Projection = "{'_id': 0}";
            options.Limit = 1;
            options.Sort = Builders<Conversation>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                "channel_id:\"" + channel_id + "\"," +
                "owner_app_id:\"" + owner_app_id + "\"," +
                "}";

            var rs = _mongoClient.excuteMongoLinqSelect(query, options,
            _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, conversations,
             true, key, DateTime.Now.AddMinutes(10), true).Result;
            if (rs != null && rs.Count > 0)
                return rs[0];
            return null;
        }

        public async Task<ReplaceOneResult> UpsertAnyMongo<T>(T obj, UpdateOptions option , FilterDefinition<T> filter ,string collectionName) where T : class
        {
            //var option = new UpdateOptions { IsUpsert = true };
            //var filter = Builders<Conversation>.Filter.Where(x => x.id == conversation.id && x.business_id == conversation.business_id);

          var t= await _mongoClient.excuteMongoLinqUpdate<T>(filter, obj, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, collectionName,
true, "", DateTime.Now.AddMinutes(10));

            return t;

        }

        public async Task<List<T>> GetDataMongo<T>(string query, FindOptions<T> options,string collectionName) where T : class
        {
            List<T> list = new List<T>();
            return await _mongoClient.excuteMongoLinqSelect(query, options,
            _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, collectionName,
             false, "", DateTime.Now.AddMinutes(10), false);

            //var key = "GetConversationsWhereExtIdIsNull" + business_id + channel_id;

            //var options = new FindOptions<Conversation>();
            //options.Projection = "{'_id': 0}";
            //options.Limit = limit;
            //options.Sort = Builders<Conversation>.Sort.Descending("timestamp");
            //var query = "{$and:[{business_id:\"" + business_id + "\"}," +
            //    "{channel_id:\"" + channel_id + "\"}," +
            //    "{$or:[{owner_ext_id:null},{owner_ext_id:\"\"}]}," +
            //    "]}";

        }

    }
}
