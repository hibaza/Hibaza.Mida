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
//    public class NodeRepository : INodeRepository
//    {
//        IConnectionFactory _connectionFactory;

//        public NodeRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }
//        public async Task<IEnumerable<Node>> GetNodes(string business_id, string channel_id, Paging page)
//        {
//            IEnumerable<Node> list;
//            var key = "GetNodes" + business_id + channel_id;
//            var rs = await CacheBase.cacheManagerGet<IEnumerable<Node>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Nodes"
//                               + " WHERE business_id=@business_id and channel_id=@channel_id and timestamp<=@until  and timestamp>=@since ORDER BY timestamp desc";
//                list = await dbConnection.QueryAsync<Node>(sQuery, new { business_id, channel_id, limit = page.Limit, since = long.Parse(page.Previous ?? "0"), until = long.Parse(page.Next ?? "9999999999") });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Node>> GetPendingNodes(string business_id, string channel_id, string type, Paging page)
//        {
//            IEnumerable<Node> list;
//            var key = "GetPendingNodes" + business_id + channel_id+ type;
//            var rs = await CacheBase.cacheManagerGet<IEnumerable<Node>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Nodes"
//                               + " WHERE business_id=@business_id and channel_id=@channel_id and timestamp<=@until  and timestamp>=@since and status='pending' and ([type]=@type or @type='') ORDER BY timestamp desc";
//                list = await dbConnection.QueryAsync<Node>(sQuery, new { business_id, channel_id, type = type ?? "", limit = page.Limit, since = long.Parse(page.Previous ?? "0"), until = long.Parse(page.Next ?? "9999999999") });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }

//        public bool UpdateStatus(string business_id, string id, string status)
//        {
//            var query = @"UPDATE [Nodes] SET
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



//        public void CreateNode(Node node)
//        {
//            if (!Update(node)) Insert(node);
//        }

//        public bool Update(Node node)
//        {
//            var query = @"UPDATE [Nodes] SET
//                   [type] = @type
//                  ,[channel_id] = @channel_id
//                  ,[data] = @data
//                  ,[timestamp] = @timestamp
//             WHERE id=@id and business_id=@business_id";

//            var param = new DynamicParameters();
//            param.Add("@id", node.id);
//            param.Add("@business_id", node.business_id);
//            param.Add("@type", node.type);
//            param.Add("@channel_id", node.channel_id);
//            param.Add("@data", node.data);
//            param.Add("@timestamp", node.timestamp);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }

//            return rowsAffected > 0;
//        }

//        public Node GetById(string business_id, string id)
//        {
//            var key = "Node_GetById" + business_id + id;
//            var rs = CacheBase.cacheManagerGet<Node>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = @"SELECT * FROM Nodes WHERE id = @id";
//                var result = dbConnection.Query<Node>(sQuery, new { business_id, id }).FirstOrDefault();
//                CacheBase.cacheManagerSet(key, result, DateTime.Now.AddMinutes(10),
//                null, null, "", false, new List<string>() { });
//                return result;
//            }
//        }


//        public IEnumerable<Node> GetAll()
//        {
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                var sQuery = @"SELECT* FROM Nodes";
//                return dbConnection.Query<Node>(sQuery);
//            }
//        }

//        public bool Insert(Node node)
//        {
//            var query = @"INSERT into Nodes([id],[business_id],[type],[channel_id],[data],[status],[timestamp],[created_time],[updated_time]) values(@id,@business_id,@type,@channel_id,@data,@status,@timestamp,@created_time,@updated_time)";
//            var param = new DynamicParameters();
//            param.Add("@id", node.id);
//            param.Add("@business_id", node.business_id);
//            param.Add("@type", node.type);
//            param.Add("@channel_id", node.channel_id);
//            param.Add("@data", node.data);
//            param.Add("@timestamp", node.timestamp);
//            param.Add("@status", node.status);
//            param.Add("@created_time", node.created_time);
//            param.Add("@updated_time", node.updated_time = node.updated_time <= DateTime.MinValue ? null : node.updated_time);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            if (rowsAffected > 0)
//            {
//                return true;
//            }
//            return false;
//        }


//        public bool Delete(string key)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
