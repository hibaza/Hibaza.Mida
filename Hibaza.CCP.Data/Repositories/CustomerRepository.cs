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
//    public class CustomerRepository : ICustomerRepository
//    {
//        IConnectionFactory _connectionFactory;

//        public CustomerRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public async Task<IEnumerable<Counter>> GetChannelCounters(string business_id)
//        {
//            //IEnumerable<Counter> list = null;
//            //var param = new DynamicParameters();
//            //using (var dbConnection = _connectionFactory.GetConnection)
//            //{
//            //    string sQuery = "SELECT channel_id as id, SUM(CASE WHEN unread > 0 THEN 1 ELSE 0 END) as unread,  COUNT(*) as count FROM Customers"
//            //                   + " WHERE business_id=@business_id GROUP BY channel_id";
//            //    list = await dbConnection.QueryAsync<Counter>(sQuery, new { business_id });

//            //}
//            //return list;

//            IEnumerable<Counter> list = null;
//            var param = new DynamicParameters();
//            param.Add("@business_id", business_id);
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                var query = "dbo.GetChannelUnreadCounters";
//                list = await dbConnection.QueryAsync<Counter>(query, param, commandType: CommandType.StoredProcedure);
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Counter>> GetAgentCounters(string business_id)
//        {
//            //IEnumerable<Counter> list = null;
//            //using (var dbConnection = _connectionFactory.GetConnection)
//            //{
//            //    string sQuery = "SELECT agent_id as id, SUM(CASE WHEN unread > 0 THEN 1 ELSE 0 END) as unread,  COUNT(*) as count FROM Customers"
//            //                   + " WHERE business_id=@business_id GROUP BY agent_id";

//            //    list = await dbConnection.QueryAsync<Counter>(sQuery, new { business_id });
//            //}
//            //return list;

//            IEnumerable<Counter> list = null;
//            var param = new DynamicParameters();
//            param.Add("@business_id", business_id);
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                var query = "dbo.GetAgentUnreadCounters";
//                list = await dbConnection.QueryAsync<Counter>(query, param, commandType: CommandType.StoredProcedure);
//            }
//            return list;
//        }

//        public bool UpdateContactInfo(string business_id, string customer_id, CustomerContactInfoModel data)
//        {
//            var query = @"UPDATE [Customers] SET 
//              [phone_list]=@phone_list, [phone]=@phone, [email]=@email, [sex]=@sex, [blocked]=@blocked,
//                [name]=@name, [city]=@city, [address]=@address, [zipcode]=@zipcode, [birthdate]=@birthdate
//              WHERE [id]=@id and business_id=@business_id";
//            var param = new DynamicParameters();
//            param.Add("@business_id", business_id);
//            param.Add("@id", customer_id);
//            param.Add("@name", data.name);
//            param.Add("@phone", data.phone);
//            param.Add("@phone_list", data.phone_list);
//            param.Add("@city", data.city);
//            param.Add("@address", data.address);
//            param.Add("@birthdate", data.birthdate);
//            param.Add("@email", data.email);
//            param.Add("@blocked", data.blocked);
//            param.Add("@zipcode", data.zipcode);
//            param.Add("@sex", data.sex);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }

//        public bool UpdatePhoneNumber(string business_id, string customer_id, string phone_list, string phone)
//        {
//            var query = @"UPDATE [Customers] SET 
//              [phone_list] = @phone_list, [phone]=@phone
//              WHERE [id]=@customer_id and business_id=@business_id";
//            var param = new DynamicParameters();
//            param.Add("@phone_list", phone_list);
//            param.Add("@customer_id", customer_id);
//            param.Add("@business_id", business_id);
//            param.Add("@phone", phone);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }

//        public bool Block(string business_id, string customer_id, bool blocked)
//        {
//            var query = @"UPDATE [Customers] SET 
//              [blocked] = @blocked
//              WHERE [id]=@customer_id and business_id=@business_id";
//            var param = new DynamicParameters();
//            param.Add("@customer_id", customer_id);
//            param.Add("@business_id", business_id);
//            param.Add("@blocked", blocked);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }

