//using Dapper;
//using Hibaza.CCP.Data.Providers.SQLServer;
//using Hibaza.CCP.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using Hibaza.CCP.Domain.Models;
//using Hibaza.CCP.Data.Providers.Mongo;
//using Newtonsoft.Json;
//namespace Hibaza.CCP.Data.Repositories
//{
//    public class AgentRepository : IAgentRepository
//    {
//        IConnectionFactory _connectionFactory;

//        public AgentRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public void Upsert(Agent agent)
//        {
//            var query = "dbo.AgentsUpsert";
//            var param = new DynamicParameters();

//            param.Add("@id", agent.id);
//            param.Add("@business_id", agent.business_id);
//            param.Add("@created_time", agent.created_time);
//            param.Add("@updated_time", agent.updated_time <= DateTime.MinValue ? null : agent.updated_time);
//            param.Add("@channel_id", agent.channel_id);
//            param.Add("@ext_id", agent.ext_id);
//            param.Add("@global_id", agent.global_id);
//            param.Add("@first_name", agent.first_name);
//            param.Add("@last_name", agent.last_name);
//            param.Add("@name", agent.name);
//            param.Add("@email", agent.email);
//            param.Add("@avatar", agent.avatar);
//            param.Add("@phone", agent.phone);
//            param.Add("@archived", agent.archived);
//            param.Add("@status", agent.status);
//            param.Add("@username", agent.username);
//            param.Add("@password", agent.password);
//            param.Add("@password_confirmation", agent.password_confirmation);
//            param.Add("@active", agent.active);
//            param.Add("@locked", agent.locked);
//            param.Add("@login_status", agent.login_status);
//            param.Add("@last_loggedin_time", agent.last_loggedin_time <= DateTime.MinValue ? null : agent.last_loggedin_time);
//            param.Add("@last_loggedout_time", agent.last_loggedout_time <= DateTime.MinValue ? null : agent.last_loggedout_time);
//            param.Add("@last_acted_time", agent.last_acted_time <= DateTime.MinValue ? null : agent.last_acted_time);
//            param.Add("@facebook_access_token", agent.facebook_access_token);
//            param.Add("@role", agent.role);
//            param.Add("@business_name", agent.business_name);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.StoredProcedure);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { agent.id });
//        }


//        public void AddRole(string business_id, string id, string role)
//        {
//            Agent agent = GetById(business_id, id);
//            agent.role = role;
//            Upsert(agent);
//        }


//        public bool Delete(string business_id, string id)
//        {
//            throw new NotImplementedException();
//        }

//        public void DeleteRole(string business_id, string id, string role)
//        {
//            throw new NotImplementedException();
//        }

//        public void DeleteRoleAll(string business_id, string id)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<dynamic> GetAgents(Paging page)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<IEnumerable<Agent>> GetAgents(string business_id, Paging page)
//        {
//            IEnumerable<Agent> list = null;
//            var key = "GetAgents" + business_id;
//            var rs = CacheBase.cacheManagerGet<List<Agent>>(key).Result;
//            if (rs != null && rs.Count > 0)
//            {
//                return rs;
//            }
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Agents"
//                               + " WHERE business_id = @business_id and (role = 'agent' or role = 'admin')";
//                list = await dbConnection.QueryAsync<Agent>(sQuery, new { business_id, limit = page.Limit });
//            }
//            CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                null, null, null, false, new List<string>() { });
//            return list;
//        }

//        public async Task<IEnumerable<Agent>> GetOnlineAgents(string business_id, Paging page)
//        {
//            IEnumerable<Agent> list;
//            var key = "GetOnlineAgents" + business_id;
//            var rs = CacheBase.cacheManagerGet<List<Agent>>(key).Result;
//            if (rs != null && rs.Count > 0)
//            {
//                return rs;
//            }
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Agents"
//                               + " WHERE business_id = @business_id and status = 'online' and (role = 'agent' or role = 'admin')";
//                list = await dbConnection.QueryAsync<Agent>(sQuery, new { business_id, limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                null, null, null, false, new List<string>() { });
//            }
//            return list;
//        }

//        public IEnumerable<Agent> GetAll(string business_id)
//        {
//            IEnumerable<Agent> list;
//            var key = "Agent_GetAll" + business_id;
//            var rs = CacheBase.cacheManagerGet<List<Agent>>(key).Result;
//            if (rs != null && rs.Count > 0)
//            {
//                return rs;
//            }
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Agents"
//                               + " WHERE business_id = @business_id";
//                list = dbConnection.Query<Agent>(sQuery, new { business_id });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                   null, null, null, false, new List<string>() { });
//            }
//            return list;
//        }

//        public Agent GetById(string id)
//        {
//            Agent agent;
//            var key = "Agent_GetById" + id;
//            var rs = CacheBase.cacheManagerGet<Agent>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Agents"
//                               + " WHERE id=@id";
//                agent = dbConnection.Query<Agent>(sQuery, new { id }).FirstOrDefault();
//                CacheBase.cacheManagerSet(key, agent, DateTime.Now.AddMinutes(10),
//                   null, null, null, false, new List<string>() { });
//            }
//            return agent;
//        }


//        public Agent GetById(string business_id, string id)
//        {
//            Agent agent;
//            var key = "Agent_GetById01" + business_id + id;
//            var rs = CacheBase.cacheManagerGet<Agent>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Agents"
//                               + " WHERE business_id = @business_id and id=@id";
//                agent = dbConnection.Query<Agent>(sQuery, new { business_id, id }).FirstOrDefault();
//                CacheBase.cacheManagerSet(key, agent, DateTime.Now.AddMinutes(10),
//                     null, null, null, false, new List<string>() { });
//            }
//            return agent;
//        }

//        public IEnumerable<Agent> GetByUserName(string username)
//        {
//            IEnumerable<Agent> list;
//            var key = "GetByUserName" + username;
//            var rs = CacheBase.cacheManagerGet<List<Agent>>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Agents"
//                               + " WHERE username = @username";
//                list = dbConnection.Query<Agent>(sQuery, new { username });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                     null, null, null, false, new List<string>() { });
//            }
//            return list;
//        }

//        public IEnumerable<string> GetRoles(string business_id, string id)
//        {
//            throw new NotImplementedException();
//        }

//    }
//}
