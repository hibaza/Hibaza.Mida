//using Dapper;
//using Hibaza.CCP.Data.Providers.SQLServer;
//using Hibaza.CCP.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using Hibaza.CCP.Domain.Models;
//using Newtonsoft.Json;

//namespace Hibaza.CCP.Data.Repositories
//{
//    public class MessageRepository : IMessageRepository
//    {
//        IConnectionFactory _connectionFactory;

//        public MessageRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public void AddGroupedByThread(string business_id, Message entity, string threadId)
//        {
//            throw new NotImplementedException();
//        }

//        public void AddGroupedByUser(string business_id, Message entity, string userId)
//        {
//            throw new NotImplementedException();
//        }

//        public string CreateMessageAgentMap(string business_id, string message_id, string agent_id)
//        {
//            throw new NotImplementedException();
//        }

//        public bool Delete(string business_id, string id)
//        {
//            throw new NotImplementedException();
//        }

//        public bool Delete(string business_id, string thread_id, string id)
//        {
//            throw new NotImplementedException();
//        }

//        public void DeleteAllForThread(string business_id, string thread_id)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<IEnumerable<Message>> GetAll(string business_id, Paging page)
//        {
//            IEnumerable<Message> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Messages WHERE business_id=@business_id"
//                    + " and timestamp>=@until  ORDER BY timestamp";
//                list = await dbConnection.QueryAsync<Message>(sQuery, new { business_id, limit = page.Limit, until = long.Parse(page.Next ?? "0") });
//            }
//            return list;
//        }

//        public Message GetById(string business_id, string id)
//        {
//            Message message = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Messages WHERE business_id=@business_id and id=@id";
//                message = dbConnection.Query<Message>(sQuery, new { business_id, id }).FirstOrDefault();
//            }
//            return message;
//        }

//        public string GetMessageAgentMap(string business_id, string message_id)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<IEnumerable<Message>> GetStarredMessagesByCustomer(string business_id, Paging page, string customer_id)
//        {
//            IEnumerable<Message> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Messages WHERE business_id=@business_id and customer_id=@customer_id and starred>0 ORDER BY timestamp DESC";
//                list = await dbConnection.QueryAsync<Message>(sQuery, new { business_id, customer_id, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Message>> GetCustomerOrAgentMessagesNonDeletedByThread(string business_id, Paging page, string thread_id)
//        {
//            IEnumerable<Message> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Messages WHERE business_id=@business_id and thread_id=@thread_id"
//                    + " and timestamp<=@until and (sender_ext_id!=channel_ext_id or (agent_id is not null and agent_id != '')) and deleted < 1  ORDER BY timestamp DESC";
//                list = await dbConnection.QueryAsync<Message>(sQuery, new { business_id, thread_id, limit = page.Limit, until = long.Parse(page.Next) });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Message>> GetMessagesByThread(string business_id, Paging page, string thread_id)
//        {
//            IEnumerable<Message> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Messages WHERE business_id=@business_id and thread_id=@thread_id"
//                    + " and timestamp<=@until  ORDER BY timestamp DESC";
//                list = await dbConnection.QueryAsync<Message>(sQuery, new { business_id, thread_id, limit = page.Limit, until = long.Parse(page.Next) });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Message>> GetNonDeletedMessagesByThread(string business_id, Paging page, string thread_id)
//        {
//            IEnumerable<Message> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Messages WHERE business_id=@business_id and thread_id=@thread_id"
//                    + " and deleted < 1 and timestamp<=@until  ORDER BY timestamp DESC";
//                list = await dbConnection.QueryAsync<Message>(sQuery, new { business_id, thread_id, limit = page.Limit, until = long.Parse(page.Next) });
//            }
//            return list;
//        }


//        public async Task<IEnumerable<Message>> GetMessagesByCustomer(string business_id, string customer_id, Paging page)
//        {
//            IEnumerable<Message> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Messages WHERE business_id=@business_id and customer_id=@customer_id"
//                    + " and timestamp<=@until  ORDER BY timestamp DESC";
//                list = await dbConnection.QueryAsync<Message>(sQuery, new { business_id, customer_id, limit = page.Limit, until = long.Parse(page.Next) });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Message>> GetMessagesByCustomerExcludeCurrentThread(string business_id, string customer_id, string thread_id, Paging page)
//        {
//            IEnumerable<Message> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Messages WHERE business_id=@business_id and customer_id=@customer_id and thread_id!=@thread_id"
//                    + " and timestamp<=@until  ORDER BY timestamp DESC";
//                list = await dbConnection.QueryAsync<Message>(sQuery, new { business_id, customer_id, thread_id, limit = page.Limit, until = long.Parse(page.Next) });
//            }
//            return list;
//        }

//        public bool MoveAllUserMessagesTo(string business_id, string from_id, string to_id)
//        {
//            throw new NotImplementedException();
//        }


