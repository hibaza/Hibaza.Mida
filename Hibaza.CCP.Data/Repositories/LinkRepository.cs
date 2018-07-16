//using Dapper;
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
//    public class LinkRepository : ILinkRepository
//    {
//        IConnectionFactory _connectionFactory;

//        public LinkRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }
//        public async Task<IEnumerable<Link>> GetLinks(string business_id, string channel_id, Paging page)
//        {
//            IEnumerable<Link> list;
//            var key = "GetLinks" + business_id + channel_id;
//            var rs = await CacheBase.cacheManagerGet<List<Link>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Links"
//                               + " WHERE business_id=@business_id and channel_id=@channel_id  and timestamp<=@until and timestamp>=@since ORDER BY timestamp desc";
//                list = await dbConnection.QueryAsync<Link>(sQuery, new { business_id, channel_id, limit = page.Limit, since= long.Parse(page.Previous ?? "0"), until = long.Parse(page.Next ?? "9999999999") });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                  null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }


//        public bool UpdateTimestamp(string business_id, string id, long timestamp)
//        {
//            var query = @"UPDATE [Links] SET
//                   [timestamp] = @timestamp
//             WHERE id=@id and business_id=@business_id and timestamp<@timestamp";

//            var param = new DynamicParameters();
//            param.Add("@id", id);
//            param.Add("@business_id", business_id);
//            param.Add("@timestamp", timestamp);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { id });
//            return rowsAffected > 0;
//        }

//        public bool UpdateStatus(string business_id, string id, string status)
//        {
//            var query = @"UPDATE [Links] SET
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
//            CacheBase.cacheDeleteCacheSame(new List<string>() { id });
//            return rowsAffected > 0;
//        }



//        public void CreateLink(Link link)
//        {
//            if (!Update(link)) Insert(link);
//        }

//        public bool Update(Link link)
//        {
//            var query = @"UPDATE [Links] SET
//                   [url] = @url
//                  ,[type] = @type
//                  ,[channel_id] = @channel_id
//                  ,[channel_ext_id] = @channel_ext_id
//                  ,[message] = @message
//                  ,[author_id] = @author_id
//                  ,[author_name] = @author_name
//                  ,[objectId] = @objectId
//                  ,[timestamp] = @timestamp
//             WHERE id=@id and business_id=@business_id";

//            var param = new DynamicParameters();
//            param.Add("@id", link.id);
//            param.Add("@business_id", link.business_id);
//            param.Add("@url", link.url);
//            param.Add("@type", link.type);
//            param.Add("@channel_id", link.channel_id);
//            param.Add("@channel_ext_id", link.channel_ext_id);
//            param.Add("@message", link.message);
//            param.Add("@author_id", link.author_id);
//            param.Add("@author_name", link.author_name);
//            param.Add("@objectId", link.objectId);
//            param.Add("@timestamp", link.timestamp);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() {link.id });
//            return rowsAffected > 0;
//        }

//        public Link GetById(string business_id, string id)
//        {
//            var key = "Link_GetById" + business_id + id;
//            var rs =  CacheBase.cacheManagerGet<Link>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = @"SELECT * FROM Links WHERE id = @id and business_id=@business_id";
//                var result = dbConnection.Query<Link>(sQuery, new { business_id, id = id }).FirstOrDefault();

//                CacheBase.cacheManagerSet(key, result, DateTime.Now.AddMinutes(10),
//                 null, null, "", false, new List<string>() { });
//                return result;
//            }
//        }


//        public IEnumerable<Link> GetAll()
//        {
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                var sQuery = @"SELECT* FROM Links";
//                return  dbConnection.Query<Link>(sQuery);

//            }
//        }

//        public bool Insert(Link link)
//        {
//            var query = @"INSERT into Links([id],[business_id],[url],[type],[channel_id],[channel_ext_id],[message],[author_id],[author_name],[objectId],[status],[timestamp],[created_time],[updated_time]) values(@id,@business_id,@url, @type,@channel_id,@channel_ext_id,@message,@author_id,@author_name,@objectId,@status,@timestamp,@created_time,@updated_time)";
//            var param = new DynamicParameters();
//            param.Add("@id", link.id);
//            param.Add("@business_id", link.business_id);
//            param.Add("@url", link.url);
//            param.Add("@type", link.type);
//            param.Add("@channel_id", link.channel_id);
//            param.Add("@channel_ext_id", link.channel_ext_id);
//            param.Add("@message", link.message);
//            param.Add("@author_id", link.author_id);
//            param.Add("@author_name", link.author_name);
//            param.Add("@objectId", link.objectId);
//            param.Add("@timestamp", link.timestamp);
//            param.Add("@status", link.status);
//            param.Add("@created_time", link.created_time);
//            param.Add("@updated_time", link.updated_time = link.updated_time <= DateTime.MinValue ? null : link.updated_time);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            if (rowsAffected > 0)
//            {
//                return true;
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { link.id });
//            return false;
//        }


//        public bool Delete(string key)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
