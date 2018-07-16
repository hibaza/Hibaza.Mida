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
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Hibaza.CCP.Core;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Hibaza.CCP.Data.Repositories
{
    public class MongoChannelRepository : IChannelRepository
    {
        public MongoFactory _mongoClient;
        IOptions<AppSettings> _appSettings = null;
        const string channels = "Channels";
        public MongoChannelRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }

        public void Add(Channel channel)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Channel>.Filter.Where(x => x.ext_id == channel.ext_id && x.business_id == channel.business_id);

            _mongoClient.excuteMongoLinqUpdate<Channel>(filter, channel, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, channels,
true, "", DateTime.Now.AddMinutes(10)).Wait();
                CacheBase.cacheModifyAllKeyLinq(new List<string>() { channel.ext_id });
        }

        public void UpsertId(Channel channel)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Channel>.Filter.Where(x => x.id == channel.id && x.business_id == channel.business_id);

            _mongoClient.excuteMongoLinqUpdate<Channel>(filter, channel, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, channels,
true, "", DateTime.Now.AddMinutes(10)).Wait();
            CacheBase.cacheModifyAllKeyLinq(new List<string>() { channel.id });
        }

        private bool Create(Channel channel)
        {
            throw new NotImplementedException();
        }

        private bool Update(Channel channel)
        {
            throw new NotImplementedException();
        }


        public bool Delete(string business_id, string id)
        {
            try
            {
                var key = "Channels_Delete" + business_id + id;
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                _mongoClient.excuteMongoLinqDelete<Channel>(query,
                   _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, channels,
                    true, key, DateTime.Now.AddMinutes(10), true);
                CacheBase.cacheModifyAllKeyLinq(new List<string>() { id });
                return true;
            }
            catch { return false; }
        }

        public IEnumerable<Channel> GetAll(string business_id)
        {
            var list = new List<Channel>();
            try
            {
                var key = "Channels_GetAll" + business_id;

                var options = new FindOptions<Channel, Channel>();
                options.Projection = "{'_id': 0}";
                options.Limit = 50;
                options.Sort = Builders<Channel>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\"}";
                return  _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, channels,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
            }
            catch { return list; }
        }

        public Channel GetById(string business_id, string id)
        {
            try
            {
                var key = "Channels_GetById" + business_id + id;

                var options = new FindOptions<Channel, Channel>();
                options.Projection = "{'_id': 0}";
                options.Limit = 50;
                //options.Sort = Builders<Channel>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                var rs= _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, channels,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;
            }
            catch { return null; }
        }

        public async Task<IEnumerable<Channel>> GetChannels(string business_id, Paging page)
        {
            var list = new List<Channel>();
            try
            {
                var key = "Channels_GetChannels" + business_id;

                var options = new FindOptions<Channel, Channel>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Channel>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, channels,
                     true, key, DateTime.Now.AddMinutes(10), true);
                
            }
            catch { return list; }
        }

        public async Task<List<Channel>> GetChannelsType(string business_id, string type, Paging page)
        {
            var list = new List<Channel>();
            try
            {
                var key = "GetChannelsType" + business_id + type;

                var options = new FindOptions<Channel, Channel>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Channel>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",type:\"" + type + "\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, channels,
                     true, key, DateTime.Now.AddMinutes(10), true);

            }
            catch { return list; }
        }

        public async Task<IEnumerable<Channel>> GetChannelsNotHotline(string business_id, Paging page)
        {
            var list = new List<Channel>();
            try
            {
                var key = "GetChannelsNotHotline" + business_id;

                var options = new FindOptions<Channel, Channel>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Channel>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",type:{$ne:\"hotline\"}}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, channels,
                     true, key, DateTime.Now.AddMinutes(10), true);

            }
            catch { return list; }
        }

        public async Task<IEnumerable<Channel>> GetChannelsByType(string business_id, string type, Paging page)
        {
            var list = new List<Channel>();
            try
            {
                var key = "GetChannelsByType" + business_id + type;

                var options = new FindOptions<Channel, Channel>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Channel>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",type:\""+type+"\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, channels,
                     true, key, DateTime.Now.AddMinutes(10), true);

            }
            catch { return list; }
        }

        public async Task<List<Channel>> GetChannelsByExtId(string ext_id, Paging page)
        {
            var list = new List<Channel>();
            try
            {
                var key = "Channels_GetChannelsByExtId" + ext_id;

                var options = new FindOptions<Channel, Channel>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Channel>.Sort.Descending("created_time");
                var query = "{ext_id:\"" + ext_id + "\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, channels,
                     true, key, DateTime.Now.AddMinutes(10), true);
            }
            catch { return list; }
        }
        public async Task<List<Channel>> GetByIdFromTrunk(string business_id, string trunk)
        {
            try
            {
                var key = "GetByIdFromTrunk" + business_id + trunk;

                var options = new FindOptions<Channel, Channel>();
                options.Projection = "{'_id': 0}";
                options.Limit = 50;
                //options.Sort = Builders<Channel>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",phones:{ $regex: \"" + trunk + "\"}}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, channels,
                     true, key, DateTime.Now.AddMinutes(10), true);
                
            }
            catch { return null; }
        }

    }
}
