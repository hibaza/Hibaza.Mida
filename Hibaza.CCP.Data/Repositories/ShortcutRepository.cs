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
//    public class ShortcutRepository : IShortcutRepository
//    {
//        IConnectionFactory _connectionFactory;
//        private const string SHORTCUTS = "shortcuts";
//        public ShortcutRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public Shortcut GetById(string business_id, string id)
//        {
//            Shortcut shortcut;
//            var key = "Shortcut_GetById" + business_id + id;
//            var rs =  CacheBase.cacheManagerGet<Shortcut>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Shortcuts"
//                               + " WHERE id=@id and business_id=@business_id";
//                shortcut = dbConnection.Query<Shortcut>(sQuery, new { business_id, id }).FirstOrDefault();
//                CacheBase.cacheManagerSet(key, shortcut, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return shortcut;
//        }


//        public void Upsert(Shortcut shortcut)
//        {
//            if (!Update(shortcut)) Insert(shortcut);
//        }

//        public bool Delete(string business_id, string id)
//        {
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "DELETE FROM Shortcuts"
//                             + " WHERE id=@id and business_id=@business_id";
//                dbConnection.Execute(sQuery, new { id, business_id });
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { id });
//            return true;
//        }

//        public async Task<IEnumerable<Shortcut>> GetAll(string business_id, Paging page)
//        {
//            IEnumerable<Shortcut> list;
//            var key = "Shortcut_GetAll" + business_id;
//            var rs = await CacheBase.cacheManagerGet<IEnumerable<Shortcut>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Shortcuts WHERE business_id=@business_id ORDER BY created_time DESC";
//                list = await dbConnection.QueryAsync<Shortcut>(sQuery, new { business_id, limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }

//        public bool Update(Shortcut shortcut)
//        {
//            var query = @"UPDATE [Shortcuts] SET
//                   [shortcut] = @shortcut
//                  ,[name] = @name
//                  ,[created_by] = @created_by
//                  ,[created_time] = @created_time
//                  ,[updated_time] = @updated_time
//             WHERE id=@id and business_id=@business_id";

//            var param = new DynamicParameters();
//            param.Add("@id", shortcut.id);
//            param.Add("@business_id", shortcut.business_id);
//            param.Add("@shortcut", shortcut.shortcut);
//            param.Add("@name", shortcut.name);
//            param.Add("@created_by", shortcut.created_by);
//            param.Add("@created_time", shortcut.created_time);
//            param.Add("@updated_time", shortcut.updated_time <= DateTime.MinValue ? null : shortcut.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() {shortcut.id });
//            return rowsAffected > 0;
//        }

//        public bool Insert(Shortcut shortcut)
//        {
//            var query = @"INSERT INTO [Shortcuts]
//           ([id]
//           ,[business_id]
//           ,[shortcut]
//           ,[name]
//           ,[created_by]
//           ,[created_time]
//           ,[updated_time])
//     VALUES
//           (@id
//           ,@business_id
//           ,@shortcut
//           ,@name
//           ,@created_by
//           ,@created_time
//           ,@updated_time)";

//            var param = new DynamicParameters();
//            param.Add("@id", shortcut.id);
//            param.Add("@business_id", shortcut.business_id);
//            param.Add("@shortcut", shortcut.shortcut);
//            param.Add("@name", shortcut.name);
//            param.Add("@created_by", shortcut.created_by);
//            param.Add("@created_time", shortcut.created_time);
//            param.Add("@updated_time", shortcut.updated_time <= DateTime.MinValue ? null : shortcut.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() {shortcut.id });
//            return rowsAffected > 0;
//        }
//    }
//}
