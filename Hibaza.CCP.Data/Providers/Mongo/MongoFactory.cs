using Dapper;
using Hibaza.CCP.Core;
using Hibaza.CCP.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hibaza.CCP.Data;
using System.IO;
using MongoDB.Driver.Core.Bindings;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq.Expressions;
using Hibaza.CCP.Domain.Models;

namespace Hibaza.CCP.Data.Providers.Mongo
{

    public class MongoFactory
    {

        public IMongoDatabase _database;
        public IMongoDatabase _databaseAi;
        public IOptions<AppSettings> _appSettings;
        public Common _cm = new Common();
        public CacheBase _cb = new CacheBase();

        public MongoFactory(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            
            var client = getMongoClient(_appSettings.Value.MongoDB.ConnectionString);
            _database = client.GetDatabase(_appSettings.Value.MongoDB.Database);
            _databaseAi = client.GetDatabase(_appSettings.Value.MongoDB.DatabaseAi);
        }

        public MongoClient getMongoClient(string connectionString) {
           var obj= CacheBase.cacheBaseGet(connectionString);
            if (obj != null)
            {
                return (MongoClient)obj;
            }
            else
            {
                var client = new MongoClient(connectionString);
                CacheBase.cacheBaseSet(connectionString, client, DateTime.Now.AddYears(1));
                return client;
            }
        }

        public IMongoCollection<Message> Messages
        {
            get
            {
                return _database.GetCollection<Message>("Messages");
            }
        }
        public IMongoCollection<Message> MessagesHistory
        {
            get
            {
                return _database.GetCollection<Message>("MessagesHistory");
            }
        }

        public IMongoCollection<Thread> Threads
        {
            get
            {
                return _database.GetCollection<Thread>("Threads");
            }
        }
        public IMongoCollection<Customer> Customers
        {
            get
            {
                return _database.GetCollection<Customer>("Customers");
            }
        }
        public IMongoCollection<BsonDocument> Customers01
        {
            get
            {
                return _database.GetCollection<BsonDocument>("Customers");
            }
        }
        public IMongoCollection<Conversation> Conversations
        {
            get
            {
                return _database.GetCollection<Conversation>("Conversations");
            }
        }
        public IMongoCollection<Attachment> Attachments
        {
            get
            {
                return _database.GetCollection<Attachment>("Attachments");
            }
        }
        public IMongoCollection<BsonDocument> Logs
        {
            get
            {
                return _database.GetCollection<BsonDocument>("Logs");
            }
        }
        public IMongoCollection<BsonDocument> CustomersInfo
        {
            get
            {
                return _databaseAi.GetCollection<BsonDocument>("CustomerInfo");
            }
        }
        public IMongoCollection<CustomerContactInfoModel> CustomersInfo01
        {
            get
            {
                return _databaseAi.GetCollection<CustomerContactInfoModel>("CustomerInfo");
            }
        }

        public IMongoCollection<BsonDocument> AddressInfo
        {
            get
            {
                return _databaseAi.GetCollection<BsonDocument>("AddressInfo");
            }
        }

        public IMongoCollection<Referral> Referrals
        {
            get
            {
                return _database.GetCollection<Referral>("Referrals");
            }
        }
        public IMongoCollection<Channel> Channels
        {
            get
            {
                return _database.GetCollection<Channel>("Channels");
            }
        }

        public IMongoCollection<Ticket> Tickets
        {
            get
            {
                return _database.GetCollection<Ticket>("Tickets");
            }
        }

        public IMongoCollection<Note> Notes
        {
            get
            {
                return _database.GetCollection<Note>("Notes");
            }
        }
        public IMongoCollection<Shortcut> Shortcuts
        {
            get
            {
                return _database.GetCollection<Shortcut>("Shortcuts");
            }
        }
        public IMongoCollection<Node> Nodes
        {
            get
            {
                return _database.GetCollection<Node>("Nodes");
            }
        }
        public IMongoCollection<Link> Links
        {
            get
            {
                return _database.GetCollection<Link>("Links");
            }
        }
        public IMongoCollection<Business> Businesses
        {
            get
            {
                return _database.GetCollection<Business>("Businesses");
            }
        }
        public IMongoCollection<Agent> Agents
        {
            get
            {
                return _database.GetCollection<Agent>("Agents");
            }
        }
        public IMongoCollection<BsonDocument> Reports
        {
            get
            {
                return _databaseAi.GetCollection<BsonDocument>("Reports");
            }
        }

