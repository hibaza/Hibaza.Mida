using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public interface IFirebaseAgentRepository : IGenericRepository<AgentModel>
    {

        IEnumerable<string> GetRoles(string business_id, string id);
        void AddRole(string business_id, string id, string role);
        void DeleteRole(string business_id, string id, string role);
        void DeleteRoleAll(string business_id, string id);
        Task<dynamic> GetAgents(Paging page);
        Task<dynamic> GetAgents(string business_id, Paging page);
        IEnumerable<AgentModel> GetByUserName(string username);
    }
}
