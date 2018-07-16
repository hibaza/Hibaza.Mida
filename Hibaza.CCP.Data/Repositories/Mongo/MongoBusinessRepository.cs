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
    public class MongoBusinessRepository : IBusinessRepository
    {
        public MongoFactory _mongoClient ;
        const string businesses = "Businesses";
        IOptions<AppSettings> _appSettings = null;
        public MongoBusinessRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }

        public Business GetById(string id)
        {
            try
            {
                var key = "Businesses_GetById" + id;

                var options = new FindOptions<Business, Business>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
               // options.Sort = Builders<Business>.Sort.Descending("created_time");
                var query = "{id:\""+ id + "\"}";
                var rs = _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, businesses,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;
            }
            catch { return null; }
        }

        public Business GetByEmail(string email)
        {
            try
            {
                var key = "Businesses_GetByEmail" + email;

                var options = new FindOptions<Business, Business>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                // options.Sort = Builders<Business>.Sort.Descending("created_time");
                var query = "{email:\"" + email + "\"}";
                var rs = _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, businesses,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;
            }
            catch { return null; }
        }


        public IEnumerable<Business> GetAll()
        {
            throw new NotImplementedException();
        }


        private bool Insert(Business business)
        {
            throw new NotImplementedException();
        }

        private bool Update(Business business)
        {
            throw new NotImplementedException();
        }

        public void Upsert(Business business)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Business>.Filter.Where(x => x.id == business.id );

            _mongoClient.excuteMongoLinqUpdate<Business>(filter, business, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, businesses,
true, "", DateTime.Now.AddMinutes(10)).Wait();
        }

        public bool Delete(string id)
        {
            try
            {
                var key = "Business_Delete";
                var query = "{id:\"" + id + "\"}";
                _mongoClient.excuteMongoLinqDelete<Business>(query,
                   _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, businesses,
                    true, key, DateTime.Now.AddMinutes(10), true);
                return true;
            }
            catch { return false; }
        }


        public IEnumerable<Business> GetBusinesses(Paging page)
        {
            var list = new List<Business>();
            try
            {
                var key = "GetBusinesses";

                var options = new FindOptions<Business, Business>();
                options.Projection = "{'_id': 0}";
                options.Limit = 10;
                options.Sort = Builders<Business>.Sort.Descending("created_time");
                var query = "{}";
                return _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, businesses,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
            }
            catch { return list; }
        }

        public async Task<Business> GetBusinessFromTokenClient(string token_client)
        {
            try
            {
                var key = "GetBusinessFromTokenClient" + token_client;
                var options = new FindOptions<Business, Business>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                var query = "{token_client:\"" + token_client + "\"}";
                var rs = await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, businesses,
                     true, key, DateTime.Now.AddMinutes(10), true);
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;
            }
            catch { return null; }
        }
    }
}
