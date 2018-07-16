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
//    public class NoteRepository : INoteRepository
//    {
//        IConnectionFactory _connectionFactory;
//        private const string NOTES = "notes";
//        public NoteRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public Note GetById(string business_id, string id)
//        {

//            Note note;
//            var key = "Note_GetById" + business_id + id;
//            var rs = CacheBase.cacheManagerGet<Note>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Notes"
//                               + " WHERE id=@id and business_id=@business_id";
//                note = dbConnection.Query<Note>(sQuery, new { business_id, id }).FirstOrDefault();
//                CacheBase.cacheManagerSet(key, note, DateTime.Now.AddMinutes(10),
//                null, null, "", false, new List<string>() { });
//            }
//            return note;
//        }


//        public int UpdateCustomerId()
//        {
//            var query = @"UPDATE Notes SET   Notes.customer_id = Threads.customer_id
//                        FROM   Notes  INNER JOIN Threads ON Notes.thread_id = Threads.id
//                        WHERE  Notes.customer_id is null";
//            var param = new DynamicParameters();
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            return rowsAffected;
//        }


//        public async Task<IEnumerable<Note>> GetCustomerNotes(string business_id, string customer_id, Paging page)
//        {

//            IEnumerable<Note> list;
//            var key = "GetCustomerNotes" + business_id+ customer_id;
//            var rs = await CacheBase.cacheManagerGet<List<Note>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Notes WHERE business_id=@business_id and customer_id=@customer_id ORDER BY created_time DESC";
//                list = await dbConnection.QueryAsync<Note>(sQuery, new { business_id, customer_id, limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }


//        public void Upsert(Note note)
//        {
//            if (!Update(note)) Insert(note);
//        }

//        public bool Delete(string business_id, string id)
//        {
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "DELETE FROM Notes"
//                             + " WHERE id=@id and business_id=@business_id";
//                dbConnection.Execute(sQuery, new { id, business_id });
//                CacheBase.cacheDeleteCacheSame(new List<string>() { id });
//            }
//            return true;
//        }

//        public async Task<IEnumerable<Note>> GetAll(string business_id, Paging page)
//        {
//            IEnumerable<Note> list;
//            var key = "Note_GetAll" + business_id ;
//            var rs = await CacheBase.cacheManagerGet<List<Note>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Notes WHERE business_id=@business_id ORDER BY created_time DESC";
//                list = await dbConnection.QueryAsync<Note>(sQuery, new { business_id, limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }

//        public bool Update(Note note)
//        {
//            var query = @"UPDATE [Notes] SET
//                  customer_id = @customer_id
//                  ,type = @type
//                  ,text = @text
//                  ,thread_id = @thread_id
//                  ,customer_name = @customer_name
//                  ,featured = @featured
//                  ,sender_id = @sender_id
//                  ,sender_name = @sender_name
//                  ,sender_avatar = @sender_avatar
//                  ,created_time = @created_time
//                  ,updated_time = @updated_time
//             WHERE id=@id and business_id=@business_id";

//            var param = new DynamicParameters();
//            param.Add("@id", note.id);
//            param.Add("@business_id", note.business_id);
//            param.Add("@customer_id", note.customer_id);
//            param.Add("@type", note.type);
//            param.Add("@text", note.text);
//            param.Add("@thread_id", note.thread_id);
//            param.Add("@customer_name", note.customer_name);
//            param.Add("@featured", note.featured);
//            param.Add("@sender_avatar", note.sender_avatar);
//            param.Add("@sender_id", note.sender_id);
//            param.Add("@sender_name", note.sender_name);
//            param.Add("@created_time", note.created_time);
//            param.Add("@updated_time", note.updated_time <= DateTime.MinValue ? null : note.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { note.id,note.customer_id });
//            return rowsAffected > 0;
//        }

//        public bool Insert(Note note)
//        {
//            var query = @"INSERT INTO [Notes]
//           ([id]
//           ,[business_id]
//           ,[customer_id]
//           ,[type]
//           ,[text]
//           ,[thread_id]
//           ,[customer_name]
//           ,[featured]
//           ,[sender_id]
//           ,[sender_name]
//           ,[sender_avatar]
//           ,[created_time]
//           ,[updated_time])
//     VALUES
//           (@id
//           ,@business_id
//           ,@customer_id
//           ,@type
//           ,@text
//           ,@thread_id
//           ,@customer_name
//           ,@featured
//           ,@sender_id
//           ,@sender_name
//           ,@sender_avatar
//           ,@created_time
//           ,@updated_time)";

//            var param = new DynamicParameters();
//            param.Add("@id", note.id);
//            param.Add("@business_id", note.business_id);
//            param.Add("@customer_id", note.customer_id);
//            param.Add("@type", note.type);
//            param.Add("@text", note.text);
//            param.Add("@thread_id", note.thread_id);
//            param.Add("@customer_name", note.customer_name);
//            param.Add("@featured", note.featured);
//            param.Add("@sender_avatar", note.sender_avatar);
//            param.Add("@sender_id", note.sender_id);
//            param.Add("@sender_name", note.sender_name);
//            param.Add("@created_time", note.created_time);
//            param.Add("@updated_time", note.updated_time <= DateTime.MinValue ? null : note.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() {note.id, note.customer_id });
//            return rowsAffected > 0;
//        }

//    }
//}