//        public bool UpdateUserId(string business_id, int key, string user_id)
//        {
//            var query = @"UPDATE [Customers] SET 
//              [id] = @user_id, global_id=@user_id
//              WHERE [key]=@key and business_id=@business_id and global_id is null";
//            var param = new DynamicParameters();
//            param.Add("@user_id", user_id);
//            param.Add("@business_id", business_id);
//            param.Add("@key", key);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }

//        public void Upsert(Customer customer)
//        {
//            if (!Update(customer)) Insert(customer);
//        }

//        public bool Update(Customer customer)
//        {
//            var query = @"UPDATE [Customers]
//                           SET [id] = @id
//                              ,[business_id] = @business_id
//                              ,[channel_id] = @channel_id
//                              ,[created_time] = @created_time
//                              ,[updated_time] = @updated_time
//                              ,[timestamp] = @timestamp
//                              ,[ext_id] = @ext_id
//                              ,[app_id] = @app_id
//                              ,[global_id] = @global_id
//                              ,[first_name] = @first_name
//                              ,[last_name] = @last_name
//                              ,[name] = @name
//                              ,[email] = @email
//                              ,[avatar] = @avatar
//                              ,[phone] = @phone
//                              ,[phone_list] = @phone_list
//                              ,[city] = @city
//                              ,[zipcode] = @zipcode
//                              ,[blocked] = @blocked
//                              ,[birthdate] = @birthdate
//                              ,[sex] = @sex
//                              ,[archived] = @archived
//                              ,[status] = @status
//                              ,[business_name] = @business_name
//                              ,[agent_id] = @agent_id
//                              ,[assigned_by] = @assigned_by
//                              ,[assigned_at] = @assigned_at
//                              ,[active_thread] = @active_thread
//                              ,[active_ticket] = @active_ticket
//                              ,[open] = @open
//                              ,[unread] = @unread
//                              ,[nonreply] = @nonreply
//                            WHERE id=@id and business_id=@business_id";
//            var param = new DynamicParameters();
//            param.Add("@id", customer.id);
//            param.Add("@business_id", customer.business_id);
//            param.Add("@created_time", customer.created_time);
//            param.Add("@updated_time", customer.updated_time <= DateTime.MinValue ? null : customer.updated_time);
//            param.Add("@channel_id", customer.channel_id);
//            param.Add("@timestamp", customer.timestamp);
//            param.Add("@ext_id", customer.ext_id);
//            param.Add("@app_id", customer.app_id);
//            param.Add("@global_id", customer.global_id);
//            param.Add("@first_name", customer.first_name);
//            param.Add("@last_name", customer.last_name);
//            param.Add("@name", customer.name);
//            param.Add("@email", customer.email);
//            param.Add("@avatar", customer.avatar);
//            param.Add("@phone", customer.phone);
//            param.Add("@phone_list", customer.phone_list);
//            param.Add("@city", customer.city);
//            param.Add("@zipcode", customer.zipcode);
//            param.Add("@blocked", customer.blocked);
//            param.Add("@birthdate", customer.birthdate <= DateTime.MinValue ? null : customer.birthdate);
//            param.Add("@sex", customer.sex);
//            param.Add("@archived", customer.archived);
//            param.Add("@status", customer.status);
//            param.Add("@unread", customer.unread);
//            param.Add("@nonreply", customer.nonreply);
//            param.Add("@agent_id", customer.agent_id);
//            param.Add("@assigned_by", customer.assigned_by);
//            param.Add("@assigned_at", customer.assigned_at <= DateTime.MinValue ? null : customer.assigned_at);
//            param.Add("@active_thread", customer.active_thread);
//            param.Add("@active_ticket", customer.active_ticket);
//            param.Add("@open", customer.open);
//            param.Add("@business_name", customer.business_name);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }


//        public bool Insert(Customer customer)
//        {
//            var query = @"INSERT INTO [Customers]
//           ([id]
//           ,[business_id]
//           ,[channel_id]
//           ,[created_time]
//           ,[updated_time]
//           ,[timestamp]
//           ,[ext_id]
//           ,[app_id]
//           ,[global_id]
//           ,[first_name]
//           ,[last_name]
//           ,[name]
//           ,[email]
//           ,[avatar]
//           ,[phone]
//           ,[phone_list]
//           ,[city]
//           ,[zipcode]
//           ,[blocked]
//           ,[birthdate]
//           ,[sex]
//           ,[archived]
//           ,[status]
//           ,[business_name]
//           ,[agent_id]
//           ,[assigned_by]
//           ,[assigned_at]
//           ,[active_thread]
//           ,[active_ticket]
//           ,[open]
//           ,[unread]
//           ,[nonreply])
//     VALUES
//           (@id
//           ,@business_id
//           ,@channel_id
//           ,@created_time
//           ,@updated_time
//           ,@timestamp
//           ,@ext_id
//           ,@app_id
//           ,@global_id
//           ,@first_name
//           ,@last_name
//           ,@name
//           ,@email
//           ,@avatar
//           ,@phone
//           ,@phone_list
//           ,@city
//           ,@zipcode
//           ,@blocked
//           ,@birthdate
//           ,@sex
//           ,@archived
//           ,@status
//           ,@business_name
//           ,@agent_id
//           ,@assigned_by
//           ,@assigned_at
//           ,@active_thread
//           ,@active_ticket
//           ,@open
//           ,@unread
//           ,@nonreply)";
//            var param = new DynamicParameters();
//            param.Add("@id", customer.id);
//            param.Add("@business_id", customer.business_id);
//            param.Add("@created_time", customer.created_time);
//            param.Add("@updated_time", customer.updated_time <= DateTime.MinValue ? null : customer.updated_time);
//            param.Add("@channel_id", customer.channel_id);
//            param.Add("@timestamp", customer.timestamp);
//            param.Add("@ext_id", customer.ext_id);
//            param.Add("@app_id", customer.app_id);
//            param.Add("@global_id", customer.global_id);
//            param.Add("@first_name", customer.first_name);
//            param.Add("@last_name", customer.last_name);
//            param.Add("@name", customer.name);
//            param.Add("@email", customer.email);
//            param.Add("@avatar", customer.avatar);
//            param.Add("@phone", customer.phone);
//            param.Add("@phone_list", customer.phone_list);
//            param.Add("@city", customer.city);
//            param.Add("@zipcode", customer.zipcode);
//            param.Add("@blocked", customer.blocked);
//            param.Add("@birthdate", customer.birthdate);
//            param.Add("@sex", customer.sex);
//            param.Add("@archived", customer.archived);
//            param.Add("@status", customer.status);
//            param.Add("@unread", customer.unread);
//            param.Add("@nonreply", customer.nonreply);
//            param.Add("@agent_id", customer.agent_id);
//            param.Add("@assigned_by", customer.assigned_by);
//            param.Add("@assigned_at", customer.assigned_at <= DateTime.MinValue ? null : customer.assigned_at);
//            param.Add("@active_thread", customer.active_thread);
//            param.Add("@active_ticket", customer.active_ticket);
//            param.Add("@open", customer.open);
//            param.Add("@business_name", customer.business_name);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected > 0;
//        }


//        public bool Delete(string business_id, string id)
//        {
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "DELETE FROM Customers"
//                             + " WHERE id=@id and business_id=@business_id";
//                dbConnection.Execute(sQuery, new { id, business_id });
//            }
//            return true;
//        }

//        public async Task<IEnumerable<Customer>> GetActiveUnreadCustomers(string business_id, string agent_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.agent_id=@agent_id and t.status='active'  and t.unread > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, agent_id, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetActiveUnreadCustomers(string business_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.status='active'  and t.unread > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, limit = page.Limit });
//            }
//            return list;
//        }


//        public async Task<IEnumerable<Customer>> GetPendingUnreadCustomers(string business_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.status='pending'  and t.unread > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetPendingUnreadCustomers(string business_id, string channel_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.channel_id=@channel_id and t.status='pending'  and t.unread > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, channel_id, limit = page.Limit });
//            }
//            return list;
//        }

//        public IEnumerable<Customer> GetAll(string business_id)
//        {
//            IEnumerable<Customer> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Customers"
//                               + " WHERE business_id = @business_id";
//                list = dbConnection.Query<Customer>(sQuery, new { business_id });
//            }
//            return list;
//        }

//        public Customer GetById(string business_id, string id)
//        {
//            Customer customer;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Customers"
//                               + " WHERE business_id=@business_id and id=@id";
//                customer = dbConnection.Query<Customer>(sQuery, new { business_id, id }).FirstOrDefault();
//            }
//            return customer;

