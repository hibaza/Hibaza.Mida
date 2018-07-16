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
//    public class ReferralRepository : IReferralRepository
//    {
//        IConnectionFactory _connectionFactory;
//        public static string REFERRALS = "referrals";

//        public ReferralRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public Referral GetById(string business_id, string id)
//        {
//            Referral referral;
//            var key = "Referral_GetById" + business_id + id;
//            var rs = CacheBase.cacheManagerGet<Referral>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Referrals"
//                               + " WHERE id=@id and business_id=@business_id";
//                referral = dbConnection.Query<Referral>(sQuery, new { business_id, id }).FirstOrDefault();
//                CacheBase.cacheManagerSet(key, referral, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return referral;
//        }

//        public int UpdateCustomerId()
//        {
//            var query = @"UPDATE Referrals SET   Referrals.customer_id = Threads.customer_id
//                        FROM   Referrals  INNER JOIN Threads ON Referrals.thread_id = Threads.id
//                        WHERE  Referrals.customer_id != Threads.customer_id";
//            var param = new DynamicParameters();
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected;
//        }


//        public void Upsert(Referral referral)
//        {
//            if (!Update(referral)) Insert(referral);
//        }

//        public bool Delete(string business_id, string id)
//        {
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "DELETE FROM Referrals"
//                             + " WHERE id=@id and business_id=@business_id";
//                dbConnection.Execute(sQuery, new { id, business_id });
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { id });
//            return true;
//        }

//        public async Task<IEnumerable<Referral>> GetAll(string business_id, Paging page)
//        {
//            IEnumerable<Referral> list;
//            var key = "Referral_GetAll" + business_id;
//            var rs = await CacheBase.cacheManagerGet<IEnumerable<Referral>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Referrals WHERE business_id=@business_id ORDER BY created_time DESC";
//                list = await dbConnection.QueryAsync<Referral>(sQuery, new { business_id, limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }

//        public bool Update(Referral referral)
//        {
//            var query = @"UPDATE [Referrals] SET 
//                   [thread_id] = @thread_id
//                  ,[customer_id] = @customer_id
//                  ,[product_id] = @product_id
//                  ,[product_sku] = @product_sku
//                  ,[product_url] = @product_url
//                  ,[product_photo_url] = @product_photo_url
//                  ,[type] = @type
//                  ,[data] = @data
//                  ,[sender_ext_id] = @sender_ext_id
//                  ,[recipient_ext_id] = @recipient_ext_id
//                  ,[timestamp] = @timestamp
//                  ,[created_time] = @created_time
//                  ,[updated_time] = @updated_time 
//             WHERE id=@id and business_id=@business_id";

//            var param = new DynamicParameters();
//            param.Add("@id", referral.id);
//            param.Add("@business_id", referral.business_id);
//            param.Add("@thread_id", referral.thread_id);
//            param.Add("@customer_id", referral.customer_id);
//            param.Add("@product_id", referral.product_id);
//            param.Add("@product_sku", referral.product_sku);
//            param.Add("@product_url", referral.product_url);
//            param.Add("@product_photo_url", referral.product_photo_url);
//            param.Add("@type", referral.type);
//            param.Add("@data", referral.data);
//            param.Add("@sender_ext_id", referral.sender_ext_id);
//            param.Add("@recipient_ext_id", referral.recipient_ext_id);
//            param.Add("@timestamp", referral.timestamp);
//            param.Add("@created_time", referral.created_time);
//            param.Add("@updated_time", referral.updated_time <= DateTime.MinValue ? null : referral.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() {referral.id });
//            return rowsAffected > 0;
//        }

//        public bool Insert(Referral referral)
//        {
//            var query = @"INSERT INTO [Referrals]
//           ([id]
//           ,[business_id]
//           ,[thread_id]
//           ,[customer_id]
//           ,[product_id]
//           ,[product_sku]
//           ,[product_url]
//           ,[product_photo_url]
//           ,[type]
//           ,[data]
//           ,[sender_ext_id]
//           ,[recipient_ext_id]
//           ,[timestamp]
//           ,[created_time]
//           ,[updated_time])
//     VALUES
//           (@id
//           ,@business_id
//           ,@thread_id
//           ,@customer_id
//           ,@product_id
//           ,@product_sku
//           ,@product_url
//           ,@product_photo_url
//           ,@type
//           ,@data
//           ,@sender_ext_id
//           ,@recipient_ext_id
//           ,@timestamp
//           ,@created_time
//           ,@updated_time)";

//            var param = new DynamicParameters();
//            param.Add("@id", referral.id);
//            param.Add("@business_id", referral.business_id);
//            param.Add("@thread_id", referral.thread_id);
//            param.Add("@customer_id", referral.customer_id);
//            param.Add("@product_id", referral.product_id);
//            param.Add("@product_sku", referral.product_sku);
//            param.Add("@product_url", referral.product_url);
//            param.Add("@product_photo_url", referral.product_photo_url);
//            param.Add("@type", referral.type);
//            param.Add("@data", referral.data);
//            param.Add("@sender_ext_id", referral.sender_ext_id);
//            param.Add("@recipient_ext_id", referral.recipient_ext_id);
//            param.Add("@timestamp", referral.timestamp);
//            param.Add("@created_time", referral.created_time);
//            param.Add("@updated_time", referral.updated_time <= DateTime.MinValue ? null : referral.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() {referral.id });
//            return rowsAffected > 0;
//        }

//        public async Task<IEnumerable<Referral>> GetReferrals(string business_id, string thread_id, Paging page)
//        {
//            IEnumerable<Referral> list;
//            var key = "GetReferrals" + business_id;
//            var rs = await CacheBase.cacheManagerGet<IEnumerable<Referral>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Referrals WHERE business_id=@business_id and thread_id=@thread_id ORDER BY created_time DESC";
//                list = await dbConnection.QueryAsync<Referral>(sQuery, new { business_id, thread_id, limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Referral>> GetReferralsByCustomer(string business_id, string customer_id, Paging page)
//        {
//            IEnumerable<Referral> list;
//            var key = "GetReferralsByCustomer" + business_id+ customer_id;
//            var rs = await CacheBase.cacheManagerGet<IEnumerable<Referral>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Referrals WHERE business_id=@business_id and customer_id=@customer_id ORDER BY created_time DESC";
//                list = await dbConnection.QueryAsync<Referral>(sQuery, new { business_id, customer_id, limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }
//    }
//}
