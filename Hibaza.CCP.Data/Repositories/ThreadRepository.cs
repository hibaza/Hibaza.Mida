//using Dapper;
//using Hibaza.CCP.Data.Providers.Firebase;
//using Hibaza.CCP.Data.Providers.SQLServer;
//using Hibaza.CCP.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using Hibaza.CCP.Domain.Models;
//using Firebase.Database.Query;
//using Firebase.Database;

//namespace Hibaza.CCP.Data.Repositories
//{
//    public class ThreadRepository : IThreadRepository
//    {
//        private const string THREADS = "threads";
//        private const string THREADS_ATTACHMENTS = "threads-attachments";
//        private const string REFERRALS = "referrals";
//        IConnectionFactory _connectionFactory;
//        public ThreadRepository(IConnectionFactory connectionFactory)
//        {
//            this._connectionFactory = connectionFactory;
//        }

//        public Thread GetById(string business_id, string id)
//        {
//            Domain.Entities.Thread thread = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM " + THREADS
//                               + " WHERE business_id=@business_id and id=@id";
//                thread = dbConnection.Query<Domain.Entities.Thread>(sQuery, new { business_id, id }).FirstOrDefault();
//            }
//            return thread;
//        }

//        public bool UpdateLastVisits(string business_id, string id, string last_visits)
//        {
//            var query = @"UPDATE [Threads] SET 
//              [last_visits] = @last_visits
//              WHERE id=@id and business_id=@business_id";
//            var param = new DynamicParameters();
//            param.Add("@id", id);
//            param.Add("@business_id", business_id);
//            param.Add("@last_visits", last_visits);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }

//        public int UpdateCustomerId()
//        {
//            return 0;
//        }

//        public bool UpdateCustomerId(string business_id, string id, string customer_id)
//        {
//            var query = @"UPDATE [Threads] SET 
//              [customer_id] = @customer_id
//              WHERE id=@id and business_id=@business_id";
//            var param = new DynamicParameters();
//            param.Add("@id", id);
//            param.Add("@business_id", business_id);
//            param.Add("@customer_id", customer_id);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }



//        public bool Update(Thread thread)
//        {
//            var query = @"UPDATE [Threads] SET 
//               [ext_id] = @ext_id
//              ,[type] = @type
//              ,[created_time] = @created_time
//              ,[updated_time] = @updated_time
//              ,[channel_ext_id] = @channel_ext_id
//              ,[channel_id] = @channel_id
//              ,[archived] = @archived
//              ,[status] = @status
//              ,[unread] = @unread
//              ,[nonreply] = @nonreply
//              ,[read_at] = @read_at
//              ,[read_by] = @read_by
//              ,[channel_type] = @channel_type
//              ,[agent_id] = @agent_id
//              ,[link_ext_id] = @link_ext_id
//              ,[customer_id] = @customer_id
//              ,[owner_id] = @owner_id
//              ,[owner_ext_id] = @owner_ext_id
//              ,[owner_app_id] = @owner_app_id
//              ,[owner_name] = @owner_name
//              ,[owner_avatar] = @owner_avatar
//              ,[onwer_timestamp] = @onwer_timestamp
//              ,[last_message_ext_id] = @last_message_ext_id
//              ,[last_message_parent_ext_id] = @last_message_parent_ext_id
//              ,[owner_last_message_ext_id] = @owner_last_message_ext_id
//              ,[owner_last_message_parent_ext_id] = @owner_last_message_parent_ext_id
//              ,[last_message] = @last_message
//              ,[last_visits] = @last_visits
//              ,[sender_id] = @sender_id
//              ,[sender_ext_id] = @sender_ext_id
//              ,[sender_name] = @sender_name
//              ,[sender_avatar] = @sender_avatar
//              ,[timestamp] = @timestamp
//              WHERE id=@id and business_id=@business_id";
//            var param = new DynamicParameters();
//            param.Add("@id", thread.id);
//            param.Add("@type", thread.type);
//            param.Add("@ext_id", thread.ext_id);
//            param.Add("@business_id", thread.business_id);
//            param.Add("@created_time", thread.created_time);
//            param.Add("@updated_time", thread.updated_time <= DateTime.MinValue ? null : thread.updated_time);
//            param.Add("@channel_ext_id", thread.channel_ext_id);
//            param.Add("@channel_id", thread.channel_id);
//            param.Add("@archived", thread.archived);
//            param.Add("@status", thread.status);
//            param.Add("@unread", thread.unread);
//            param.Add("@nonreply", thread.nonreply);
//            param.Add("@read_at", thread.read_at);
//            param.Add("@read_by", thread.read_by);
//            param.Add("@channel_type", thread.channel_type);
//            param.Add("@agent_id", thread.agent_id);
//            param.Add("@link_ext_id", thread.link_ext_id);
//            param.Add("@customer_id", thread.customer_id);
//            param.Add("@owner_id", thread.owner_id);
//            param.Add("@owner_ext_id", thread.owner_ext_id);
//            param.Add("@owner_app_id", thread.owner_app_id);
//            param.Add("@owner_name", thread.owner_name);
//            param.Add("@owner_avatar", thread.owner_avatar);
//            param.Add("@onwer_timestamp", thread.owner_timestamp);
//            param.Add("@last_message_ext_id", thread.last_message_ext_id);
//            param.Add("@last_message_parent_ext_id", thread.last_message_parent_ext_id);
//            param.Add("@owner_last_message_ext_id", thread.owner_last_message_ext_id);
//            param.Add("@owner_last_message_parent_ext_id", thread.owner_last_message_parent_ext_id);
//            param.Add("@last_message", thread.last_message);
//            param.Add("@last_visits", thread.last_visits);
//            param.Add("@sender_id", thread.sender_id);
//            param.Add("@sender_ext_id", thread.sender_ext_id);
//            param.Add("@sender_name", thread.sender_name);
//            param.Add("@sender_avatar", thread.sender_avatar);
//            param.Add("@timestamp", thread.timestamp);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }


//        public bool Insert(Thread thread)
//        {
//            var query = @"INSERT INTO [Threads]
//           ([id]
//           ,[type]
//           ,[ext_id]
//           ,[business_id]
//           ,[created_time]
//           ,[updated_time]
//           ,[channel_ext_id]
//           ,[channel_id]
//           ,[archived]
//           ,[status]
//           ,[unread]
//           ,[nonreply]
//           ,[read_at]
//           ,[read_by]
//           ,[channel_type]
//           ,[agent_id]
//           ,[link_ext_id]
//           ,[customer_id]
//           ,[owner_id]
//           ,[owner_ext_id]
//           ,[owner_app_id]
//           ,[owner_name]
//           ,[owner_avatar]
//           ,[onwer_timestamp]
//           ,[last_message_ext_id]
//           ,[last_message_parent_ext_id]
//           ,[owner_last_message_ext_id]
//           ,[owner_last_message_parent_ext_id]
//           ,[last_message]
//           ,[last_visits]
//           ,[sender_id]
//           ,[sender_ext_id]
//           ,[sender_name]
//           ,[sender_avatar]
//           ,[timestamp])
//     VALUES
//           (@id
//           ,@type
//           ,@ext_id
//           ,@business_id
//           ,@created_time
//           ,@updated_time
//           ,@channel_ext_id
//           ,@channel_id
//           ,@archived
//           ,@status
//           ,@unread
//           ,@nonreply
//           ,@read_at
//           ,@read_by
//           ,@channel_type
//           ,@agent_id
//           ,@link_ext_id
//           ,@customer_id
//           ,@owner_id
//           ,@owner_ext_id
//           ,@owner_app_id
//           ,@owner_name
//           ,@owner_avatar
//           ,@onwer_timestamp
//           ,@last_message_ext_id
//           ,@last_message_parent_ext_id
//           ,@owner_last_message_ext_id
//           ,@owner_last_message_parent_ext_id
//           ,@last_message
//           ,@last_visits
//           ,@sender_id
//           ,@sender_ext_id
//           ,@sender_name
//           ,@sender_avatar
//           ,@timestamp)";
//            var param = new DynamicParameters();
//            param.Add("@id", thread.id);
//            param.Add("@type", thread.type);
//            param.Add("@ext_id", thread.ext_id);
//            param.Add("@business_id", thread.business_id);
//            param.Add("@created_time", thread.created_time);
//            param.Add("@updated_time", thread.updated_time <= DateTime.MinValue ? null : thread.updated_time);
//            param.Add("@channel_ext_id", thread.channel_ext_id);
//            param.Add("@channel_id", thread.channel_id);
//            param.Add("@archived", thread.archived);
//            param.Add("@status", thread.status);
//            param.Add("@unread", thread.unread);
//            param.Add("@nonreply", thread.nonreply);
//            param.Add("@read_at", thread.read_at);
//            param.Add("@read_by", thread.read_by);
//            param.Add("@channel_type", thread.channel_type);
//            param.Add("@agent_id", thread.agent_id);
//            param.Add("@link_ext_id", thread.link_ext_id);
//            param.Add("@customer_id", thread.customer_id);
//            param.Add("@owner_id", thread.owner_id);
//            param.Add("@owner_ext_id", thread.owner_ext_id);
//            param.Add("@owner_app_id", thread.owner_app_id);
//            param.Add("@owner_name", thread.owner_name);
//            param.Add("@owner_avatar", thread.owner_avatar);
//            param.Add("@onwer_timestamp", thread.owner_timestamp);
//            param.Add("@last_message_ext_id", thread.last_message_ext_id);
//            param.Add("@last_message_parent_ext_id", thread.last_message_parent_ext_id);
//            param.Add("@owner_last_message_ext_id", thread.owner_last_message_ext_id);
//            param.Add("@owner_last_message_parent_ext_id", thread.owner_last_message_parent_ext_id);
//            param.Add("@last_message", thread.last_message);
//            param.Add("@last_visits", thread.last_visits);
//            param.Add("@sender_id", thread.sender_id);
//            param.Add("@sender_ext_id", thread.sender_ext_id);
//            param.Add("@sender_name", thread.sender_name);
//            param.Add("@sender_avatar", thread.sender_avatar);
//            param.Add("@timestamp", thread.timestamp);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }

//        public void Upsert(string business_id, Thread thread)
//        {
//            if (!Update(thread)) Insert(thread);
//            //UpsertSP(business_id, thread);
//        }


//        public void UpsertSP(string business_id, Thread thread)
//        {
//            var query = "dbo.ThreadsUpsert";
//            var param = new DynamicParameters();
//            param.Add("@id", thread.id);
//            param.Add("@type", thread.type);
//            param.Add("@ext_id", thread.ext_id);
//            param.Add("@business_id", thread.business_id);
//            param.Add("@created_time", thread.created_time);
//            param.Add("@updated_time", thread.updated_time <= DateTime.MinValue ? null: thread.updated_time);
//            param.Add("@channel_ext_id", thread.channel_ext_id);
//            param.Add("@channel_id", thread.channel_id);
//            param.Add("@archived", thread.archived);
//            param.Add("@status", thread.status);
//            param.Add("@unread", thread.unread);
//            param.Add("@nonreply", thread.nonreply);
//            param.Add("@read_at", thread.read_at);
//            param.Add("@read_by", thread.read_by);
//            param.Add("@channel_type", thread.channel_type);
//            param.Add("@agent_id", thread.agent_id);
//            param.Add("@link_ext_id", thread.link_ext_id);
//            param.Add("@customer_id", thread.customer_id);
//            param.Add("@owner_id", thread.owner_id);
//            param.Add("@owner_ext_id", thread.owner_ext_id);
//            param.Add("@owner_app_id", thread.owner_app_id);
//            param.Add("@owner_name", thread.owner_name);
//            param.Add("@owner_avatar", thread.owner_avatar);
//            param.Add("@onwer_timestamp", thread.owner_timestamp);
//            param.Add("@last_message_ext_id", thread.last_message_ext_id);
//            param.Add("@last_message_parent_ext_id", thread.last_message_parent_ext_id);
//            param.Add("@owner_last_message_ext_id", thread.owner_last_message_ext_id);
//            param.Add("@owner_last_message_parent_ext_id", thread.owner_last_message_parent_ext_id);
//            param.Add("@last_message", thread.last_message);
//            param.Add("@last_visits", thread.last_visits);
//            param.Add("@sender_id", thread.sender_id);
//            param.Add("@sender_ext_id", thread.sender_ext_id);
//            param.Add("@sender_name", thread.sender_name);
//            param.Add("@sender_avatar", thread.sender_avatar);
//            param.Add("@timestamp", thread.timestamp);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.StoredProcedure);
//            }
//            if (rowsAffected > 0)
//            {
//                //return true;
//            }
//        }

