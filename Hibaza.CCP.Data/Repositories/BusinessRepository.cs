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
//    public class BusinessRepository : IBusinessRepository
//    {
//        IConnectionFactory _connectionFactory;
//        private const string BUSINESS = "business";
//        public BusinessRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public Business GetById(string id)
//        {
//            Business business;
//            var key = "Business_GetById" + id;
//            var rs = CacheBase.cacheManagerGet<Business>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Businesses"
//                               + " WHERE id=@id";
//                business = dbConnection.Query<Business>(sQuery, new { id }).FirstOrDefault();
//                CacheBase.cacheManagerSet(key, business, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return business;
//        }


//        public IEnumerable<Business> GetAll()
//        {
//            throw new NotImplementedException();
//        }


//        private bool Insert(Business business)
//        {
//            var query = @"INSERT INTO [Businesses]
//           ([id]
//           ,[logo] 
//           ,[type]
//           ,[name]
//           ,[ext_id]
//           ,[token]
//           ,[active]
//           ,[phone]
//           ,[email]
//           ,[address]
//           ,[zip]
//           ,[city]
//           ,[auto_hide]
//           ,[auto_like]
//           ,[auto_message]
//           ,[auto_assign]
//           ,[auto_ticket]
//           ,[country]
//           ,[created_time]
//           ,[updated_time])
//     VALUES
//           (@id
//           ,@logo
//           ,@type
//           ,@name
//           ,@ext_id
//           ,@token
//           ,@active
//           ,@phone
//           ,@email
//           ,@address
//           ,@zip
//           ,@city
//           ,@auto_hide
//           ,@auto_like
//           ,@auto_message
//           ,@auto_assign
//           ,@auto_ticket
//           ,@country
//           ,@created_time
//           ,@updated_time)";

//            var param = new DynamicParameters();
//            param.Add("@id", business.id);
//            param.Add("@logo", business.logo);
//            param.Add("@type", business.type);
//            param.Add("@name", business.name);
//            param.Add("@ext_id", business.ext_id);
//            param.Add("@token", business.token);
//            param.Add("@active", business.active);
//            param.Add("@phone", business.phone);
//            param.Add("@email", business.email);
//            param.Add("@address", business.address);
//            param.Add("@zip", business.zip);
//            param.Add("@city", business.city);
//            param.Add("@auto_hide", business.auto_hide);
//            param.Add("@auto_like", business.auto_like);
//            param.Add("@auto_message", business.auto_message);
//            param.Add("@auto_assign", business.auto_assign);
//            param.Add("@auto_ticket", business.auto_ticket);
//            param.Add("@country", business.country);
//            param.Add("@created_time", business.created_time);
//            param.Add("@updated_time", business.updated_time <= DateTime.MinValue ? null : business.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { business.id });
//            return rowsAffected > 0;
//        }

//        private bool Update(Business business)
//        {
//            var query = @"UPDATE [Businesses]
//           SET [type] = @type
//              ,[logo] = @logo  
//              ,[name] = @name
//              ,[ext_id] = @ext_id
//              ,[token] = @token
//              ,[active] = @active
//              ,[phone] = @phone
//              ,[email] = @email
//              ,[address] = @address
//              ,[zip] = @zip
//              ,[city] = @city
//              ,[country] = @country
//              ,[auto_hide] = @auto_hide
//              ,[auto_like] = @auto_like
//              ,[auto_message] = @auto_message
//              ,[auto_assign] = @auto_assign
//              ,[auto_ticket] = @auto_ticket
//              ,[created_time] = @created_time
//              ,[updated_time] = @updated_time
//            WHERE id=@id";

//            var param = new DynamicParameters();
//            param.Add("@id", business.id);
//            param.Add("@type", business.type);
//            param.Add("@logo", business.logo);
//            param.Add("@name", business.name);
//            param.Add("@ext_id", business.ext_id);
//            param.Add("@token", business.token);
//            param.Add("@active", business.active);
//            param.Add("@phone", business.phone);
//            param.Add("@email", business.email);
//            param.Add("@address", business.address);
//            param.Add("@zip", business.zip);
//            param.Add("@city", business.city);
//            param.Add("@country", business.country);
//            param.Add("@auto_hide", business.auto_hide);
//            param.Add("@auto_like", business.auto_like);
//            param.Add("@auto_message", business.auto_message);
//            param.Add("@auto_assign", business.auto_assign);
//            param.Add("@auto_ticket", business.auto_ticket);
//            param.Add("@created_time", business.created_time);
//            param.Add("@updated_time", business.updated_time <= DateTime.MinValue ? null : business.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { business.id });
//            return rowsAffected > 0;
//        }

//        public void Upsert(Business business)
//        {
//            if (!Update(business)) Insert(business);
//        }

//        public bool Delete(string id)
//        {
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "DELETE FROM businesses"
//                             + " WHERE id=@id";
//                dbConnection.Execute(sQuery, new { id });
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { id });
//            return true;
//        }


//        public IEnumerable<Business> GetBusinesses(Paging page)
//        {
//            IEnumerable<Business> list;
//            var key = "GetBusinesses";
//            var rs = CacheBase.cacheManagerGet<List<Business>>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Businesses ORDER BY created_time DESC";
//                list = dbConnection.Query<Business>(sQuery, new { limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }
//    }
//}
