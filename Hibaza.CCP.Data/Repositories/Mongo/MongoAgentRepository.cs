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
using MongoDB.Bson.Serialization;
using System.Linq.Expressions;

namespace Hibaza.CCP.Data.Repositories
{
    public class MongoAgentRepository : IAgentRepository
    {
        public MongoFactory _mongoClient ;
        IOptions<AppSettings> _appSettings ;
        const string agents = "Agents";
        public MongoAgentRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _mongoClient = new MongoFactory(appSettings);
        }

        

        public void Upsert(Agent agent)
        {
            var option = new UpdateOptions { IsUpsert = true };
            var filter = Builders<Agent>.Filter.Where(x => x.id == agent.id && x.business_id == agent.business_id);

            _mongoClient.excuteMongoLinqUpdate<Agent>(filter, agent, option,
_appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, agents,
true, "", DateTime.Now.AddMinutes(10)).Wait();

            try
            {
                if (agent.status.ToLower() == "offline")
                {
                    var uri = _appSettings.Value.BaseUrls.ApiHotline + "api/PhoneAccounts/changeStatus/" + agent.id + "/offline";
                    Core.Helpers.WebHelper.HttpGetAsync<string>(uri).Wait();
                }
            }
            catch { }
            CacheBase.cacheModifyAllKeyLinq(new List<string>() { agent.username });
        }


        public void AddRole(string business_id, string id, string role)
        {
            Agent agent = GetById(business_id, id);
            agent.role = role;
            Upsert(agent);
        }


        public bool Delete(string business_id, string id)
        {
            throw new NotImplementedException();
        }

        public void DeleteRole(string business_id, string id, string role)
        {
            throw new NotImplementedException();
        }

        public void DeleteRoleAll(string business_id, string id)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetAgents(Paging page)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Agent>> GetAgents(string business_id, Paging page)
        {
            var list = new List<Agent>();
            try
            {
                var key = "Agents_GetAgents" + business_id;

                var options = new FindOptions<Agent, Agent>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Agent>.Sort.Descending("status");
                var query = "{business_id:\"" + business_id + "\"}";
                list= await _mongoClient.excuteMongoLinqSelect(query, options,
                    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, agents,
                     true, key, DateTime.Now.AddMinutes(10), true);
                return list;
            }
            catch { return list; }
        }

        public async Task<IEnumerable<Agent>> GetOnlineAgents(string business_id, Paging page)
        {
            var list = new List<Agent>();
            try
            {
                var key = "Agents_GetOnlineAgents" + business_id;

                var options = new FindOptions<Agent, Agent>();
                options.Projection = "{'_id': 0}";
                options.Limit = page.Limit;
                options.Sort = Builders<Agent>.Sort.Descending("status");
                var query = "{business_id:\"" + business_id + "\",status: \"online\"}";
                list = await _mongoClient.excuteMongoLinqSelect(query, options, _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, agents,
                     true, key, DateTime.Now.AddMinutes(10), true);
                return list;

            }
            catch { return list; }
        }

        public IEnumerable<Agent> GetAll(string business_id)
        {
            var list = new List<Agent>();
            try
            {
                var key = "Agents_GetAll" + business_id;

                var options = new FindOptions<Agent, Agent>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                options.Sort = Builders<Agent>.Sort.Descending("status");
                var query = "{business_id:\"" + business_id + "\"}";
                list = _mongoClient.excuteMongoLinqSelect(query, options, _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, agents,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
                return list;
            }
            catch { return list; }
        }

        public Agent GetById(string id)
        {
            try
            {
                var key = "Agents_GetById" + id;

                var options = new FindOptions<Agent, Agent>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                //options.Sort = Builders<Agent>.Sort.Descending("status");
                var query = "{id:\"" + id + "\"}";
                var rs = _mongoClient.excuteMongoLinqSelect(query, options, _appSettings.Value.MongoDB.ConnectionString,
                    _appSettings.Value.MongoDB.Database, agents,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;
            }
            catch { return null; }

        }


        public Agent GetById(string business_id, string id)
        {
            try
            {
                var key = "Agents_GetById01" + business_id + id;

                var options = new FindOptions<Agent, Agent>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                options.Sort = Builders<Agent>.Sort.Descending("status");
                var query = "{business_id:\"" + business_id + "\",id:\"" + id + "\"}";
                var rs = _mongoClient.excuteMongoLinqSelect(query, options, _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, agents,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;
                if (rs != null && rs.Count > 0)
                    return rs[0];
                return null;
            }
            catch { return null; }
        }

        public IEnumerable<Agent> GetByUserName(string username)
        {
            var list = new List<Agent>();
            try
            {
                var key = "GetByUserName" + username;

                var options = new FindOptions<Agent, Agent>();
                options.Projection = "{'_id': 0}";
                options.Limit = 1;
                options.Sort = Builders<Agent>.Sort.Descending("status");
                var query = "{username:\"" + username + "\"}";
                list = _mongoClient.excuteMongoLinqSelect(query, options, _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.Database, agents,
                     true, key, DateTime.Now.AddMinutes(10), true).Result;

                var option1 = new UpdateOptions { IsUpsert = false };
                var filter1 = Builders<Agent>.Filter.Where(x => x.username == username);
                var update1 = Builders<Agent>.Update.Set(p => p.last_loggedin_time, DateTime.UtcNow).
    Set(p => p.status, "busy").
    Set(p => p.login_status, "online").
    Set(p => p.last_acted_time, DateTime.UtcNow)

    ;
                _mongoClient.excuteMongoLinqUpdateColumns<Agent>(filter1, update1, option1,
    _appSettings.Value.MongoDB.ConnectionString, _appSettings.Value.MongoDB.DatabaseAi, agents,
    true, "", DateTime.Now.AddMinutes(10)).Wait();
                
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        public IEnumerable<string> GetRoles(string business_id, string id)
        {
            throw new NotImplementedException();
        }

    }
}