//        public bool Insert(Message message)
//        {
//            var query = @"INSERT INTO [Messages]
//                           ([id]
//                           ,[parent_id]
//                           ,[parent_ext_id]
//                           ,[root_ext_id]
//                           ,[business_id]
//                           ,[ext_id]
//                           ,[url]
//                           ,[urls]
//                           ,[tag]
//                           ,[template]  
//                           ,[file_name]
//                           ,[size]
//                           ,[subject]
//                           ,[message]
//                           ,[agent_id]
//                           ,[thread_id]
//                           ,[thread_type]
//                           ,[conversation_ext_id]
//                           ,[sender_id]
//                           ,[sender_ext_id]
//                           ,[sender_name]
//                           ,[sender_avatar]
//                           ,[recipient_id]
//                           ,[recipient_ext_id]
//                           ,[recipient_name]
//                           ,[recipient_avatar]
//                           ,[author]
//                           ,[customer_id]
//                           ,[type]
//                           ,[read]
//                           ,[hidden]
//                           ,[liked]
//                           ,[starred]
//                           ,[timestamp]
//                           ,[replied]
//                           ,[replied_at]
//                           ,[channel_id]
//                           ,[channel_ext_id]
//                           ,[channel_type]
//                           ,[created_time]
//                           ,[updated_time])
//                     VALUES
//                           (@id
//                           ,@parent_id
//                           ,@parent_ext_id
//                           ,@root_ext_id
//                           ,@business_id
//                           ,@ext_id
//                           ,@url
//                           ,@urls
//                           ,@tag
//                           ,@template
//                           ,@file_name
//                           ,@size
//                           ,@subject
//                           ,@message
//                           ,@agent_id
//                           ,@thread_id
//                           ,@thread_type
//                           ,@conversation_ext_id
//                           ,@sender_id
//                           ,@sender_ext_id
//                           ,@sender_name
//                           ,@sender_avatar
//                           ,@recipient_id
//                           ,@recipient_ext_id
//                           ,@recipient_name
//                           ,@recipient_avatar
//                           ,@author
//                           ,@customer_id
//                           ,@type
//                           ,@read
//                           ,@hidden
//                           ,@liked
//                           ,@starred
//                           ,@timestamp
//                           ,@replied
//                           ,@replied_at
//                           ,@channel_id
//                           ,@channel_ext_id
//                           ,@channel_type
//                           ,@created_time
//                           ,@updated_time)";
//            var param = new DynamicParameters();
//            param.Add("@id", message.id);
//            param.Add("@parent_id", message.parent_id);
//            param.Add("@parent_ext_id", message.parent_ext_id);
//            param.Add("@root_ext_id", message.root_ext_id);
//            param.Add("@business_id", message.business_id);
//            param.Add("@ext_id", message.ext_id);
//            param.Add("@url", message.url);
//            param.Add("@urls", message.urls);
//            param.Add("@tag", message.tag);
//            param.Add("@template", message.template);
//            param.Add("@file_name", message.file_name);
//            param.Add("@size", message.size);
//            param.Add("@subject", message.subject);
//            param.Add("@message", message.message);
//            param.Add("@agent_id", message.agent_id);
//            param.Add("@thread_id", message.thread_id);
//            param.Add("@thread_type", message.thread_type);
//            param.Add("@conversation_ext_id", message.conversation_ext_id);
//            param.Add("@sender_id", message.sender_id);
//            param.Add("@sender_ext_id", message.sender_ext_id);
//            param.Add("@sender_name", message.sender_name);
//            param.Add("@sender_avatar", message.sender_avatar);
//            param.Add("@recipient_id", message.recipient_id);
//            param.Add("@recipient_ext_id", message.recipient_ext_id);
//            param.Add("@recipient_name", message.recipient_name);
//            param.Add("@recipient_avatar", message.recipient_avatar);
//            param.Add("@author", message.author);
//            param.Add("@customer_id", message.customer_id);
//            param.Add("@type", message.type);
//            param.Add("@read", message.read);
//            param.Add("@hidden", message.hidden);
//            param.Add("@liked", message.liked);
//            param.Add("@starred", message.starred);
//            param.Add("@timestamp", message.timestamp);
//            param.Add("@replied", message.replied);
//            param.Add("@replied_at", message.replied_at);
//            param.Add("@channel_id", message.channel_id);
//            param.Add("@channel_ext_id", message.channel_ext_id);
//            param.Add("@channel_type", message.channel_type);
//            param.Add("@created_time", message.created_time);
//            param.Add("@updated_time", message.updated_time <= DateTime.MinValue ? null : message.updated_time);


//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }

//            return rowsAffected > 0;
//        }