        public async Task<List<T>> DeserializeBsonToEntity<T>(List<BsonDocument> data) where T : class
        {
            try
            {
                var t = Task<List<T>>.Factory.StartNew(() =>
                {
                    List<T> lst = new List<T>();
                    foreach (var val in data)
                    {
                        try
                        {
                            var dynamic = BsonSerializer.Deserialize<dynamic>(val);
                            string str = JsonConvert.SerializeObject(dynamic);
                            var json = JsonConvert.DeserializeObject(str).ToString();
                            lst.Add(JsonConvert.DeserializeObject<T>(json));
                        }
                        catch { }
                    }
                    return lst;
                });
                return await t;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public async Task<List<T>> ConvertGroupToEntity<T>(List<BsonDocument> data)
        {
            try
            {
                var t = Task<List<T>>.Factory.StartNew(() =>
                {
                    List<T> lst = new List<T>();
                    foreach (var val in data)
                    {
                        try
                        {
                            var str = JsonConvert.SerializeObject(val.ToString());
                            var json = JsonConvert.DeserializeObject(str.Replace("false", "0").Replace("true", "1")).ToString();
                            lst.Add(JsonConvert.DeserializeObject<T>(json));
                        }
                        catch { }
                    }
                    return lst;
                });
                return await t;
            }
            catch (Exception ex) { return null; }
        }

        public void deleteTemp(BsonDocument document, string collectionName)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            #region check xem co can xoa bang tam
            try
            {
                var collectionConn = _database.GetCollection<BsonDocument>(collectionName);
                var match = "{$match:{thread_id:\"" + document["thread_id"] + "\"}}";
                var group = "{$group:{_id:{thread_id:\"$thread_id\"}, " +
                                    "count:{$sum:1}}}";
                var project = "{$project:{_id:0}}";
                var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project) };
                var result = collectionConn.AggregateAsync<BsonDocument>(Pipeline).Result.ToList();

                if (result != null && result.Count > 0 && (int)result[0]["count"] > 20)
                {
                    var options = new FindOptions<BsonDocument>();
                    options.Sort = Builders<BsonDocument>.Sort.Ascending("timestamp");
                    options.Limit = (int)result[0]["count"] - 20;
                    var queryMg = "{thread_id:\"" + document["thread_id"] + "\"}";
                    var threadDelete = collectionConn.FindAsync<BsonDocument>(filter: queryMg, options: options).Result.ToList();
                    if (threadDelete != null && threadDelete.Count > 0)
                        foreach (var val in threadDelete)
                        {
                            try
                            {
                                collectionConn.DeleteOneAsync(BsonDocument.Parse("{_id:\"" + val["_id"] + "\"}"));
                            }
                            catch { }
                        }
                }
            }
            catch (Exception ex) { }
            #endregion
            //  });
        }
        public async Task<List<T>> excuteProceduceMongoSelect<T>(string proceduce, Dictionary<string, object> paras
            , bool isCache, string key, DateTime cacheTime, bool isModify, string type = "select", IMongoDatabase database = null
            ) where T : class
        {
            try
            {
                isCache = false;
                if (type == "select")
                {
                    //var cache = CacheBase.cacheManagerGet<string>(key).Result;
                    //if (cache != null)
                    //{
                    //    return _cm.DeserializeBsonarrayToEntity<T>(cache).Result;
                    //}
                }
                if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
                {
                    BsonClassMap.RegisterClassMap<T>(cm =>
                    {
                        cm.AutoMap();
                        cm.SetIgnoreExtraElements(true);
                        cm.SetIsRootClass(true);
                    });
                }
               var client= getMongoClient(_appSettings.Value.MongoDB.ConnectionString);
                // gọi tới thủ tục
                if (database == null)
                {
                    database = client.GetDatabase(_appSettings.Value.MongoDB.Database);
                }
                else
                {
                    database = client.GetDatabase(_appSettings.Value.MongoDB.DatabaseAi);
                }
                var cmd = await _cm.formatCommandProcedure(proceduce, paras);
                var rs = await database.RunCommandAsync<BsonDocument>(cmd);

                List<T> list = null;
                if (type == "select")
                {
                    list = await convertBsonToEntity<T>(rs, cmd, isCache, key, cacheTime, isModify, type, database);
                }
                return list;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<List<T>> convertBsonToEntity<T>(BsonDocument rs, JsonCommand<BsonDocument> cmd, bool isCache, string key,
            DateTime cacheTime, bool isModify, string type = "select", IMongoDatabase database = null) where T : class
        {
            var d = rs["retval"].ToString();
            var t = Task<List<T>>.Factory.StartNew(() =>
            {
                var list = _cm.DeserializeBsonarrayToEntity<T>(d).Result;
                if (isCache)
                {
                    if (database == null)
                        database = _database;
                   // CacheBase.cacheManagerSetForProceduce(key, d, cacheTime, cmd, database, isModify);
                }
                return list;
            });
            return await t;
        }

        public void excuteProceduceMongoUpsert(string proceduce, Dictionary<string, object> paras, List<string> modifyKeys, bool isCache = true, string key = "upsertproduct", string type = "upsert", IMongoDatabase database = null
            )
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            if (database == null)
                database = _database;

            var q = "";
            if (paras != null && paras.Count > 0)
                foreach (var para in paras)
                {
                    q += "'" + para.Value + "',";
                }

            var cmd = new JsonCommand<BsonDocument>("{ eval: \"" + proceduce + "(" + (q == "" ? q : q.Substring(0, q.Length - 1)) + ")\"}");

            try
            {
                var rs = database.RunCommandAsync<BsonDocument>(cmd).Result;
                var d = rs["retval"].ToString();
                if (d.IndexOf("false") > 0)
                {
                   // CacheBase.cacheManagerSetUpsertForProceduce(key, DateTime.Now.AddHours(1), cmd, database, type);
                    return;
                }
               // CacheBase.cacheModifyAllKeyForProcedure(modifyKeys);
            }
            catch (Exception ex)
            {
               // CacheBase.cacheManagerSetUpsertForProceduce(key, DateTime.Now.AddHours(1), cmd, database, type);
            }
            // });
        }

        public async Task<List<T>> excuteMongoLinqSelect<T>(string query, FindOptions<T, T> options,
            string connectionString, string databaseName, string collectionName,
            bool isCache, string key, DateTime cacheTime, bool isModify, string type = "selectlinq"
           ) where T : class
        {
            try
            {
                isCache = false;
                var list = new List<T>();
                //if (type == "selectlinq")
                //{
                //    var cache = CacheBase.cacheManagerGet<List<T>>(key).Result;
                //    if (cache != null)
                //    {
                //        return cache;
                //    }
                //}
                var client = getMongoClient(connectionString);
                var db = client.GetDatabase(databaseName);
                var collection = db.GetCollection<T>(collectionName);

                var rs = await collection.FindAsync<T>(query, options);
                list = rs.ToList();
                //CacheBase.cacheManagerSetForLinq(key, list, cacheTime, query, options,
                //    connectionString, databaseName, collectionName, isModify, type);
                GC.Collect();
                return list;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<DeleteResult> excuteMongoLinqDelete<T>(string query,
            string connectionString, string databaseName, string collectionName,
            bool isCache, string key, DateTime cacheTime, bool isModify, string type = "deletelinq"
           ) where T : class
        {
            try
            {
                var list = new List<T>();
                var client = getMongoClient(connectionString);
                var db = client.GetDatabase(databaseName);//_appSettings.Value.MongoDB.Database
                var collection = db.GetCollection<T>(collectionName);

                var rs = await collection.DeleteManyAsync(query);
                return rs;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<ReplaceOneResult> excuteMongoLinqUpdate<T>(FilterDefinition<T> filters, T documents, UpdateOptions option,
            string connectionString, string databaseName, string collectionName,
            bool isCache, string key, DateTime cacheTime, string type = "updatelinq"
           ) where T : class
        {
            try
            {
                key = "updatelinq";
                //CacheBase.cacheModifyAllKeyLinqUpdate(key);

                var client = getMongoClient(connectionString);
                var db = client.GetDatabase(databaseName);
                var collection = db.GetCollection<T>(collectionName);

                try
                {
                    var rs = await collection.ReplaceOneAsync(filters, documents, option);

                    // var rs = await collection.UpdateManyAsync(filter: filters, update: documents, options: option);
                    if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null)) ;
                    else
                    {
                       // CacheBase.cacheManagerSetForLinqUpdate<T>(key, filters, documents, option, cacheTime, connectionString, databaseName, collectionName, type);
                    }
                    return rs;
                }
                catch (Exception ex)
                {
                 //   CacheBase.cacheManagerSetForLinqUpdate<T>(key, filters, documents, option, cacheTime, connectionString, databaseName, collectionName, type);
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<int> excuteMongoInsert<T>(T documents, InsertOneOptions option,
            string connectionString, string databaseName, string collectionName,
            bool isCache, string key, DateTime cacheTime, string type = "insertlinq"
           ) where T : class
        {
            try
            {
                key = "insertlinq";
                var client = getMongoClient(connectionString);
                var db = client.GetDatabase(databaseName);
                var collection = db.GetCollection<T>(collectionName);

                var rs = collection.InsertOneAsync(documents, option);
                return 1;
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        public async Task<UpdateResult> excuteMongoLinqUpdateColumns<T>(FilterDefinition<T> filters, UpdateDefinition<T> documents, UpdateOptions option,
          string connectionString, string databaseName, string collectionName,
          bool isCache, string key, DateTime cacheTime, string type = "updatelinqcolumns"
         ) where T : class
        {
            try
            {
                key = "updatelinqcolumns";
                var client = getMongoClient(connectionString);
                var db = client.GetDatabase(databaseName);
                var collection = db.GetCollection<T>(collectionName);

                try
                {
                    var rs = await collection.UpdateOneAsync(filters, documents, option);

                    // var rs = await collection.UpdateManyAsync(filter: filters, update: documents, options: option);

                    return rs;
                }
                catch (Exception ex)
                {
                    // CacheBase.cacheManagerSetForLinqUpdate<T>(key, filters, documents, option, cacheTime, connectionString, databaseName, collectionName, type);
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}