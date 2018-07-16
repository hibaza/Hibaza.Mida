//using Dapper;
//using Hibaza.CCP.Data.Providers.SQLServer;
//using Hibaza.CCP.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using Hibaza.CCP.Domain.Models;

//namespace Hibaza.CCP.Data.Repositories
//{
//    public class ConversationRepository : IConversationRepository
//    {
//        IConnectionFactory _connectionFactory;

//        public ConversationRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public bool Insert(Conversation conversation)
//        {
//            var query = @"INSERT INTO [Conversations]([id],[ext_id],[business_id],[channel_id],[owner_ext_id],[owner_app_id],[link],[timestamp],[created_time],[updated_time]) VALUES( 
//                     @id,@ext_id,@business_id,@channel_id,@owner_ext_id, @owner_app_id, @link,@timestamp,@created_time, @updated_time)";

//            var param = new DynamicParameters();
//            param.Add("@id", conversation.id);
//            param.Add("@ext_id", conversation.ext_id);
//            param.Add("@business_id", conversation.business_id);
//            param.Add("@channel_id", conversation.channel_id);
//            param.Add("@owner_app_id", conversation.owner_app_id);
//            param.Add("@owner_ext_id", conversation.owner_ext_id);
//            param.Add("@link", conversation.link);
//            param.Add("@created_time", conversation.created_time);
//            param.Add("@updated_time", conversation.updated_time = conversation.updated_time <=DateTime.MinValue ? null : conversation.updated_time);
//            param.Add("@timestamp", conversation.timestamp);


//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }

//            return rowsAffected > 0;
//        }

//        public bool UpdateOwner(string business_id, string id, string owner_ext_id, string owner_app_id)
//        {
//            var query = @"UPDATE [Conversations] SET
//                   [updated_time] = @updated_time
//                  ,[owner_ext_id] = @owner_ext_id
//                  ,[owner_app_id] = @owner_app_id
//             WHERE id=@id and business_id=@business_id";

//            var param = new DynamicParameters();
//            param.Add("@id", id);
//            param.Add("@business_id", business_id);
//            param.Add("@owner_app_id", owner_app_id);
//            param.Add("@owner_ext_id", owner_ext_id);
//            param.Add("@updated_time", DateTime.UtcNow);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }

//            return rowsAffected > 0;
//        }

//        private bool Update(Conversation conversation)
//        {
//            var query = @"UPDATE [Conversations] SET [ext_id]=@ext_id,[channel_id]=@channel_id,[owner_ext_id]=@owner_ext_id,[owner_app_id]=@owner_app_id,[link]=@link,[status]=@status,[timestamp]=@timestamp,[updated_time]=@updated_time 
//                     WHERE id=@id and business_id=@business_id";

//            var param = new DynamicParameters();
//            param.Add("@id", conversation.id);
//            param.Add("@ext_id", conversation.ext_id);
//            param.Add("@business_id", conversation.business_id);
//            param.Add("@owner_app_id", conversation.owner_app_id);
//            param.Add("@owner_ext_id", conversation.owner_ext_id);
//            param.Add("@channel_id", conversation.channel_id);
//            param.Add("@timestamp", conversation.timestamp);
//            param.Add("@status", conversation.status);
//            param.Add("@link", conversation.link);
//            param.Add("@updated_time", DateTime.UtcNow);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }

//            return rowsAffected > 0;
//        }

//        private bool UpdateTimestamp(string business_id, string id, long timestamp)
//        {
//            var query = @"UPDATE [Conversations] SET [timestamp]=@timestamp, 
//                     WHERE id=@id and business_id=@business_id";

//            var param = new DynamicParameters();
//            param.Add("@id", id);
//            param.Add("@business_id", business_id);
//            param.Add("@timestamp", timestamp);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }

//            return rowsAffected > 0;
//        }


//        public void Upsert(Conversation conversation)
//        {
//            if (!Update(conversation)) Insert(conversation);
//        }

//        public void UpsertTimestamp(Conversation conversation)
//        {
//            if (!UpdateTimestamp(conversation.business_id, conversation.id, conversation.timestamp)) Insert(conversation);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="business_id"></param>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public Conversation GetById(string business_id, string id)
//        {
//            dynamic rs = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Conversations"
//                               + " WHERE business_id=@business_id and id=@id";
//                rs = dbConnection.Query<Conversation>(sQuery, new { business_id, id }).FirstOrDefault();
//            }
//            return rs;
//        }


//        public bool UpdateStatus(string business_id, string id, string status)
//        {
//            var query = @"UPDATE [Conversations] SET
//                   [status] = @status
//                  ,[updated_time] = @updated_time
//             WHERE id=@id and business_id=@business_id";

//            var param = new DynamicParameters();
//            param.Add("@id", id);
//            param.Add("@business_id", business_id);
//            param.Add("@status", status);
//            param.Add("@updated_time", DateTime.UtcNow);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }

//            return rowsAffected > 0;
//        }

//        public async Task<IEnumerable<Conversation>> GetConversations(string business_id, string channel_id, Paging page)
//        {
//            IEnumerable<Conversation> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Conversations"
//                               + " WHERE business_id=@business_id and channel_id=@channel_id  and timestamp<=@until and timestamp>=@since ORDER BY timestamp desc";
//                list = await dbConnection.QueryAsync<Conversation>(sQuery, new { business_id, channel_id, limit = page.Limit, since = long.Parse(page.Previous ?? "0"), until = long.Parse(page.Next ?? "9999999999") });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Conversation>> GetConversationsWhereExtIdIsNull(string business_id, string channel_id, int limit)
//        {
//            dynamic rs = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Conversations"
//                               + " WHERE business_id=@business_id and channel_id=@channel_id and (owner_ext_id is null or owner_ext_id = '') order by timestamp desc";
//                rs = await dbConnection.QueryAsync<Conversation>(sQuery, new { business_id, channel_id, limit });
//            }
//            return rs;
//        }

//        public Conversation GetByOwnerExtId(string business_id, string owner_ext_id)
//        {
//            dynamic rs = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Conversations"
//                               + " WHERE business_id=@business_id and owner_ext_id=@owner_ext_id order by timestamp desc";
//                rs = dbConnection.Query<Conversation>(sQuery, new { business_id, owner_ext_id }).FirstOrDefault();
//            }
//            return rs;
//        }
//        public Conversation GetByOwnerAppId(string business_id, string channel_id, string owner_app_id)
//        {
//            dynamic rs = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Conversations"
//                               + " WHERE business_id=@business_id and channel_id=@channel_id and owner_app_id=@owner_app_id order by timestamp desc";
//                rs = dbConnection.Query<Conversation>(sQuery, new { business_id, channel_id, owner_app_id }).FirstOrDefault();
//            }
//            return rs;
//        }
//    }
//}
