using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IAgentRepository
    {
        Agent GetById(string id);
        Agent GetById(string business_id, string id);
        IEnumerable<Agent> GetAll(string business_id);
        bool Delete(string business_id, string id);
        void Upsert(Agent agent);

        IEnumerable<string> GetRoles(string business_id, string id);
        void AddRole(string business_id, string id, string role);
        void DeleteRole(string business_id, string id, string role);
        void DeleteRoleAll(string business_id, string id);
        Task<dynamic> GetAgents(Paging page);
        Task<IEnumerable<Agent>> GetAgents(string business_id, Paging page);
        Task<IEnumerable<Agent>> GetOnlineAgents(string business_id, Paging page);
        IEnumerable<Agent> GetByUserName(string username);
    }
}
