using Dapper;
using Firebase.Database.Query;
using Hibaza.CCP.Core;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Data.Providers.Mongo;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public class MongoReferralRepository : IReferralRepository
    {
        public MongoFactory _mongoClient;
        const string referrals = "Referrals";
        IOptions<AppSettings> _appSettings = null;
        Common _cm = new Common();
        public MongoReferralRepository(IOptions<AppSettings> appSettings)
        {
                _mongoClient = new MongoFactory(appSettings);
            _appSettings = appSettings;
        }

        public Referral GetById(string business_id, string id)
        {
            Referral referral = null;
            try
            {
                var key = "Referral_GetById" + business_id + id;

                var options = new FindOptions<Referral, Referral>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
               // options.Sort = Builders<Referral>.Sort.Descending("created_time");
                var query = "{id:\"" + id + "\"}";
                var rs= _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, referrals,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;
            }
            catch { return referral; }
        }

        public int UpdateCustomerId()
        {
            try
            {
                return 1;
                // dùng job
                //System.Threading.Tasks.Task.Factory.StartNew(() =>
                //{
                //    return _mongoClient.excuteProceduceMongoUpsert("Referrals_UpdateCustomerId", null, null, false);
                //});
                //return 1;
            }
            catch { return 0; }
        }


        public void Upsert(Referral referral)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Referral>.Filter.Where(x => x.id == referral.id && x.business_id == referral.business_id);

            _mongoClient.excuteMongoLinqUpdate<Referral>(filter, referral, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, referrals,
true, "", DateTime.Now.AddMinutes(10)).Wait();
            
                CacheBase.cacheModifyAllKeyLinq(new List<string>() { referral.customer_id });

            // save to bot
            try
            {
                if (referral.product_sku != null && referral.product_sku != "")
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        var dicConfig = new Dictionary<string, string>();
                        dicConfig.Add("session_customer", referral.customer_id);
                        dicConfig.Add("using_full_search", "0");
                        dicConfig.Add("page_id", referral.recipient_ext_id);
                        dicConfig.Add("auto_agents", "ManualAgents");
                        dicConfig.Add("business_id", referral.business_id);

                        var dicPara = new Dictionary<string, string>();
                        dicPara.Add("q", referral.product_sku);

                        var json = new Dictionary<string, string>();
                        json.Add("config", JsonConvert.SerializeObject(dicConfig));
                        json.Add("para", JsonConvert.SerializeObject(dicPara));
                        _cm.PostAsync(_appSettings.Value.BaseUrls.ApiAi + "api/AiExcuteAll/Excute", JsonConvert.SerializeObject(json));
                    });
                }
            }
            catch { }
        }


        public bool Delete(string business_id, string id)
        {
            try
            {
                var key = "Referral_Delete"+id;
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                _mongoClient.excuteMongoLinqDelete<Referral>(query,
                   _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, referrals,
                    true, key, DateTime.Now.AddMinutes(10), true);
                CacheBase.cacheModifyAllKeyLinq(new List<string>() {id});
            }
            catch { }
            return true;
        }

        public async Task<IEnumerable<Referral>> GetAll(string business_id, Paging page)
        {
            List<Referral> list =new List<Referral>();
            try
            {
                var key = "Referral_GetAll" + business_id;

                var options = new FindOptions<Referral, Referral>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Referral>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, referrals,
                     true, key, DateTime.Now.AddMinutes(10), true);
            }
            catch { return list; }
        }

        public bool Update(Referral referral)
        {
            throw new NotImplementedException();
        }

        public bool Insert(Referral referral)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Referral>> GetReferrals(string business_id, string thread_id, Paging page)
        {
            List<Referral> list = new List<Referral>();
            try
            {
                var key = "Referral_GetReferrals" + business_id;

                var options = new FindOptions<Referral, Referral>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Referral>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",thread_id:\"" + thread_id + "\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, referrals,
                     true, key, DateTime.Now.AddMinutes(10), true);
            }
            catch { return list; }
        }

        public async Task<IEnumerable<Referral>> GetReferralsByCustomer(string business_id, string customer_id, Paging page)
        {
            List<Referral> list = new List<Referral>();
            try
            {
                var key = "GetReferralsByCustomer" + business_id + customer_id;

                var options = new FindOptions<Referral, Referral>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Referral>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",customer_id:\"" + customer_id + "\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, referrals,
                     true, key, DateTime.Now.AddMinutes(10), true);
            }
            catch { return list; }
        }

        public async Task<List<Referral>> GetReferralsByCustomerIsNull(int limit)
        {
            List<Referral> list = new List<Referral>();
            try
            {
                var key = "GetReferralsByCustomerIsNull";

                var options = new FindOptions<Referral, Referral>();
                options.Projection = "{'_id': 0}";
                options.Limit = limit;
                options.Sort = Builders<Referral>.Sort.Descending("created_time");
                var query = "{customer_id:null}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, referrals,
                     true, key, DateTime.Now.AddMinutes(10), true);
            }
            catch { return list; }
        }
    }
}