//        public bool Update(Message message)
//        {
//            var query = @"UPDATE [Messages] SET 
//                           [ext_id] = @ext_id
//                          ,[parent_id] = @parent_id
//                          ,[parent_ext_id] = @parent_ext_id
//                          ,[root_ext_id] = @root_ext_id
//                          ,[url] = @url
//                          ,[urls] = @urls
//                          ,[tag] = @tag
//                          ,[template] = @template
//                          ,[file_name] = @file_name
//                          ,[size] = @size
//                          ,[subject] = @subject
//                          ,[message] = @message
//                          ,[agent_id] = @agent_id
//                          ,[thread_id] = @thread_id
//                          ,[thread_type] = @thread_type
//                          ,[conversation_ext_id] = @conversation_ext_id
//                          ,[sender_id] = @sender_id
//                          ,[sender_ext_id] = @sender_ext_id
//                          ,[sender_name] = @sender_name
//                          ,[sender_avatar] = @sender_avatar
//                          ,[recipient_id] = @recipient_id
//                          ,[recipient_ext_id] = @recipient_ext_id
//                          ,[recipient_name] = @recipient_name
//                          ,[recipient_avatar] = @recipient_avatar
//                          ,[author] = @author
//                          ,[customer_id] = @customer_id
//                          ,[type] = @type
//                          ,[read] = @read
//                          ,[hidden] = @hidden
//                          ,[liked] = @liked
//                          ,[starred] = @starred
//                          ,[timestamp] = @timestamp
//                          ,[replied] = @replied
//                          ,[replied_at] = @replied_at
//                          ,[channel_id] = @channel_id
//                          ,[channel_ext_id] = @channel_ext_id
//                          ,[channel_type] = @channel_type
//                          ,[created_time] = @created_time
//                          ,[updated_time] = @updated_time
//                           WHERE [id] = @id and [business_id] = @business_id";

//            var param = new DynamicParameters();
//            param.Add("@id", message.id);
//            param.Add("@parent_id", message.parent_id);
//            param.Add("@parent_ext_id", message.parent_ext_id);
//            param.Add("@root_ext_id", message.root_ext_id);
//            param.Add("@business_id", message.business_id);
//            param.Add("@ext_id", message.ext_id);
//            param.Add("@url", message.url);
//            param.Add("@urls", message.urls);
//            param.Add("@tag", message.tag);
//            param.Add("@template", message.template);
//            param.Add("@file_name", message.file_name);
//            param.Add("@size", message.size);
//            param.Add("@subject", message.subject);
//            param.Add("@message", message.message);
//            param.Add("@agent_id", message.agent_id);
//            param.Add("@thread_id", message.thread_id);
//            param.Add("@thread_type", message.thread_type);
//            param.Add("@conversation_ext_id", message.conversation_ext_id);
//            param.Add("@sender_id", message.sender_id);
//            param.Add("@sender_ext_id", message.sender_ext_id);
//            param.Add("@sender_name", message.sender_name);
//            param.Add("@sender_avatar", message.sender_avatar);
//            param.Add("@recipient_id", message.recipient_id);
//            param.Add("@recipient_ext_id", message.recipient_ext_id);
//            param.Add("@recipient_name", message.recipient_name);
//            param.Add("@recipient_avatar", message.recipient_avatar);
//            param.Add("@author", message.author);
//            param.Add("@customer_id", message.customer_id);
//            param.Add("@type", message.type);
//            param.Add("@read", message.read);
//            param.Add("@hidden", message.hidden);
//            param.Add("@liked", message.liked);
//            param.Add("@starred", message.starred);
//            param.Add("@timestamp", message.timestamp);
//            param.Add("@replied", message.replied);
//            param.Add("@replied_at", message.replied_at);
//            param.Add("@channel_id", message.channel_id);
//            param.Add("@channel_ext_id", message.channel_ext_id);
//            param.Add("@channel_type", message.channel_type);
//            param.Add("@created_time", message.created_time);
//            param.Add("@updated_time", message.updated_time <= DateTime.MinValue ?  null : message.updated_time);


//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }

//            return rowsAffected > 0;
//        }

//        public void Upsert(Message message)
//        {
//            if (!Update(message)) Insert(message);
//        }

//        public bool MarkAsDeleted(string business_id, string id)
//        {
//            var query = @"UPDATE Messages SET deleted = 1 WHERE  business_id=@business_id and [id]=@id";
//            var param = new DynamicParameters();
//            param.Add("@business_id", business_id);
//            param.Add("@id", id);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }

//        public bool MarkAsReplied(string business_id, string id, long replied_at)
//        {
//            var query = @"UPDATE Messages SET replied=1, replied_at=@replied_at 
//                        WHERE  business_id=@business_id and [id]=@id";
//            var param = new DynamicParameters();
//            param.Add("@business_id", business_id);
//            param.Add("@id", id);
//            param.Add("@replied_at", replied_at);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }

//        public int UpdateCustomerId()
//        {
//            var query = @"UPDATE Messages SET   Messages.customer_id = Threads.customer_id
//                        FROM   Messages  INNER JOIN Threads ON Messages.thread_id = Threads.id
//                        WHERE  Messages.customer_id is null";
//            var param = new DynamicParameters();
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected;
//        }
//    }
//}
