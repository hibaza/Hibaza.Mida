using Dapper;
using Firebase.Database.Query;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public class FirebaseAgentRepository : IFirebaseAgentRepository
    {
        IFirebaseFactory _connectionFactory;
        private const string AGENTS = "agents";
        private const string AGENTS_ROLES = AGENTS + "-roles";
        public FirebaseAgentRepository(IFirebaseFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Add(AgentModel entity)
        {
            var rs = _connectionFactory.GetConnection.Child(AGENTS).Child(entity.id).PutAsync(entity);
        }


        public void Update(AgentModel entity)
        {
            throw new NotImplementedException();
        }

        public AgentModel GetById(string id)
        {
            var c = _connectionFactory.GetConnection
              .Child(AGENTS).Child(id).OnceSingleAsync<AgentModel>().Result;
            return c;
        }

        public AgentModel GetById(string business_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(business_id)
              .Child(AGENTS).Child(id).OnceSingleAsync<AgentModel>().Result;
            return c;
        }

        public IEnumerable<AgentModel> GetByUserName(string username)
        {
            var rs = _connectionFactory.GetConnection.Child(AGENTS).OrderBy("username").StartAt(username).EndAt(username).LimitToLast(100).OnceAsync<AgentModel>().Result;
            var list = new List<AgentModel>();
            foreach (var a in rs)
            {
                list.Add(a.Object);
            }
            return list;
        }


        public async Task<dynamic> GetAgents(Paging page)
        {
            return await _connectionFactory.GetConnection.Child(AGENTS).OrderBy("created_time").LimitToLast(page.Limit).OnceAsync<AgentModel>();
        }

        public async Task<dynamic> GetAgents(string business_id, Paging page)
        {
            return await _connectionFactory.GetConnection.Child(business_id).Child(AGENTS).OrderBy("created_time").LimitToLast(page.Limit).OnceAsync<AgentModel>();
        }

        public bool Delete(string id)
        {
            var c = _connectionFactory.GetConnection.Child(AGENTS).Child(id).DeleteAsync();
            return true;
        }

        public void AddRole(string business_id, string id, string role)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(AGENTS_ROLES).Child(id).Child(role).PutAsync(true);
        }

        public void DeleteRole(string business_id, string id, string role)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(AGENTS_ROLES).Child(id).Child(role).DeleteAsync();
        }

        public void DeleteRoleAll(string business_id, string id)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(AGENTS_ROLES).Child(id).DeleteAsync();
        }

        public IEnumerable<string> GetRoles(string business_id, string id)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(AGENTS_ROLES).Child(id).OnceAsync<string>().Result.Select(r => r.Key);
            return rs;
        }

        public void Upsert(string business_id, AgentModel agent)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(AGENTS).Child(agent.id).PutAsync(agent);
        }

        public bool Delete(string bussiness_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(bussiness_id).Child(AGENTS).Child(id).DeleteAsync();
            return true;
        }

        public IEnumerable<AgentModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AgentModel> GetAll(string business_id)
        {
            throw new NotImplementedException();
        }

        public void Update(string business_id, AgentModel entity)
        {
            throw new NotImplementedException();
        }
    }
}
