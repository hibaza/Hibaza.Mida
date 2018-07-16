using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface IAgentService
    {
        Task<IEnumerable<Agent>> GetAgents(string business_id, int pageIndex, int pageSize);
        Task<IEnumerable<Agent>> GetOnlineAgents(string business_id, int pageIndex, int pageSize);
        string Create(Domain.Entities.Agent data);
        Agent GetById(string id);
        Agent GetById(string business_id, string id);
        bool Delete(string business_id, string id);
        Agent GetSingleOrDefaultByUserName(string username);
        string ToogleRole(string business_id, string id, string role);
        Agent SetLoginStatus(string id, string loginStatus, DateTime time);
        Agent SetLoginStatus(Agent agent, string status, DateTime time);
        Agent SetWorkStatus(string id, string loginStatus, DateTime time);
        Task<int> LogoutAllInActivityAgents(string business_id, int minutes);
        Task<int> SetBusyAllInActivityAgents(string business_id, int minutes);
        System.Threading.Tasks.Task SetLastActivityTime(string id, long timestamp);
    }
}
