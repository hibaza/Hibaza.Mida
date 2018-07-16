using Dapper;
using Hibaza.CCP.Core;
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
    public class MongoLinkRepository : ILinkRepository
    {
        public MongoFactory _mongoClient;
        IOptions<AppSettings> _appSettings = null;
        const string links = "Links";
        public MongoLinkRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }
        public async Task<IEnumerable<Link>> GetLinks(string business_id, string channel_id, Paging page)
        {
            var list = new List<Link>();
            try
            {
                var key = "Links_GetLinks" + business_id + channel_id;

                var options = new FindOptions<Link, Link>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Link>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",channel_id:\"" + channel_id + "\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, links,
                     true, key, DateTime.Now.AddMinutes(10), true);
                //para.Add("since", long.Parse(page.Previous ?? "0"));
                //para.Add("until", long.Parse(page.Next ?? "9999999999"));
                
            }
            catch { return list; }
        }


        public bool UpdateTimestamp(string business_id, string id, long timestamp)
        {
            try
            {
                var option = new UpdateOptions { IsUpsert = false };
                var filter = Builders<Link>.Filter.Where(x => x.id == id && x.business_id == business_id);
                var update = Builders<Link>.Update.Set(x => x.timestamp , timestamp);

                _mongoClient.excuteMongoLinqUpdateColumns<Link>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, links,
true, "", DateTime.Now.AddMinutes(10)).Wait();

                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
                return true;
            }
            catch { return false; }
        }

        public bool UpdateStatus(string business_id, string id, string status)
        {
            try
            {
                var option = new UpdateOptions { IsUpsert = false };
                var filter = Builders<Link>.Filter.Where(x => x.id == id && x.business_id == business_id);
                var update = Builders<Link>.Update.Set(x => x.status , status ).Set(x=>x.updated_time, DateTime.Now);

                _mongoClient.excuteMongoLinqUpdateColumns<Link>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, links,
true, "", DateTime.Now.AddMinutes(10)).Wait();

                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
                return true;
            }
            catch { return false; }
        }

        public void CreateLink(Link link)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Link>.Filter.Where(x => x.id == link.id && x.business_id == link.business_id);

            _mongoClient.excuteMongoLinqUpdate<Link>(filter, link, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, links,
true, "", DateTime.Now.AddMinutes(10)).Wait();
        }

        public bool Update(Link link)
        {
            throw new NotImplementedException();
        }

        public Link GetById(string business_id, string id)
        {
            try
            {
                var key = "Links_GetById" + business_id + id;

                var options = new FindOptions<Link, Link>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                options.Sort = Builders<Link>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                var rs= _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, links,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;

                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;
            }
            catch { return null; }
        }


        public IEnumerable<Link> GetAll()
        {
            var list = new List<Link>();
            try
            {
                var key = "Links_GetAll";

                var options = new FindOptions<Link, Link>();
                options.Projection = "{'_id': 0}";
                options.Limit = 10;
                options.Sort = Builders<Link>.Sort.Descending("created_time");
                var query = "{}";
                return _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, links,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;                
            }
            catch { return list; }
        }

        public bool Insert(Link link)
        {
            throw new NotImplementedException();
        }


        public bool Delete(string key)
        {
            throw new NotImplementedException();
        }
    }
}