//        }


//        public async Task<IEnumerable<Customer>> GetCustomers(string business_id, Paging page)
//        {
//            IEnumerable<Customer> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Customers"
//                               + " WHERE business_id = @business_id";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> SearchCustomersByKeywords(string business_id, string channel_id, string agent_id, string status, string flag, string keywords, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            long end;
//            if (!long.TryParse(page.Next, out end))
//            {
//                end = 9999999999999;
//            }

//            var query = string.IsNullOrEmpty(keywords) ? "dbo.CustomersGetByFilterOrderByDate" : "dbo.CustomersFTSearchByOwnerNameOrderByRankAndDate";
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
//                list = await connection.QueryAsync<Customer>(query, param, commandType: CommandType.StoredProcedure);
//                if ((list == null || list.Count() == 0) && !string.IsNullOrWhiteSpace(keywords))
//                {
//                    query = "dbo.CustomersGetByFilterOrderByDate";
//                    list = await connection.QueryAsync<Customer>(query, param, commandType: CommandType.StoredProcedure);
//                }
//            }

//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetUnreadCustomers(string business_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and [timestamp] <= @end and t.unread > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetUnreadCustomersByChannel(string business_id, string channel_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.channel_id=@channel_id and [timestamp] <= @end and t.unread > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, channel_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetUnreadCustomersByAgent(string business_id, string agent_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.agent_id=@agent_id and [timestamp] <= @end and t.unread > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, agent_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetUnreadCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.channel_id=@channel_id and t.agent_id=@agent_id and [timestamp] <= @end and t.unread > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, channel_id, agent_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetCustomersWhereExtIdIsNull(string business_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and [timestamp] <= @end and (t.ext_id is null  or t.ext_id='')  ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetNonReplyCustomers(string business_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and [timestamp] <= @end and t.nonreply > 0 and t.unread < = 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetNonReplyCustomersByChannel(string business_id, string channel_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.channel_id=@channel_id and [timestamp] <= @end and t.nonreply > 0  and t.unread < = 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, channel_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetNonReplyCustomersByAgent(string business_id, string agent_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.agent_id=@agent_id and [timestamp] <= @end and t.nonreply > 0  and t.unread < = 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, agent_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetNonReplyCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.channel_id=@channel_id and t.agent_id=@agent_id and [timestamp] <= @end and t.nonreply > 0  and t.unread < = 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, channel_id, agent_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetAllCustomers(string business_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and [timestamp] <= @end ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, end = page.Next, limit = page.Limit });
//            }
//            return list;

//        }

//        public async Task<IEnumerable<Customer>> GetOpenCustomers(string business_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and [timestamp] <= @end and t.[open] > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, end = page.Next, limit = page.Limit });
//            }
//            return list;

//        }
//        public async Task<IEnumerable<Customer>> GetAllCustomersByChannel(string business_id, string channel_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.channel_id=@channel_id and [timestamp] <= @end ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, channel_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetOpenCustomersByChannel(string business_id, string channel_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.channel_id=@channel_id and [timestamp] <= @end and t.[open] > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, channel_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetAllCustomersByAgent(string business_id, string agent_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.agent_id=@agent_id and [timestamp] <= @end ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, agent_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetOpenCustomersByAgent(string business_id, string agent_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.agent_id=@agent_id and [timestamp] <= @end and t.[open] > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, agent_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }
//        public async Task<IEnumerable<Customer>> GetAllCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.channel_id=@channel_id and t.agent_id=@agent_id and [timestamp] <= @end ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, channel_id, agent_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Customer>> GetOpenCustomersByChannelAndAgent(string business_id, string channel_id, string agent_id, Paging page)
//        {
//            IEnumerable<Customer> list = null;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) t.* FROM Customers t"
//                               + " WHERE t.business_id=@business_id and t.channel_id=@channel_id and t.agent_id=@agent_id and [timestamp] <= @end and t.open > 0 ORDER BY t.timestamp DESC";
//                list = await dbConnection.QueryAsync<Customer>(sQuery, new { business_id, channel_id, agent_id, end = page.Next, limit = page.Limit });
//            }
//            return list;
//        }
//    }
//}
