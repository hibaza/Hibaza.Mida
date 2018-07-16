using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface ITaskRepository
    {
        Domain.Entities.Task GetById(string id);
        IEnumerable<Domain.Entities.Task> GetAll();
        void Add(Domain.Entities.Task entity);
        bool Delete(string id);
        void Update(Domain.Entities.Task entity);

        Task<IEnumerable<Domain.Entities.Task>> GetAllTasksByPageIndex(int pageIndex, int pageSize);
        bool CreateTask(Domain.Entities.Task task);
        bool UpdateTask(Domain.Entities.Task task);
        IEnumerable<Domain.Entities.AgentTask> GetTasksForAgent(string agentId);
        bool CreateAgentTask(Domain.Entities.AgentTask agentTask);
        bool UpdateAgentTask(Domain.Entities.AgentTask agentTask);
        bool DeleteAgentTask(Domain.Entities.AgentTask agentTask);
    }
}
