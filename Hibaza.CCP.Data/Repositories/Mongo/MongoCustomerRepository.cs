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
using System.Globalization;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Hibaza.CCP.Data.Repositories
{
    public class MongoCustomerRepository : ICustomerRepository
    {
        public MongoFactory _mongoClient;
        IOptions<AppSettings> _appSettings = null;
        const string customers = "Customers";
        public MongoCustomerRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _mongoClient = new MongoFactory(appSettings);
        }

        public async Task<IEnumerable<Counter>> GetChannelCounters(string business_id)
        {
            List<Counter> list = new List<Counter>();
            var key = "GetChannelCounters" + business_id;
            var rs = await CacheBase.cacheManagerGet<List<BsonDocument>>(key);
            if (rs != null && rs.Count > 0)
            {
                return await _mongoClient.DeserializeBsonToEntity<Counter>(rs);
            }

            //var match = "{$match:{$and:[{business_id:\"" + business_id + "\"},{ blocked: false},{$or:[{ unread: true},{$and:[{ unread: false},{ nonreply: true}]}]}]}}";
            var match = "{$match:{business_id:\"" + business_id + "\",blocked: false,$or:[{ unread: true},{ nonreply: true}]}}";
            var group = "{$group: {" +
                                "_id: \"$channel_id\", " +
                                "unreads: {$sum: 1}" +
                                "counts: {$sum: 1}" +
                                "}}";
            var project = "{$project: {" +
                                        "_id: 0, " +
                                        "id: \"$_id\", " +
                                        "unread: \"$unreads\"" +
                                        "count: \"$counts\"" +
                                    "}}";

            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project) };
            var result = await _mongoClient.Customers.AggregateAsync<BsonDocument>(Pipeline);

            var d = result.ToList();
            list = await _mongoClient.DeserializeBsonToEntity<Counter>(d);
            CacheBase.cacheManagerSet(key, d, DateTime.Now.AddMinutes(10),
                _mongoClient.Customers01, null, null, false, new List<string>() { });
            return list;
        }

        public async Task<IEnumerable<Counter>> GetAgentCounters(string business_id)
        {
            List<Counter> list = new List<Counter>();
            var key = "GetAgentCounters" + business_id;
            var rs = await CacheBase.cacheManagerGet<List<BsonDocument>>(key);
            if (rs != null && rs.Count > 0)
            {
                return await _mongoClient.DeserializeBsonToEntity<Counter>(rs);
            }

            //var match = "{$match:{$and:[{business_id:\""+business_id+"\"},{ blocked: false},{$or:[{ unread: true},{$and:[{ unread: false},{ nonreply: true}]}]}]}}";
            var match = "{$match:{business_id:\"" + business_id + "\", blocked: false,$or:[{ unread: true},{ nonreply: true}]}}";
            var group = "{$group: {" +
                                "_id: \"$agent_id\", " +
                                "unread: {$sum: 1}" +
                                "counts: {$sum: 1}" +
                                "}}";
            var project = "{$project: {" +
                                        "_id: 0, " +
                                        "id: \"$_id\", " +
                                        "unread: \"$unread\"" +
                                        "count: \"$counts\"" +
                                    "}}";

            var Pipeline = new[] { BsonDocument.Parse(match), BsonDocument.Parse(group), BsonDocument.Parse(project) };
            var result = await _mongoClient.Customers.AggregateAsync<BsonDocument>(Pipeline);

            var d = result.ToList();
            list = await _mongoClient.DeserializeBsonToEntity<Counter>(d);
            CacheBase.cacheManagerSet(key, d, DateTime.Now.AddMinutes(10),
                _mongoClient.Customers01, null, null, false, new List<string>() { });
            return list;
        }

        public bool UpdateContactInfo(string business_id, string customer_id, CustomerContactInfoModel data)
        {

            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            var option = new UpdateOptions { IsUpsert = false };
            var filter = Builders<Customer>.Filter.Where(x => x.id == customer_id && x.business_id == business_id);
            var update = Builders<Customer>.Update.Set(x => x.name, data.name).
            Set(x => x.phone, data.phone).
            Set(x => x.phone_list, (data.phone_list == null ? new List<string>() : data.phone_list.Distinct().ToList())).
            Set(x => x.city, data.city).
            Set(x => x.address, data.address).
            Set(x => x.birthdate, DateTime.MinValue).
            Set(x => x.email, data.email).
            Set(x => x.blocked, data.blocked).
            Set(x => x.zipcode, data.zipcode).
            Set(x => x.sex, data.sex).
            Set(x => x.weight, data.weight).
            Set(x => x.height, data.height).
            Set(x => x.real_name, data.real_name)
            ;

            _mongoClient.excuteMongoLinqUpdateColumns<Customer>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
true, "", DateTime.Now.AddMinutes(10)).Wait();

            updateCustomerInfo(business_id, customer_id, data).Wait();

            // });
            return true;

        }

        public async Task<bool> UpdatePhoneNumber(string business_id, string customer_id, string phone_list, string phone)
        {
            return true;
            //var t = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //  {
            //            #region check thêm phone đúng đầu số
            //            try
            //            {
            //                var dicConfig = new Dictionary<string, string>();
            //                dicConfig.Add("mongoConnect", "MongoConnect");
            //                dicConfig.Add("mongoDb", "mongoDbAi");
            //                dicConfig.Add("CollectionName", "AddressInfo");
            //                dicConfig.Add("Type", "searchlike");

            //                var para1 = "{\"headphone\":{$regex:/^" + phone.Substring(0, 3) + "/}}";
            //                var projection = "{_id:0}";

            //                var json = new Dictionary<string, string>();
            //                json.Add("config", JsonConvert.SerializeObject(dicConfig));
            //                json.Add("para", para1);
            //                json.Add("projection", projection);
            //                var client = new HttpClient();
            //                var check = client.PostAsync(_appSettings.Value.BaseUrls.ApiAi + "api/MoogExcute",
            //                      new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json")).Result;
            //                var contents = check.Content.ReadAsStringAsync();
            //                if (contents == null || contents.Result == null || contents.Result == "null" || contents.Result == "\"[]\"")
            //                {
            //                    return false;
            //                }
            //            }
            //            catch { }
            //            #endregion

            //            try
            //            {
            //                var option = new UpdateOptions { IsUpsert = false };
            //                var filter = Builders<Customer>.Filter.Where(x => x.id == customer_id && x.business_id == business_id);
            //                var update = Builders<Customer>.Update.Set(x => x.phone_list, (phone_list == null || phone_list == "" ? new List<string>() : phone_list.Split(',').ToList())).
            //                Set(x => x.phone, phone);

            //                _mongoClient.excuteMongoLinqUpdateColumns<Customer>(filter, update, option,
            //_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
            //true, "", DateTime.Now.AddMinutes(10));

            //                CacheBase.cacheModifyAllKeyLinq(new List<string>() { customer_id });

            //                var option1 = new UpdateOptions { IsUpsert = false };
            //                var filter1 = Builders<CustomerContactInfoModel>.Filter.Where(x => x.id == customer_id && x.business_id == business_id);
            //                var update1 = Builders<CustomerContactInfoModel>.Update.Set(x => x.phone_list, (phone_list == null || phone_list == "" ? new List<string>() : phone_list.Split(',').ToList())).
            //                Set(x => x.phone, phone);

            //                _mongoClient.excuteMongoLinqUpdateColumns<CustomerContactInfoModel>(filter1, update1, option1,
            //_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.DatabaseAi, "CustomerInfo",
            //true, "", DateTime.Now.AddMinutes(10));
            //                return true;
            //            }
            //            catch (Exception)
            //            {
            //                return true;
            //            }
            //  });

            // return t.Result;
        }

        public async Task<int> updateCustomerInfo(string business_id, string customer_id, CustomerContactInfoModel data)
        {
            //var t = System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            var option = new UpdateOptions { IsUpsert = false };
            var filter = Builders<CustomerContactInfoModel>.Filter.Where(x => x.id == customer_id && x.business_id == business_id);
            var update = Builders<CustomerContactInfoModel>.Update.Set(p => p.name, data.name).
Set(p => p.phone, data.phone).

Set(p => p.phone_list, (data.phone_list == null ? new List<string>() : data.phone_list.Distinct().ToList())).
Set(p => p.city, data.city).
Set(p => p.address, data.address).
Set(p => p.email, data.email).
Set(p => p.blocked, data.blocked).
Set(p => p.zipcode, data.zipcode).
Set(p => p.sex, data.sex).
Set(p => p.age, data.age).
Set(p => p.weight, data.weight).
Set(p => p.height, data.height).
Set(p => p.updated_time, data.updated_time).
Set(p => p.created_time, data.created_time).
Set(p => p.first_name, data.first_name).
Set(p => p.last_name, data.last_name).
Set(p => p.avatar, data.avatar)
;
            _mongoClient.excuteMongoLinqUpdateColumns<CustomerContactInfoModel>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.DatabaseAi, "CustomerInfo",
true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { customer_id });

            // });
            return 1;

        }
        public bool Block(string business_id, string customer_id, bool blocked)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            var customer = GetById(business_id, customer_id);
            var option = new UpdateOptions { IsUpsert = false };
            var filter = Builders<Customer>.Filter.Where(x => x.id == customer_id && x.business_id == business_id);
            var update = Builders<Customer>.Update.Set(x => x.blocked, blocked);

            _mongoClient.excuteMongoLinqUpdateColumns<Customer>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { customer_id });

            #region add log action

            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            if (customer != null)
            {
                var option1 = new UpdateOptions { IsUpsert = true };
                var filter1 = Builders<Customer>.Filter.Where(x => x.id == customer.id && x.business_id == customer.business_id);

                customer.blocked = blocked;
                _mongoClient.excuteMongoLinqUpdate<Customer>(filter1, customer, option1,
    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, "LogActions",
    true, "", DateTime.Now.AddMinutes(10)).Wait();
            }
            // });
            #endregion
            //});
            return true;
        }

        public bool UpdateUserId(string business_id, int key, string user_id)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            var option = new UpdateOptions { IsUpsert = false };
            var filter = Builders<Customer>.Filter.Where(x => x.business_id == business_id && x.global_id == "" && x.key == key);
            var update = Builders<Customer>.Update.Set(x => x.id, user_id).Set(x => x.global_id, user_id);

            _mongoClient.excuteMongoLinqUpdateColumns<Customer>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { user_id });
            //});
            return true;
        }


        public void Upsert(Customer customer)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Customer>.Filter.Where(x => x.id == customer.id && x.business_id == customer.business_id);

            _mongoClient.excuteMongoLinqUpdate<Customer>(filter, customer, option,
   _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
   true, "", DateTime.Now.AddMinutes(10)).Wait();

            CacheBase.cacheModifyAllKeyLinq(new List<string>() { customer.id });

            #region them update sang CustomerAi
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    CustomerContactInfoModel c = new CustomerContactInfoModel();
                    c.address = customer.address;
                    c.age = customer.age;
                    c.avatar = customer.avatar;
                    c.birthdate = DateTime.MinValue;
                    c.blocked = customer.blocked;
                    c.business_id = customer.business_id;
                    c.city = customer.city;
                    c.created_time = customer.created_time;
                    c.email = customer.email;
                    c.first_name = customer.first_name;
                    c.height = customer.height;
                    c.id = customer.id;
                    c._id = customer.id;
                    c.last_name = customer.last_name;
                    c.name = customer.name;
                    c.phone = customer.phone;
                    c.phone_list = customer.phone_list;
                    c.sex = customer.sex;
                    c.updated_time = customer.updated_time;
                    c.weight = customer.weight;
                    c.zipcode = customer.zipcode;

                    var option1 = new UpdateOptions { IsUpsert = true };
                    var filter1 = Builders<CustomerContactInfoModel>.Filter.Where(x => x.id == customer.id && x.business_id == customer.business_id);

                    _mongoClient.excuteMongoLinqUpdate<CustomerContactInfoModel>(filter1, c, option1,
        _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.DatabaseAi, "CustomerInfo",
        true, "", DateTime.Now.AddMinutes(10));
                }
                catch { }
            });
            #endregion
        }

        public bool Update(Customer customer)
        {
            throw new NotImplementedException();
        }


        public bool Insert(Customer customer)
        {
            throw new NotImplementedException();
        }


        public bool Delete(string business_id, string id)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            try
            {
                var key = "Customer_Delete";
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                _mongoClient.excuteMongoLinqDelete<Customer>(query,
              _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
               true, key, DateTime.Now.AddMinutes(10), true);
                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });

            }
            catch (Exception)
            {
            }
            //});
            return true;
        }

        public async Task<IEnumerable<Customer>> GetActiveUnreadCustomers(string business_id, string agent_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetActiveUnreadCustomers" + business_id + agent_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\"," +
                "agent_id:\"" + agent_id + "\"," +
                "status:\"active\"," +
                "unread:true," +
                "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
             _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
              true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetActiveUnreadCustomers(string business_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetActiveUnreadCustomers" + business_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Customer>.Sort.Ascending("timestamp");
            options.Limit = page.Limit;

            var query = "{business_id:\"" + business_id + "\"," +
                            "status:\"active\"," +
                            "unread:true," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
              _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
               true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetActiveNonReplyCustomers(string business_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetActiveNonReplyCustomers" + business_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Customer>.Sort.Ascending("timestamp");
            options.Limit = page.Limit;

            var query = "{business_id:\"" + business_id + "\"," +
                            "status:\"active\"," +
                            "$or:[{unread:true},{nonreply: true}]," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
              _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
               true, key, DateTime.Now.AddMinutes(10), true);
        }
        public async Task<IEnumerable<Customer>> GetPendingNonReplyCustomers(string business_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetPendingNonReplyCustomers" + business_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Customer>.Sort.Ascending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\",status:\"pending\", blocked: false,$or:[{ unread: true},{ nonreply: true}]}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
            _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
             true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetPendingUnreadCustomers(string business_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetPendingUnreadCustomers" + business_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Customer>.Sort.Ascending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\"," +
                            "status:\"pending\"," +
                            "unread:true," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
            _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
             true, key, DateTime.Now.AddMinutes(10), true);
        }


        public async Task<IEnumerable<Customer>> GetPendingUnreadCustomers(string business_id, string channel_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetPendingUnreadCustomers" + business_id + channel_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\"," +
                            "channel_id:\"" + channel_id + "\"," +
                            "status:\"pending\"," +
                            "unread:true," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
            _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
             true, key, DateTime.Now.AddMinutes(10), true);
        }

        public IEnumerable<Customer> GetAll(string business_id)
        {
            List<Customer> list = new List<Customer>();
            var key = "Customer_GetAll";
            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            var query = "{business_id:\"" + business_id + "\",blocked: false}";

            return _mongoClient.excuteMongoLinqSelect(query, options,
          _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
           true, key, DateTime.Now.AddMinutes(10), true).Result;
        }

        public Customer GetById(string business_id, string id)
        {
            var key = "Custormer_GetById" + business_id + id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = 1;
            //   options.Sort = Builders<BsonDocument>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "id:\"" + id + "\"," +
                            "blocked: false" +
                            "}";


            var rs = _mongoClient.excuteMongoLinqSelect(query, options,
          _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
           true, key, DateTime.Now.AddMinutes(10), true).Result;
            if (rs != null && rs.Count > 0)
                return rs[0];
            return null;
        }


        public async Task<CustomerContactInfoModel> GetCustomerId(string business_id, string id)
        {
            var key = "GetCustomerId" + business_id + id;

            var options = new FindOptions<CustomerContactInfoModel>();
            options.Projection = "{'_id': 0,birthdate:0}";
            options.Limit = 1;
            var query = "{business_id:\"" + business_id + "\"," +
                            "id:\"" + id + "\"," +
                            "blocked: false" +
                            "}";
            var rs = await _mongoClient.excuteMongoLinqSelect<CustomerContactInfoModel>(query, options,
          _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.DatabaseAi, "CustomerInfo",
           true, key, DateTime.Now.AddMinutes(10), true);
            if (rs != null && rs.Count > 0)
                return rs[0];
            return null;
        }

        public async Task<IEnumerable<Customer>> GetCustomers(string business_id, Paging page)
        {
            List<Customer> list = new List<Customer>();

            var key = "GetCustomers" + business_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\"," +
                "timestamp:{$lte:" + page.Next + "}," +
                "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
           _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
            true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> SearchCustomersByKeywords(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "SearchCustomersByKeywords" + business_id + channel_id + agent_id + status + flag + keywords;

            long end;
            if (!long.TryParse(page.Next, out end))
            {
                end = 9999999999999;
            }

            if (keywords != null && keywords != "")
            {
                var options1 = new FindOptions<Customer>();
                options1.Projection = "{_id: 0}";
                options1.Sort = Builders<Customer>.Sort.Descending("updated_time");
                options1.Limit = 50;

                var query1 = "{$and:[{$text: { $search: \"\\\"" + keywords + "\\\"\"}},{blocked:false},{business_id:\"" + business_id + "\"}]}";
                list = await _mongoClient.excuteMongoLinqSelect(query1, options1,
        _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
         true, key, DateTime.Now.AddMinutes(10), true);
                if (list == null || list.Count == 0)
                {
                    query1 = "{$and:[{$text: { $search: \"" + keywords + "\"}},{blocked:false},{business_id:\"" + business_id + "\"}]}";
                    list = await _mongoClient.excuteMongoLinqSelect(query1, options1,
            _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
             true, key, DateTime.Now.AddMinutes(10), true);
                }

            }
            else
            {

                var options = new FindOptions<Customer>();
                options.Projection = "{'_id': 0}";
                options.Sort = Builders<Customer>.Sort.Descending("timestamp");
                options.Limit = page.Limit;
                var query = "{business_id:\"" + business_id + "\",";
                if (flag == "unread")
                    query += "unread:true,";
                if (flag == "nonreply")
                    query += "$or:[{unread:true},{nonreply:true}],";
                if (flag == "open")
                {
                    query += "open:true,";
                }

                if (agent_id != null && agent_id != "")
                    query += "agent_id:\"" + agent_id + "\",";
                query += "timestamp:{$lte:" + page.Next + "},";
                query += "blocked:false";
                query += "}";
                list = await _mongoClient.excuteMongoLinqSelect(query, options,
         _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
          true, key, DateTime.Now.AddMinutes(10), true);

            }
            #region them truong hop  neu dang click vao 1 kh thi set nguoi nay van hoat dong
            if (!string.IsNullOrWhiteSpace(agent_id))
            {
                var option = new UpdateOptions { IsUpsert = false };
                var filter = Builders<Agent>.Filter.Where(x => x.id == agent_id);
                var update = Builders<Agent>.Update.Set(x => x.last_acted_time, DateTime.Now);

                _mongoClient.excuteMongoLinqUpdateColumns<Agent>(filter, update, option,
    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, "Agents",
    true, "", DateTime.Now.AddMinutes(10));
            }
            #endregion

            return list;
        }

        public async Task<IEnumerable<Customer>> GetUnreadCustomers(string business_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetUnreadCustomers" + business_id;

            var options = new FindOptions<Customer>();
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            var query = "{business_id:\"" + business_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "unread:true," +
                            "blocked: false" +
            "}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
       _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
        true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetUnreadCustomersByChannel(string business_id, string channel_id, Paging page)
        {
            List<Customer> list = new List<Customer>();

            var key = "GetUnreadCustomersByChannel" + business_id + channel_id;
            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "channel_id:\"" + channel_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "unread:true," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
        _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
         true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetUnreadCustomersByAgent(string business_id, string agent_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetUnreadCustomersByAgent" + business_id + agent_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "agent_id:\"" + agent_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "unread:true," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
           _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
            true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetUnreadCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetUnreadCustomersByChannelAndAgent" + business_id + channel_id + agent_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "channel_id:\"" + channel_id + "\"," +
                            "agent_id:\"" + agent_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "unread:true," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
          _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
           true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetCustomersWhereExtIdIsNull(string business_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetCustomersWhereExtIdIsNull" + business_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{$and:[{business_id:\"" + business_id + "\"}," +
                            "{timestamp:{$lte:" + page.Next + "}}," +
                            "{$or:[{ext_id:null},{ext_id:\"\"}]}," +
                            "{blocked: false}" +
                            "]}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
         _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
          true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetNonReplyCustomers(string business_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetNonReplyCustomers" + business_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            //var query = "{$and:[{business_id:\"" + business_id + "\"},{timestamp:{$lte:" + page.Next + "}},{ blocked: false},{$or:[{ unread: true},{$and:[{ unread: false},{ nonreply: true}]}]}]}";
            var query = "{business_id:\"" + business_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "$or:[{unread:true},{nonreply:true}]," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
        _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
         true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetNonReplyCustomersByChannel(string business_id, string channel_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetNonReplyCustomersByChannel" + business_id + channel_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            //var query = "{$and:[{business_id:\"" + business_id + "\"},{channel_id:\"" + channel_id + "\"},{timestamp:{$lte:" + page.Next + "}},{ blocked: false},{$or:[{ unread: true},{$and:[{ unread: false},{ nonreply: true}]}]}]}";
            var query = "{business_id:\"" + business_id + "\"," +
                            "channel_id:\"" + channel_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "$or:[{unread:true},{nonreply:true}]," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
       _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
        true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetNonReplyCustomersByAgent(string business_id, string agent_id, Paging page)
        {
            List<Customer> list = new List<Customer>();

            var key = "GetNonReplyCustomersByAgent" + business_id + agent_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            //var query = "{$and:[{business_id:\"" + business_id + "\"},{agent_id:\"" + agent_id + "\"},{timestamp:{$lte:" + page.Next + "}},{ blocked: false},{$or:[{ unread: true},{$and:[{ unread: false},{ nonreply: true}]}]}]}";
            var query = "{business_id:\"" + business_id + "\"," +
                            "agent_id:\"" + agent_id + "\"," +
                             "timestamp:{$lte:" + page.Next + "}," +
                             "blocked: false," +
                            "$or:[{unread:true},{nonreply:true}]" +
                            "}";

            return await _mongoClient.excuteMongoLinqSelect(query, options,
     _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
      true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetNonReplyCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetNonReplyCustomersByChannelAndAgent" + business_id + channel_id + agent_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            //var query = "{$and:[{business_id:\"" + business_id + "\"},{channel_id:\"" + channel_id + "\",{ agent_id:\"" + agent_id + "\"},{timestamp:{$lte:" + page.Next + "}},{ blocked: false},{$or:[{ unread: true},{$and:[{ unread: false},{ nonreply: true}]}]}]}";
            var query = "{business_id:\"" + business_id + "\"," +
                            "channel_id:\"" + channel_id + "\"," +
                            "agent_id:\"" + agent_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "$or:[{unread:true},{nonreply:true}]," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
     true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers(string business_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetAllCustomers" + business_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
   _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
    true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetOpenCustomers(string business_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetOpenCustomers" + business_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "open:true," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
 _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
  true, key, DateTime.Now.AddMinutes(10), true);
        }
        public async Task<IEnumerable<Customer>> GetAllCustomersByChannel(string business_id, string channel_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetAllCustomersByChannel" + business_id + channel_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "channel_id:\"" + channel_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetOpenCustomersByChannel(string business_id, string channel_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetOpenCustomersByChannel" + business_id + channel_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "channel_id:\"" + channel_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "open:true," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersByAgent(string business_id, string agent_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetAllCustomersByAgent" + business_id + agent_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "agent_id:\"" + agent_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetOpenCustomersByAgent(string business_id, string agent_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetOpenCustomersByAgent" + business_id + agent_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "agent_id:\"" + agent_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "open:true," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
true, key, DateTime.Now.AddMinutes(10), true);
        }
        public async Task<IEnumerable<Customer>> GetAllCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetAllCustomersByChannelAndAgent" + business_id + channel_id + agent_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "channel_id:\"" + channel_id + "\"," +
                            "agent_id:\"" + agent_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetOpenCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetOpenCustomersByChannelAndAgent" + business_id + channel_id + agent_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                            "channel_id:\"" + channel_id + "\"," +
                            "agent_id:\"" + agent_id + "\"," +
                            "timestamp:{$lte:" + page.Next + "}," +
                            "open:true," +
                            "blocked: false" +
                            "}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<IEnumerable<Customer>> GetCustomerActiveThread(string business_id)
        {
            List<Customer> list = new List<Customer>();
            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = 100;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\",active_thread:null}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
false, "", DateTime.Now.AddMinutes(10), false);
        }

        public async Task<List<Customer>> GetCustomersActiveThreadLikeThread(string business_id, string thread_id, Paging page)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetCustomersActiveThreadLikeThread" + thread_id;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = page.Limit;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id =\"" + business_id + "\", active_thread:{$regex:\"" + thread_id + "\"}}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<List<Customer>> GetCustomersAppId(string business_id, string appId)
        {
            List<Customer> list = new List<Customer>();
            var key = "GetCustomersAppId" + appId;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = 1;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{$and:[{business_id:\"" + business_id + "\"},{$or:[{id:\"" + appId + "\"},{ app_id:\"" + appId + "\"}]}]}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<List<Customer>> GetCustomerFromPhone(string business_id, string phone)
        {
            List<Customer> list = new List<Customer>();

            var key = "GetCustomerFromPhone" + business_id + phone;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = 1;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{business_id:\"" + business_id + "\"," +
                "phone_list:\"" + phone + "\"}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
           _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
            true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<List<Customer>> GetCustomerFromPhone(string phone)
        {
            List<Customer> list = new List<Customer>();

            var key = "GetCustomerFromPhone01" + phone;

            var options = new FindOptions<Customer>();
            options.Projection = "{'_id': 0}";
            options.Limit = 1;
            options.Sort = Builders<Customer>.Sort.Descending("timestamp");
            var query = "{phone_list:\"" + phone + "\"}";
            return await _mongoClient.excuteMongoLinqSelect(query, options,
           _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
            true, key, DateTime.Now.AddMinutes(10), true);
        }

        public async Task<bool> UpdateRealName(string business_id, string customer_id, string real_name)
        {
            try
            {
                var option = new UpdateOptions { IsUpsert = false };
                var filter = Builders<Customer>.Filter.Where(x => x.id == customer_id && x.business_id == business_id);
                var update = Builders<Customer>.Update.Set(x => x.real_name, real_name);

                await _mongoClient.excuteMongoLinqUpdateColumns<Customer>(filter, update, option,
      _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, customers,
      true, "", DateTime.Now.AddMinutes(10));
                return true;
            }
            catch { return false; }
        }
    }
}
