using Dapper;
using Hibaza.CCP.Data.Providers.Mongo;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        IConnectionFactory _connectionFactory;
        
        public TaskRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<IEnumerable<Domain.Entities.Task>> GetAllTasksByPageIndex(int pageIndex, int pageSize)
        {
            var query = "usp_GetAllTasksByPageIndex";
            var param = new DynamicParameters();
            param.Add("@PageIndex", pageIndex);
            param.Add("@PageSize", pageSize);
            var list = await SqlMapper.QueryAsync<Domain.Entities.Task>(_connectionFactory.GetConnection, query, param, commandType: CommandType.StoredProcedure);
            return list;
        }

        public bool CreateTask(Domain.Entities.Task task)
        {
            var query = @"INSERT INTO Tasks([Id],[ConversationId],[Type],[CreatedTime]) VALUES(@Id, @ConversationId, @Type, @CreatedTime)";
            var param = new DynamicParameters();
            param.Add("@Id", task.id);
            param.Add("@ConversationId", task.ConversationId);
            param.Add("@Type", task.Type);
            param.Add("@CreatedTime", task.created_time);
            int rowsAffected;
            using (var connection = _connectionFactory.GetConnection)
            {
                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
            }
            //CacheBase.cacheDeleteCacheSame(new List<string>() {task.id });
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }

        public bool UpdateTask(Domain.Entities.Task task)
        {
            var query = @"UPDATE [Tasks] SET [LastMessageId] = @LastMessageId, [ConversationId] = @ConversationId, [UpdatedTime] = @UpdatedTime WHERE Id = @Id";
            var param = new DynamicParameters();
            param.Add("@Id", task.id);
            param.Add("@ConversationId", task.ConversationId);
            param.Add("@LastMessageId", task.LastMessageId);
            param.Add("@UdpatedTime", DateTime.UtcNow);
            int rowsAffected;
            using (var connection = _connectionFactory.GetConnection)
            {
                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
            }
           // CacheBase.cacheDeleteCacheSame(new List<string>() {task.id });
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }

        public bool UpdateAgentTask(AgentTask agentTask)
        {
            var query = @"UPDATE [AgentTasks] SET [LastMessageId] = @LastMessageId, [ConversationId] = @ConversationId, [UpdatedTime] = @UpdatedTime WHERE Id = @Id";
            var param = new DynamicParameters();
            param.Add("@Id", agentTask.id);
            param.Add("@ConversationId", agentTask.ConversationId);
            param.Add("@LastMessageId", agentTask.LastMessageId);
            param.Add("@UdpatedTime", DateTime.UtcNow);
            int rowsAffected;
            using (var connection = _connectionFactory.GetConnection)
            {
                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
            }
           // CacheBase.cacheDeleteCacheSame(new List<string>() {agentTask.id });
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }

        public bool CreateAgentTask(AgentTask agentTask)
        {
            var query = @"INSERT INTO AgentTasks([Id],[AgentId],[TaskId],[ConversationId],[Type],[CreatedTime]) VALUES(@Id, @AgentId, @TaskId, @ConversationId, @Type, @CreatedTime)";
            var param = new DynamicParameters();
            param.Add("@Id", agentTask.id);
            param.Add("@AgentId", agentTask.AgentId);
            param.Add("@TaskId", agentTask.TaskId);
            param.Add("@ConversationId", agentTask.ConversationId);
            param.Add("@Type", agentTask.Type);
            param.Add("@CreatedTime", agentTask.created_time);
            int rowsAffected;
            using (var connection = _connectionFactory.GetConnection)
            {
                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
            }
          //  CacheBase.cacheDeleteCacheSame(new List<string>() {agentTask.id });
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }


        public bool DeleteAgentTask(AgentTask agentTask)
        {
            throw new NotImplementedException();
        }

        public Domain.Entities.Task GetById(string id)
        {
            Domain.Entities.Task task = null;
            using (var dbConnection = _connectionFactory.GetConnection)
            {
                string sQuery = "SELECT * FROM Tasks"
                               + " WHERE Id = @Id";
                task = dbConnection.Query<Domain.Entities.Task>(sQuery, new { Id = id }).FirstOrDefault();
            }
            return task;
        }

        public IEnumerable<AgentTask> GetTasksForAgent(string agentId)
        {
            IEnumerable<Domain.Entities.AgentTask> list;
            var key = "GetTasksForAgent" + agentId;
            var rs = CacheBase.cacheManagerGet<IEnumerable<AgentTask>>(key).Result;
            if (rs != null)
            {
                return rs;
            }

            using (var dbConnection = _connectionFactory.GetConnection)
            {
                string sQuery = "SELECT * FROM AgentTasks"
                               + " WHERE AgentId = @AgentId";
                list = dbConnection.Query<Domain.Entities.AgentTask>(sQuery, new { AgentId = agentId });
                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
                 null, null, "", false, new List<string>() { });
            }
            return list;
        }


        public IEnumerable<Domain.Entities.Task> GetAll()
        {
            IEnumerable<Domain.Entities.Task> list;
            var key = "Task_GetAll";
            var rs = CacheBase.cacheManagerGet<IEnumerable<Domain.Entities.Task>>(key).Result;
            if (rs != null)
            {
                return rs;
            }

            using (var dbConnection = _connectionFactory.GetConnection)
            {
                var sQuery = @"SELECT* FROM Tasks";

                list = dbConnection.Query<Domain.Entities.Task>(sQuery);
                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
                 null, null, "", false, new List<string>() { });
            }
            return list;
        }


        public void Add(Domain.Entities.Task entity)
        {
            throw new NotImplementedException();
        }


        public void Update(Domain.Entities.Task entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string key)
        {
            throw new NotImplementedException();
        }
    }
}