//        public async Task<IEnumerable<Thread>> SearchThreadsDistinctCustomerByKeywords(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page)
//        {
//            IEnumerable<Thread> list = null;
//            long end;
//            if (!long.TryParse(page.Next, out end))
//            {
//                end = 9999999999999;
//            }

//            var query = string.IsNullOrEmpty(keywords) ? "dbo.ThreadsGetDistinctCustomerByFilterOrderByDate" : "dbo.ThreadsFTSearchDistinctCustomerByOwnerNameOrderByRankAndDate";
//            var param = new DynamicParameters();
//            param.Add("@business_id", business_id);
//            param.Add("@channel_id", channel_id);
//            param.Add("@agent_id", agent_id);
//            param.Add("@status", status);
//            param.Add("@flag", flag);
//            param.Add("@keywords", keywords);
//            param.Add("@limit", page.Limit);
//            param.Add("@end", end);

//            using (var connection = _connectionFactory.GetConnection)
//            {
//                list = await connection.QueryAsync<Thread>(query, param, commandType: CommandType.StoredProcedure);
//                if ((list == null || list.Count() == 0) && !string.IsNullOrWhiteSpace(keywords))
//                {
//                    query = "dbo.ThreadsGetByFilterOrderByDate";
//                    list = await connection.QueryAsync<Thread>(query, param, commandType: CommandType.StoredProcedure);
//                }
//            }

//            return list;
//        }

//        public async Task<IEnumerable<Thread>> SearchThreadsByKeywords(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page)
//        {
//            IEnumerable<Thread> list = null;
//            long end;
//            if (!long.TryParse(page.Next, out end))
//            {
//                end = 9999999999999;
//            }

//            var query = string.IsNullOrEmpty(keywords) ? "dbo.ThreadsGetByFilterOrderByDate" : "dbo.ThreadsFTSearchByOwnerNameOrderByRankAndDate";
//            var param = new DynamicParameters();
//            param.Add("@business_id", business_id);
//            param.Add("@channel_id", channel_id);
//            param.Add("@agent_id", agent_id);
//            param.Add("@status", status);
//            param.Add("@flag", flag);
//            param.Add("@keywords", keywords);
//            param.Add("@limit", page.Limit);
//            param.Add("@end", end);

//            using (var connection = _connectionFactory.GetConnection)
//            {
//                list = await connection.QueryAsync<Thread>(query, param, commandType: CommandType.StoredProcedure);
//                if ((list == null || list.Count() == 0) && !string.IsNullOrWhiteSpace(keywords))
//                {
//                    query = "dbo.ThreadsGetByFilterOrderByDate";
//                    list = await connection.QueryAsync<Thread>(query, param, commandType: CommandType.StoredProcedure);
//                }
//            }

//            return list;
//        }

//        public async Task<IEnumerable<Thread>> GetThreadsByCustomer(string business_id, string customer_id, Paging page)
//        {
//            IEnumerable<Thread> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Threads t"
//                               + " WHERE t.business_id=@business_id and t.customer_id=@customer_id ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Thread>(sQuery, new { business_id, customer_id, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Thread>> GetUnreadOrNonReplyThreadsByCustomer(string business_id, string customer_id, Paging page)
//        {
//            IEnumerable<Thread> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Threads t"
//                               + " WHERE t.business_id=@business_id and t.customer_id=@customer_id and (t.unread > 0 or t.nonreply > 0) ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Thread>(sQuery, new { business_id, customer_id, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Thread>> GetThreadsWhereCustomerIsNull(string business_id, Paging page)
//        {
//            IEnumerable<Thread> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Threads t"
//                               + " WHERE t.business_id=@business_id and (t.customer_id='' or t.customer_id is null) ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Thread>(sQuery, new { business_id, limit = page.Limit });
//            }
//            return list;
//        }


