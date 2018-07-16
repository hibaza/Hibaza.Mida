//using Dapper;
//using Firebase.Database.Query;
//using Hibaza.CCP.Data.Providers.Firebase;
//using Hibaza.CCP.Data.Providers.Mongo;
//using Hibaza.CCP.Data.Providers.SQLServer;
//using Hibaza.CCP.Domain.Entities;
//using Hibaza.CCP.Domain.Models;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Hibaza.CCP.Data.Repositories
//{
//    public class TicketRepository : ITicketRepository
//    {
//        IConnectionFactory _connectionFactory;
//        private const string TICKETS = "tickets";
//        public TicketRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public Ticket GetById(string id)
//        {

//            Ticket ticket;
//            var key = "Ticket_GetById"  + id;
//            var rs = CacheBase.cacheManagerGet<Ticket>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Tickets"
//                               + " WHERE id=@id";
//                ticket = dbConnection.Query<Ticket>(sQuery, new { id }).FirstOrDefault();
//                CacheBase.cacheManagerSet(key, ticket, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return ticket;
//        }

//        public Ticket GetById(string business_id, string id)
//        {
//            Ticket ticket;
//            var key = "Ticket_GetById01" + business_id+ id;
//            var rs = CacheBase.cacheManagerGet<Ticket>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Tickets"
//                               + " WHERE id=@id and business_id=@business_id";
//                ticket = dbConnection.Query<Ticket>(sQuery, new { business_id, id }).FirstOrDefault();
//                CacheBase.cacheManagerSet(key, ticket, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return ticket;
//        }



//        public int UpdateCustomerId()
//        {
//            var query = @"UPDATE Tickets SET Tickets.customer_id = Threads.customer_id
//                        FROM   Tickets  INNER JOIN Threads ON Tickets.thread_id = Threads.id
//                        WHERE Tickets.customer_id is null";
//            var param = new DynamicParameters();
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }

//            return rowsAffected;
//        }

//        public async Task<Ticket> GetCustomerLastTicket(string business_id, string customer_id, int status)
//        {

//            IEnumerable<Ticket> list = await GetCustomerTickets(business_id, customer_id, status, new Paging { Limit = 1 });
//            if (list!=null && list.Count() > 0)
//            {
//                return list.First();
//            }
//            return null;
//        }

//        public async Task<IEnumerable<Ticket>> GetCustomerTickets(string business_id, string customer_id, int status, Paging page)
//        {

//            IEnumerable<Ticket> list;
//            var key = "GetCustomerTickets" + business_id+ customer_id + status;
//            var rs = await CacheBase.cacheManagerGet<IEnumerable<Ticket>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Tickets WHERE business_id=@business_id and customer_id=@customer_id and (status=@status or @status < 0 )ORDER BY created_time DESC";
//                list = await dbConnection.QueryAsync<Ticket>(sQuery, new { business_id, customer_id, status, limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Ticket>> GetTickets(string business_id, int status, Paging page)
//        {

//            IEnumerable<Ticket> list;
//            var key = "GetTickets" + business_id  + status;
//            var rs = await CacheBase.cacheManagerGet<IEnumerable<Ticket>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Tickets WHERE business_id=@business_id and created_time>=@start and created_time<=@end and (status=@status or @status < 0) ORDER BY created_time DESC";
//                list = await dbConnection.QueryAsync<Ticket>(sQuery, new { business_id, status, start = DateTime.Parse(page.Previous), end = DateTime.Parse(page.Next), limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                  null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }

//        public void Upsert(Ticket ticket)
//        {
//            if (!Update(ticket)) Insert(ticket);
//        }

//        public bool Delete(string business_id, string id)
//        {
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "DELETE FROM Tickets"
//                             + " WHERE id=@id and business_id=@business_id";
//                dbConnection.Execute(sQuery, new { id, business_id });
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { id });
//            return true;
//        }

