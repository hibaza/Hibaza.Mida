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
    public class MongoShortcutRepository : IShortcutRepository
    {
        public MongoFactory _mongoClient;
        const string shortcuts = "Shortcuts";
        IOptions<AppSettings> _appSettings = null;
        public MongoShortcutRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }

        public Shortcut GetById(string business_id, string id)
        {
            try
            {
                var key = "Shortcuts_GetById" + business_id + id;

                var options = new FindOptions<Shortcut, Shortcut>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                //options.Sort = Builders<Shortcut>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                var rs= _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, shortcuts,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;                
            }
            catch { return null; }
        }


        public void Upsert(Shortcut shortcut)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Shortcut>.Filter.Where(x => x.id == shortcut.id && x.business_id == shortcut.business_id);

            _mongoClient.excuteMongoLinqUpdate<Shortcut>(filter, shortcut, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, shortcuts,
true, "", DateTime.Now.AddMinutes(10)).Wait();
            
                CacheBase.cacheModifyAllKeyLinq(new List<string>() { shortcut.id});
        }

        public bool Delete(string business_id, string id)
        {
            try
            {
                var key = "Shortcut_Delete";
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                _mongoClient.excuteMongoLinqDelete<Channel>(query,
                   _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, shortcuts,
                    true, key, DateTime.Now.AddMinutes(10), true);
                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
                return true;
            }
            catch { return false; }
        }

        public async Task<IEnumerable<Shortcut>> GetAll(string business_id, Paging page)
        {
            var list = new List<Shortcut>();
            try
            {
                var key = "Shortcuts_GetAll" + business_id;

                var options = new FindOptions<Shortcut, Shortcut>();
                options.Projection = "{'_id': 0,'created_time':0,'updated_time':0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Shortcut>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, shortcuts,
                     true, key, DateTime.Now.AddMinutes(10), true);                
            }
            catch { return list; }
        }

        public async Task<List<Shortcut>> GetByAgent(string business_id,string agent_id)
        {
            var list = new List<Shortcut>();
            try
            {
                var key = "GetByAgent" + business_id;

                var options = new FindOptions<Shortcut, Shortcut>();
                options.Projection = "{'_id': 0,'created_time':0,'updated_time':0}";
                //options.Limit = 1;
                options.Sort = Builders<Shortcut>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",created_by:\"" + agent_id + "\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, shortcuts,
                     true, key, DateTime.Now.AddMinutes(10), true);
            }
            catch { return list; }
        }

        public bool Update(Shortcut shortcut)
        {
            throw new NotImplementedException();
        }

        public bool Insert(Shortcut shortcut)
        {
            throw new NotImplementedException();
        }
    }
}
