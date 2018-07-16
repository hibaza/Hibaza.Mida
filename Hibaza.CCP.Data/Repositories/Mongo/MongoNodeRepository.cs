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
    public class MongoNodeRepository : INodeRepository
    {
        public MongoFactory _mongoClient;
        const string nodes = "Nodes";
        IOptions<AppSettings> _appSettings = null;
        public MongoNodeRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
                _mongoClient = new MongoFactory(appSettings);
        }
        public async Task<IEnumerable<Node>> GetNodes(string business_id, string channel_id, Paging page)
        {
            var list = new List<Node>();
            try
            {
                var key = "Nodes_GetNodes" + business_id + channel_id;

                var options = new FindOptions<Node, Node>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Node>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",channel_id:\"" + channel_id + "\"}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, nodes,
                     true, key, DateTime.Now.AddMinutes(10), true);
            }
            catch { return list; }
        }

        public async Task<IEnumerable<Node>> GetPendingNodes(string business_id, string channel_id, string type, Paging page)
        {
            var list = new List<Node>();
            try
            {
                var key = "Nodes_GetPendingNodes" + business_id + channel_id + type;

                var options = new FindOptions<Node, Node>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Node>.Sort.Descending("created_time");
                // var query = "{$and:[{business_id: '"+business_id+"'},{ channel_id: '"+channel_id+"'},{ status: 'pending'},{$or:[{ type: "+type+" },{ type: ''}]}]}";
                var query = "{$and:[{ status: 'pending'},{$or:[{ type: \"" + type + "\" },{ type: ''}]}]}";
                return await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, nodes,
                     true, key, DateTime.Now.AddMinutes(10), true);
            }
            catch { return list; }
        }

        public bool UpdateStatus(string business_id, string id, string status)
        {
                var option = new UpdateOptions { IsUpsert = false };
            //var filter = Builders<Node>.Filter.Where(x => x.id == id && x.business_id == business_id);
            //var update = Builders<Node>.Update.Set(x => x.status , status ).Set(x=> x.updated_time, DateTime.Now);
            var filter = Builders<Node>.Filter.Where(x => x.id == id );
            var update = Builders<Node>.Update.Set(x => x.status, status).Set(x => x.updated_time, DateTime.Now);

            _mongoClient.excuteMongoLinqUpdateColumns<Node>(filter, update, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, nodes,
true, "", DateTime.Now.AddMinutes(10)).Wait();
            return true;
        }
        
        public void CreateNode(Node node)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Node>.Filter.Where(x => x.id == node.id && x.business_id == node.business_id);

            _mongoClient.excuteMongoLinqUpdate<Node>(filter, node, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, nodes,
true, "", DateTime.Now.AddMinutes(10)).Wait();
    
                CacheBase.cacheModifyAllKeyLinq(new List<string>() { node.id});
        }

        public bool Update(Node node)
        {
            throw new NotImplementedException();
        }

        public Node GetById(string business_id, string id)
        {
            try
            {
                var key = "Nodes_GetById" + business_id + id;

                var options = new FindOptions<Node, Node>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                //options.Sort = Builders<Node>.Sort.Descending("created_time");
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                var rs=  _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, nodes,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;                
            }
            catch { return null; }
        }


        public IEnumerable<Node> GetAll()
        {
            var list = new List<Node>();
            try
            {
                var key = "Nodes_GetAll";

                var options = new FindOptions<Node, Node>();
                options.Projection = "{'_id': 0}";
                options.Limit = 10;
                options.Sort = Builders<Node>.Sort.Descending("created_time");
                var query = "{}";
                return  _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, nodes,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
            }
            catch { return list; }
        }

        public bool Insert(Node node)
        {
            throw new NotImplementedException();
        }


        public bool Delete(string key)
        {
            throw new NotImplementedException();
        }
    }
}