//        public async Task<IEnumerable<Ticket>> GetAll(string business_id, Paging page)
//        {
//            IEnumerable<Ticket> list;
//            var key = "Ticket_GetAll" + business_id ;
//            var rs = await CacheBase.cacheManagerGet<IEnumerable<Ticket>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Tickets WHERE business_id=@business_id ORDER BY created_time DESC";
//                list = await dbConnection.QueryAsync<Ticket>(sQuery, new { business_id, limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }

//        public bool Update(Ticket ticket)
//        {
//            var query = @"UPDATE [Tickets] SET
//                  customer_id = @customer_id
//                  ,type = @type
//                  ,short_description = @short_description
//                  ,description = @description
//                  ,channel_id = @channel_id
//                  ,thread_id = @thread_id
//                  ,customer_name = @customer_name
//                  ,status = @status
//                  ,tags = @tags
//                  ,sender_id = @sender_id
//                  ,sender_name = @sender_name
//                  ,timestamp = @timestamp
//                  ,created_time = @created_time
//                  ,updated_time = @updated_time
//             WHERE id=@id";

//            var param = new DynamicParameters();
//            param.Add("@id", ticket.id);
//            param.Add("@business_id", ticket.business_id);
//            param.Add("@customer_id", ticket.customer_id);
//            param.Add("@type", ticket.type);
//            param.Add("@short_description", ticket.short_description);
//            param.Add("@description", ticket.description);
//            param.Add("@channel_id", ticket.channel_id);
//            param.Add("@thread_id", ticket.thread_id);
//            param.Add("@customer_name", ticket.customer_name);
//            param.Add("@status", ticket.status);
//            param.Add("@tags", ticket.tags);
//            param.Add("@sender_id", ticket.sender_id);
//            param.Add("@sender_name", ticket.sender_name);
//            param.Add("@timestamp", ticket.timestamp);
//            param.Add("@created_time", ticket.created_time);
//            param.Add("@updated_time", ticket.updated_time <= DateTime.MinValue ? null : ticket.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() {ticket.id ,ticket.customer_id });

//            return rowsAffected > 0;
//        }

//        public bool Insert(Ticket ticket)
//        {
//            var query = @"INSERT INTO [Tickets]
//           ([id]
//           ,[business_id]
//           ,[customer_id]
//           ,[type]
//           ,[short_description]
//           ,[description]
//           ,[channel_id]
//           ,[thread_id]
//           ,[customer_name]
//           ,[status]
//           ,[tags]
//           ,[sender_id]
//           ,[sender_name]
//           ,[timestamp]
//           ,[created_time]
//           ,[updated_time])
//     VALUES
//           (@id
//           ,@business_id
//           ,@customer_id
//           ,@type
//           ,@short_description
//           ,@description
//           ,@channel_id 
//           ,@thread_id
//           ,@customer_name
//           ,@status
//           ,@tags
//           ,@sender_id
//           ,@sender_name
//           ,@timestamp
//           ,@created_time
//           ,@updated_time)";

//            var param = new DynamicParameters();
//            param.Add("@id", ticket.id);
//            param.Add("@business_id", ticket.business_id);
//            param.Add("@customer_id", ticket.customer_id);
//            param.Add("@type", ticket.type);
//            param.Add("@short_description", ticket.short_description);
//            param.Add("@description", ticket.description);
//            param.Add("@channel_id", ticket.channel_id);
//            param.Add("@thread_id", ticket.thread_id);
//            param.Add("@customer_name", ticket.customer_name);
//            param.Add("@status", ticket.status);
//            param.Add("@tags", ticket.tags);
//            param.Add("@sender_id", ticket.sender_id);
//            param.Add("@sender_name", ticket.sender_name);
//            param.Add("@timestamp", ticket.timestamp);
//            param.Add("@created_time", ticket.created_time);
//            param.Add("@updated_time", ticket.updated_time <= DateTime.MinValue ? null : ticket.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { ticket.id, ticket.customer_id });
//            return rowsAffected > 0;
//        }
//    }
//}
