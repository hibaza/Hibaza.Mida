using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Collections;
using Hibaza.CCP.Data.Providers.Mongo;
using MongoDB.Bson.Serialization;
using Hibaza.CCP.Domain.Entities;

namespace Hibaza.CCP.Data
{
    public class CacheBase
    {
        public static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        public static object lockobj = new object();
        public static void cacheManagerAddList(BsonDocument query, BsonDocument update, UpdateOptions options, IMongoCollection<BsonDocument> mongoCollection, string key, DateTime cacheTime, string type)
        {
            try
            {
                //System.Threading.Tasks.Task.Factory.StartNew(() =>
                //{
                lock (lockobj)
                {
                    cacheManagerExcute(key, cacheTime);
                    ConcurrentBag<Dictionary<string, object>> lst = new ConcurrentBag<Dictionary<string, object>>();
                    lst = (ConcurrentBag<Dictionary<string, object>>)_cache.Get(key);
                    if (lst == null)
                        lst = new ConcurrentBag<Dictionary<string, object>>();

                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj.Add("query", query);
                    obj.Add("update", update);
                    obj.Add("options", options);
                    obj.Add("mongocollection", mongoCollection);
                    obj.Add("type", type);
                    lst.Add(obj);

                    _cache.Set(key, lst, cacheTime);
                }
                //});
            }
            catch (Exception ex)
            {
            }
        }

        public static void cacheManagerExcute(string key, DateTime cacheTime)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            try
            {
                var lst = (ConcurrentBag<Dictionary<string, object>>)_cache.Get(key);
                if (lst == null)
                    return;
                lock (lockobj)
                {
                    foreach (var obj in lst)
                    {
                        try
                        {
                            IMongoCollection<BsonDocument> mongoCollection = (IMongoCollection<BsonDocument>)obj["mongocollection"];
                            BsonDocument query = (BsonDocument)obj["query"];
                            BsonDocument update = (BsonDocument)obj["update"];
                            UpdateOptions options = (UpdateOptions)obj["options"];
                            string type = (string)obj["type"];

                            if (type == "upsert")
                            {
                                var rs = mongoCollection.UpdateManyAsync(filter: query, update: update, options: options).Result;
                                if (rs != null && rs.MatchedCount > 0)
                                {
                                    Dictionary<string, object> t = obj;
                                    lst.TryTake(out t);
                                }
                            }

                            if (type == "insert")
                            {
                                var rs = mongoCollection.InsertOneAsync(query).Id;
                                if (rs > 0)
                                {
                                    Dictionary<string, object> t = obj;
                                    lst.TryTake(out t);
                                }
                            }

                            if (type == "delete")
                            {
                                var rs = mongoCollection.DeleteManyAsync(query).Result;
                                if (rs != null && rs.DeletedCount > 0)
                                {
                                    Dictionary<string, object> t = obj;
                                    lst.TryTake(out t);
                                }
                            }
                        }
                        catch (Exception ex)
                        { }
                    }
                    _cache.Set(key, lst, cacheTime);
                }
            }
            catch (Exception ex)
            {
            }
            //});
        }

        public static async Task<T> cacheManagerGet<T>(string key) where T : class
        {
            try
            {
                return null;
                var t = Task<T>.Factory.StartNew(() =>
                 {
                     lock (lockobj)
                     {
                         var objs = new Dictionary<string, object>();
                         _cache.TryGetValue(key, out objs);
                         if (objs != null)
                         {
                             var rs = (T)objs["result"];
                             return rs;
                         }
                         return null;
                     }
                 });
                return await t;
            }
            catch (Exception ex)
            { return null; }
        }