//        public Task<IEnumerable<Thread>> GetThreads(string businessId, Paging page)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Thread>> GetThreadsByChannel(string businessId, Paging page, string channelId)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Thread>> GetThreadsByChannelAndAgent(string businessId, Paging page, string channelId, string agentId)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Thread>> GetThreadsByChannelAndStatus(string businessId, Paging page, string channelId, string status)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Thread>> GetThreadsByChannelAndAgentAndStatus(string businessId, Paging page, string channelId, string agentId, string status)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Thread>> GetThreadsByAgentAndStatus(string businessId, Paging page, string agentId, string status)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Thread>> GetThreadsByAgent(string businessId, Paging page, string agentId)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Thread>> GetThreadsByStatus(string businessId, Paging page, string status)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Thread>> GetThreadsByFlag(string businessId, Paging page, string flag)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Thread>> GetThreadsByAgentAndFlag(string businessId, Paging page, string agentId, string flag)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Thread>> GetThreadsByChannelAndFlag(string businessId, Paging page, string channelId, string flag)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<Thread>> GetThreadsByChannelAndAgentAndFlag(string businessId, Paging page, string channelId, string agentId, string flag)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<Thread> GetUnreadThreads(string businessId, Paging page)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetPageUIDByAppUID(string auid)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetBusinessUIDByPageUID(string puid)
//        {
//            throw new NotImplementedException();
//        }

//        public void UpdatePageBusinessMapping(string buid, string puid, string @ref)
//        {
//            throw new NotImplementedException();
//        }

//        public void UpdateAppPageMapping(string puid, string auid, string @ref)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetAppRefParam(string auid)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<Thread> GetAll(string business_id)
//        {
//            throw new NotImplementedException();
//        }

//        public bool Delete(string business_id, string id)
//        {
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "DELETE FROM Threads"
//                             + " WHERE id = @id and business_id=@business_id";
//                dbConnection.Execute(sQuery, new { business_id, id });
//            }
//            return true;
//        }

//        public async Task<IEnumerable<Thread>> GetActiveUnreadThreads(string business_id, string agent_id, Paging page)
//        {
//            IEnumerable<Thread> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Threads t"
//                               + " WHERE t.business_id=@business_id and t.agent_id=@agent_id and t.status='active'  and t.unread > 0 ORDER BY t.timestamp";
//                list = await dbConnection.QueryAsync<Thread>(sQuery, new { business_id, agent_id, limit = page.Limit });
//            }
//            return list;
//        }


//        public async Task<IEnumerable<Thread>> GetNoReponseThreads(string business_id, string channel_id, Paging page)
//        {
//            IEnumerable<Thread> list = null;
//            long until;
//            if (!long.TryParse(page.Next, out until))
//            {
//                until = 9999999999999;
//            }

//            long since;
//            if (!long.TryParse(page.Previous, out since))
//            {
//                since = 0;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Threads t"
//                               + " WHERE t.business_id=@business_id and (t.channel_id=@channel_id or @channel_id is null or @channel_id='') and t.type = 'message' and t.nonreply <= 0  and t.unread <= 0 and timestamp >= @since and timestamp<=@until ORDER BY timestamp";
//                list = await dbConnection.QueryAsync<Thread>(sQuery, new { business_id, channel_id, since, until, limit = page.Limit });
//            }
//            return list;
//        }


//        public async Task<IEnumerable<Counter>> GetChannelCounters(string business_id)
//        {
//            IEnumerable<Counter> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT channel_id as id, SUM(CASE WHEN unread > 0 THEN 1 ELSE 0 END) as unread,  COUNT(*) as count FROM Threads"
//                               + " WHERE business_id=@business_id GROUP BY channel_id";
//                list = await dbConnection.QueryAsync<Counter>(sQuery, new { business_id });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Counter>> GetAgentCounters(string business_id)
//        {
//            IEnumerable<Counter> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT agent_id as id, SUM(CASE WHEN unread > 0 THEN 1 ELSE 0 END) as unread,  COUNT(*) as count FROM Threads"
//                               + " WHERE business_id=@business_id GROUP BY agent_id";

//                list = await dbConnection.QueryAsync<Counter>(sQuery, new { business_id });
//            }
//            return list;
//        }
//        public Thread GetByIdFromCustomerId(string customerId)
//        {
//            Domain.Entities.Thread thread = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM " + THREADS
//                               + " WHERE  id=@customerId";
//                thread = dbConnection.Query<Domain.Entities.Thread>(sQuery, new { customerId }).FirstOrDefault();
//            }
//            return thread;
//        }

//    }
//}
