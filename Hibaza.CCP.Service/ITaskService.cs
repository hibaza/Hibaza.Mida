using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface ITaskService
    {
        Task<int> AutoAssignToAvailableAgents(string business_id, Paging page);
        Task<int> SetBusyAllInActivityAgents(string business_id, int minutes);
        Task<int> LogoutAllInActivityAgents(string business_id, int minutes);
    }
}