        public static void cacheManagerSet<T>(string key, T result, DateTime cacheTime, IMongoCollection<BsonDocument> mongoCollection,
            FindOptions<BsonDocument> options, string query, bool isModify, List<string> modifyKeys) where T : class
        {
            try
            {
                //System.Threading.Tasks.Task.Factory.StartNew(() =>
                //{
                lock (lockobj)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("key", key);
                    dic.Add("result", result);
                    dic.Add("cacheTime", cacheTime);
                    dic.Add("query", query);
                    dic.Add("options", options);
                    dic.Add("mongoCollection", mongoCollection);
                    dic.Add("modifyKeys", modifyKeys);
                    dic.Add("isModify", isModify);

                    _cache.Set(key, dic, cacheTime);
                }
                //});
                GC.Collect();
            }
            catch (Exception ex)
            { }
        }

        public static void cacheModifyAllKey(List<string> modifyKeys)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            return;
            try
            {
                lock (lockobj)
                {
                    ConcurrentBag<string> caches = new ConcurrentBag<string>();
                    if (_cache == null || _cache.Count == 0)
                        return;
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var entries = _cache.GetType().GetField("_entries", flags).GetValue(_cache);

                    try
                    {
                        IDictionary cacheItems = entries as IDictionary;
                        if (cacheItems != null && cacheItems.Count > 0)
                        {
                            foreach (DictionaryEntry cacheItem in cacheItems)
                            {
                                try
                                {
                                    var key = cacheItem.Key.ToString();
                                    foreach (var modi in modifyKeys)
                                    {
                                        if (key.IndexOf(modi) > 0)
                                        {
                                            caches.Add(key);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    foreach (var key in caches)
                    {
                        try
                        {
                            var dic = new Dictionary<string, object>();
                            _cache.TryGetValue(key, out dic);
                            if (dic.ContainsKey("isModify") && dic.ContainsKey("mongoCollection") && (bool)dic["isModify"])
                            {
                                var mongoCollection = (IMongoCollection<BsonDocument>)dic["mongoCollection"];
                                var options = (FindOptions<BsonDocument>)dic["options"];
                                var query = (string)dic["query"];
                                var cacheTime = (DateTime)dic["cacheTime"];
                                var result = mongoCollection.FindAsync<BsonDocument>(query, options).Result;

                                dic["result"] = result;
                                _cache.Set(key, dic, cacheTime);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            //  });
        }

        //public  void cacheDeleteCacheSame(List<string> modifyKeys)
        //{
        //    System.Threading.Tasks.Task.Factory.StartNew(() =>
        //    {
        //        try
        //        {
        //            lock (lockobj)
        //            {
        //                ConcurrentBag<string> caches = new ConcurrentBag<string>();
        //                if (_cache == null || _cache.Count == 0)
        //                    return;
        //                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        //                var entries = _cache.GetType().GetField("_entries", flags).GetValue(_cache);

        //                try
        //                {

        //                    IDictionary cacheItems = entries as IDictionary;
        //                    if (cacheItems != null && cacheItems.Count > 0)
        //                    {
        //                        foreach (DictionaryEntry cacheItem in cacheItems)
        //                        {
        //                            try
        //                            {
        //                                var key = cacheItem.Key.ToString();
        //                                foreach (var modi in modifyKeys)
        //                                {
        //                                    if (key.IndexOf(modi) > 0)
        //                                    {
        //                                        caches.Add(key);
        //                                    }
        //                                }
        //                            }
        //                            catch (Exception ex)
        //                            { }
        //                        }
        //                    }
        //                }
        //                catch
        //                {
        //                }
        //                foreach (var key in caches)
        //                {
        //                    try
        //                    {
        //                        _cache.Remove(key);
        //                    }
        //                    catch { }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        { }
        //    });

        //}

        public static void cacheManagerSetForProceduce<T>(string key, T result, DateTime cacheTime,
            JsonCommand<BsonDocument> JsonCommand, IMongoDatabase database, bool isModify, string type = "select", bool removeCan = true) where T : class
        {
            try
            {
                //System.Threading.Tasks.Task.Factory.StartNew(() =>
                //{
                lock (lockobj)
                {
                    ConcurrentBag<Dictionary<string, object>> lst = new ConcurrentBag<Dictionary<string, object>>();
                    if (type != "select")
                    {
                        lst = (ConcurrentBag<Dictionary<string, object>>)_cache.Get(key);
                        if (lst == null)
                            lst = new ConcurrentBag<Dictionary<string, object>>();
                    }

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("key", key);
                    dic.Add("result", result);
                    dic.Add("cacheTime", cacheTime);
                    dic.Add("JsonCommand", JsonCommand);
                    dic.Add("database", database);
                    dic.Add("isModify", isModify);
                    dic.Add("type", type);
                    dic.Add("removeCan", removeCan);

                    //_cache.Remove(key);
                    if (type == "select")
                        _cache.Set(key, dic, cacheTime);
                    else
                    {
                        lst.Add(dic);
                        _cache.Set(key, lst, cacheTime);
                    }
                }
                //});
                GC.Collect();
            }
            catch (Exception ex)
            { }
        }


        public static void cacheModifyAllKeyForProcedure(List<string> modifyKeys)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            return;
            try
            {
                if (modifyKeys == null || modifyKeys.Count == 0)
                    return;
                lock (lockobj)
                {
                    ConcurrentBag<string> caches = new ConcurrentBag<string>();
                    if (_cache == null || _cache.Count == 0)
                        return;
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var entries = _cache.GetType().GetField("_entries", flags).GetValue(_cache);

                    try
                    {

                        IDictionary cacheItems = entries as IDictionary;
                        if (cacheItems != null && cacheItems.Count > 0)
                        {
                            foreach (DictionaryEntry cacheItem in cacheItems)
                            {
                                try
                                {
                                    var key = cacheItem.Key.ToString();
                                    foreach (var modi in modifyKeys)
                                    {
                                        if (key.IndexOf(modi) > 0)
                                        {
                                            caches.Add(key);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                    }
                    foreach (var key in caches)
                    {
                        try
                        {
                            var dic = new Dictionary<string, object>();
                            _cache.TryGetValue(key, out dic);
                            var isModify = !dic.ContainsKey("isModify") ? false : (bool)dic["isModify"];
                            var type = !dic.ContainsKey("type") ? "" : (string)dic["type"];
                            var removeCan = !dic.ContainsKey("removeCan") ? true : (bool)dic["removeCan"];
                            if (isModify && type == "select" && dic.ContainsKey("JsonCommand"))
                            {
                                var keyNew = (string)dic["key"];
                                var JsonCommand = (JsonCommand<BsonDocument>)dic["JsonCommand"];
                                var database = (IMongoDatabase)dic["database"];

                                var rs = database.RunCommandAsync<BsonDocument>(JsonCommand).Result;

                                if (rs != null && rs["retval"] != null)
                                {
                                    var cacheTime = (DateTime)dic["cacheTime"];
                                    dic["result"] = rs["retval"].ToString();

                                    // _cache.Remove(key);
                                    _cache.Set(key, dic, cacheTime);
                                }
                            }
                            else
                            {
                                if (key != "cache" && removeCan)
                                    _cache.Remove(key);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            //});

        }

        public static void cacheManagerSetUpsertForProceduce(string key, DateTime cacheTime,
            JsonCommand<BsonDocument> JsonCommand, IMongoDatabase database, string type = "upsert")
        {
            try
            {
                //System.Threading.Tasks.Task.Factory.StartNew(() =>
                //{
                lock (lockobj)
                {
                    ConcurrentBag<Dictionary<string, object>> lst = new ConcurrentBag<Dictionary<string, object>>();
                    lst = (ConcurrentBag<Dictionary<string, object>>)_cache.Get(key);
                    if (lst == null)
                        lst = new ConcurrentBag<Dictionary<string, object>>();

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("key", key);
                    dic.Add("cacheTime", cacheTime);
                    dic.Add("JsonCommand", JsonCommand);
                    dic.Add("database", database);
                    dic.Add("type", type);

                    lst.Add(dic);
                    //_cache.Remove(key);
                    _cache.Set(key, lst, cacheTime);
                }
                // });
                GC.Collect();
            }
            catch (Exception ex)
            { }
        }
        public static async Task<bool> cacheCheckExist(string key)
        {
            var t = Task<bool>.Factory.StartNew(() =>
             {
                 try
                 {
                     if (_cache == null || _cache.Count == 0)
                         return false;
                     var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                     var entries = _cache.GetType().GetField("_entries", flags).GetValue(_cache);
                     IDictionary cacheItems = entries as IDictionary;
                     if (cacheItems != null && cacheItems.Count > 0)
                     {
                         return cacheItems.Contains(key);
                     }
                 }
                 catch { }
                 return false;
             });
            return await t;
        }

        public static void cacheManagerSetForLinq<T>(string key, List<T> result,
            DateTime cacheTime, string query, FindOptions<T, T> options,
            string connectionString, string databaseName, string collectionName,
            bool isModify, string type = "selectlinq", bool removeCan = true) where T : class
        {
            try
            {
                //System.Threading.Tasks.Task.Factory.StartNew(() =>
                //{
                lock (lockobj)
                {
                    ConcurrentBag<Dictionary<string, object>> lst = new ConcurrentBag<Dictionary<string, object>>();
                    if (type != "selectlinq")
                    {
                        lst = (ConcurrentBag<Dictionary<string, object>>)_cache.Get(key);
                        if (lst == null)
                            lst = new ConcurrentBag<Dictionary<string, object>>();
                    }

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("key", key);
                    dic.Add("result", result);
                    dic.Add("cacheTime", cacheTime);
                    dic.Add("query", query);
                    dic.Add("options", options);
                    dic.Add("connectionString", connectionString);
                    dic.Add("databaseName", databaseName);
                    dic.Add("collectionName", collectionName);
                    dic.Add("isModify", isModify);
                    dic.Add("type", type);
                    dic.Add("removeCan", removeCan);

                    //_cache.Remove(key);
                    if (type == "selectlinq")
                        _cache.Set(key, dic, cacheTime);
                    else
                    {
                        lst.Add(dic);
                        _cache.Set(key, lst, cacheTime);
                    }
                }
                //  });
                GC.Collect();
            }
            catch (Exception ex)
            { }
        }

        public static void cacheModifyAllKeyLinq(List<string> modifyKeys)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            return;
            try
            {
                lock (lockobj)
                {
                    ConcurrentBag<string> caches = new ConcurrentBag<string>();
                    if (_cache == null || _cache.Count == 0)
                        return;
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var entries = _cache.GetType().GetField("_entries", flags).GetValue(_cache);

                    try
                    {
                        IDictionary cacheItems = entries as IDictionary;
                        if (cacheItems != null && cacheItems.Count > 0)
                        {
                            foreach (DictionaryEntry cacheItem in cacheItems)
                            {
                                try
                                {
                                    var key = cacheItem.Key.ToString();
                                    foreach (var modi in modifyKeys)
                                    {
                                        if (key.IndexOf(modi) > 0)
                                        {
                                            caches.Add(key);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    foreach (var key in caches)
                    {
                        try
                        {
                            var dic = new Dictionary<string, object>();
                            _cache.TryGetValue(key, out dic);
                            if (dic.ContainsKey("isModify") && dic.ContainsKey("connectionString") && (bool)dic["isModify"] && dic.ContainsKey("query"))
                            {
                                var client = new MongoClient((string)dic["connectionString"]);
                                var db = client.GetDatabase((string)dic["databaseName"]);
                                var collectionName = (string)dic["collectionName"];
                                var query = (string)dic["query"];
                                if (collectionName == "Messages" || collectionName == "MessagesHistory")
                                {
                                    var options = (FindOptions<Message>)dic["options"];
                                    var collection = db.GetCollection<Message>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Message>(query, (FindOptions<Message>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }
                                if (collectionName == "Threads")
                                {
                                    var options = (FindOptions<Thread>)dic["options"];
                                    var collection = db.GetCollection<Thread>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Thread>(query, (FindOptions<Thread>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }
                                if (collectionName == "Customers")
                                {
                                    var options = (FindOptions<Customer>)dic["options"];
                                    var collection = db.GetCollection<Customer>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Customer>(query, (FindOptions<Customer>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }
                                if (collectionName == "Conversations")
                                {
                                    var options = (FindOptions<Conversation>)dic["options"];
                                    var collection = db.GetCollection<Conversation>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Conversation>(query, (FindOptions<Conversation>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }
                                if (collectionName == "Attachments")
                                {
                                    var options = (FindOptions<Attachment>)dic["options"];
                                    var collection = db.GetCollection<Attachment>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Attachment>(query, (FindOptions<Attachment>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }
                                if (collectionName == "Referrals")
                                {
                                    var options = (FindOptions<Referral>)dic["options"];
                                    var collection = db.GetCollection<Referral>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Referral>(query, (FindOptions<Referral>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }
                                if (collectionName == "Channels")
                                {
                                    var options = (FindOptions<Channel>)dic["options"];
                                    var collection = db.GetCollection<Channel>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Channel>(query, (FindOptions<Channel>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }
                                if (collectionName == "Tickets")
                                {
                                    var options = (FindOptions<Ticket>)dic["options"];
                                    var collection = db.GetCollection<Ticket>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Ticket>(query, (FindOptions<Ticket>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }
                                if (collectionName == "Notes")
                                {
                                    var options = (FindOptions<Note>)dic["options"];
                                    var collection = db.GetCollection<Note>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Note>(query, (FindOptions<Note>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }
                                if (collectionName == "Links")
                                {
                                    var options = (FindOptions<Link>)dic["options"];
                                    var collection = db.GetCollection<Link>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Link>(query, (FindOptions<Link>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }
                                if (collectionName == "Businesses")
                                {
                                    var options = (FindOptions<Business>)dic["options"];
                                    var collection = db.GetCollection<Business>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Business>(query, (FindOptions<Business>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }
                                if (collectionName == "Agents")
                                {
                                    var options = (FindOptions<Agent>)dic["options"];
                                    var collection = db.GetCollection<Agent>((string)dic["collectionName"]);
                                    var result = collection.FindAsync<Agent>(query, (FindOptions<Agent>)dic["options"]).Result;
                                    dic["result"] = result.ToList();
                                    _cache.Set(key, dic, (DateTime)dic["cacheTime"]);
                                }

                            }
                            else
                                _cache.Remove(key);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            // });
        }

        public static void cacheManagerSetForLinqUpdate<T>(string key, FilterDefinition<T> filters, T documents, UpdateOptions option,
           DateTime cacheTime,
           string connectionString, string databaseName, string collectionName,
           string type = "updatelinq") where T : class
        {
            try
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    lock (lockobj)
                    {
                        ConcurrentBag<Dictionary<string, object>> lst = new ConcurrentBag<Dictionary<string, object>>();

                        lst = (ConcurrentBag<Dictionary<string, object>>)_cache.Get(key);
                        if (lst == null)
                            lst = new ConcurrentBag<Dictionary<string, object>>();

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("key", key);
                        dic.Add("cacheTime", cacheTime);
                        dic.Add("filters", filters);
                        dic.Add("documents", documents);
                        dic.Add("option", option);
                        dic.Add("connectionString", connectionString);
                        dic.Add("databaseName", databaseName);
                        dic.Add("collectionName", collectionName);
                        dic.Add("type", type);

                        lst.Add(dic);
                        _cache.Set(key, lst, cacheTime);
                    }
                });
                GC.Collect();
            }
            catch (Exception ex)
            { }
        }


        public static void cacheModifyAllKeyLinqUpdate(string key)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    lock (lockobj)
                    {
                        ConcurrentBag<Dictionary<string, object>> lst = new ConcurrentBag<Dictionary<string, object>>();
                        lst = (ConcurrentBag<Dictionary<string, object>>)_cache.Get(key);

                        try
                        {
                            if (lst == null || lst.Count == 0)
                                return;

                            foreach (var dic in lst)
                            {
                                try
                                {
                                    var client = new MongoClient((string)dic["connectionString"]);
                                    var db = client.GetDatabase((string)dic["databaseName"]);
                                    var collectionName = (string)dic["collectionName"];
                                    var cacheTime = (DateTime)dic["cacheTime"];
                                    var option = (UpdateOptions)dic["option"];

                                    if (collectionName.ToLower() == "messages" || collectionName.ToLower() == "messageshistory")
                                    {
                                        Message documents = (Message)dic["documents"];
                                        FilterDefinition<Message> filters = (FilterDefinition<Message>)dic["filters"];
                                        var collection = db.GetCollection<Message>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }

                                    if (collectionName.ToLower() == "threads")
                                    {
                                        Thread documents = (Thread)dic["documents"];
                                        FilterDefinition<Thread> filters = (FilterDefinition<Thread>)dic["filters"];
                                        var collection = db.GetCollection<Thread>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }
                                    if (collectionName.ToLower() == "customers")
                                    {
                                        Customer documents = (Customer)dic["documents"];
                                        FilterDefinition<Customer> filters = (FilterDefinition<Customer>)dic["filters"];
                                        var collection = db.GetCollection<Customer>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }
                                    if (collectionName.ToLower() == "conversations")
                                    {
                                        Conversation documents = (Conversation)dic["documents"];
                                        FilterDefinition<Conversation> filters = (FilterDefinition<Conversation>)dic["filters"];
                                        var collection = db.GetCollection<Conversation>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }
                                    if (collectionName.ToLower() == "attachments")
                                    {
                                        Attachment documents = (Attachment)dic["documents"];
                                        FilterDefinition<Attachment> filters = (FilterDefinition<Attachment>)dic["filters"];
                                        var collection = db.GetCollection<Attachment>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }
                                    if (collectionName.ToLower() == "referrals")
                                    {
                                        Referral documents = (Referral)dic["documents"];
                                        FilterDefinition<Referral> filters = (FilterDefinition<Referral>)dic["filters"];
                                        var collection = db.GetCollection<Referral>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }
                                    if (collectionName.ToLower() == "channels")
                                    {
                                        Channel documents = (Channel)dic["documents"];
                                        FilterDefinition<Channel> filters = (FilterDefinition<Channel>)dic["filters"];
                                        var collection = db.GetCollection<Channel>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }
                                    if (collectionName.ToLower() == "tickets")
                                    {
                                        Ticket documents = (Ticket)dic["documents"];
                                        FilterDefinition<Ticket> filters = (FilterDefinition<Ticket>)dic["filters"];
                                        var collection = db.GetCollection<Ticket>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }
                                    if (collectionName.ToLower() == "notes")
                                    {
                                        Note documents = (Note)dic["documents"];
                                        FilterDefinition<Note> filters = (FilterDefinition<Note>)dic["filters"];
                                        var collection = db.GetCollection<Note>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }
                                    if (collectionName.ToLower() == "links")
                                    {
                                        Link documents = (Link)dic["documents"];
                                        FilterDefinition<Link> filters = (FilterDefinition<Link>)dic["filters"];
                                        var collection = db.GetCollection<Link>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }
                                    if (collectionName.ToLower() == "businesses")
                                    {
                                        Business documents = (Business)dic["documents"];
                                        FilterDefinition<Business> filters = (FilterDefinition<Business>)dic["filters"];
                                        var collection = db.GetCollection<Business>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }
                                    if (collectionName.ToLower() == "agents")
                                    {
                                        Agent documents = (Agent)dic["documents"];
                                        FilterDefinition<Agent> filters = (FilterDefinition<Agent>)dic["filters"];
                                        var collection = db.GetCollection<Agent>((string)dic["collectionName"]);
                                        var rs = collection.ReplaceOne(filters, documents, options: option);
                                        if (rs != null && (rs.MatchedCount > 0 || rs.ModifiedCount > 0 || rs.UpsertedId != null))
                                        {
                                            Dictionary<string, object> t = dic;
                                            lst.TryTake(out t);
                                        }
                                    }
                                    _cache.Set(key, lst, cacheTime);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            });
        }


        public static void cacheBaseSet(string key, object obj, DateTime cacheTime)
        {
            try
            {
                lock (lockobj)
                {
                    _cache.Set(key, obj, cacheTime);
                }
            }
            catch (Exception ex)
            { }
        }
        public static object cacheBaseGet(string key)
        {            
            try
            {
                lock (lockobj)
                {
                    object objs ;
                    _cache.TryGetValue(key, out objs);
                        return objs;
                    
                }
            }
            catch (Exception ex)
            { }
            return null;
        }

    }

}
