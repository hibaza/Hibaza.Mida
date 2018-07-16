//using Dapper;
//using Hibaza.CCP.Data.Providers.SQLServer;
//using Hibaza.CCP.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using Hibaza.CCP.Domain.Models;
//using Hibaza.CCP.Data.Providers.Mongo;
//using Newtonsoft.Json;

//namespace Hibaza.CCP.Data.Repositories
//{
//    public class ChannelRepository : IChannelRepository
//    {
//        IConnectionFactory _connectionFactory;

//        public ChannelRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public void Add(Channel channel)
//        {
//            if (!Update(channel)) Create(channel);
//        }

//        private bool Create(Channel channel)
//        {
//            var query = @"INSERT INTO [Channels]([id],[business_id],[name],[type],[ext_id],[token],[active],[created_time],[updated_time]) VALUES( 
//                     @id, @business_id, @name, @type, @ext_id, @token, @active, @created_time,@updated_time)";

//            var param = new DynamicParameters();
//            param.Add("@id", channel.id);
//            param.Add("@business_id", channel.business_id);
//            param.Add("@name", channel.name);
//            param.Add("@type", channel.type);
//            param.Add("@ext_id", channel.ext_id);
//            param.Add("@token", channel.token);
//            param.Add("@active", channel.active);
//            param.Add("@created_time", channel.created_time);
//            param.Add("@updated_time", channel.updated_time <= DateTime.MinValue ? null : channel.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { channel.id });
//            return rowsAffected > 0;
//        }

//        private bool Update(Channel channel)
//        {
//            var query = @"UPDATE [Channels] SET 
//                     [name] = @name, [type] = @type, [ext_id] = @ext_id, [token] = @token, [active] = @active,[created_time] = @created_time, [updated_time] = @updated_time 
//                     WHERE id = @id and business_id=@business_id";
//            var param = new DynamicParameters();
//            param.Add("@id", channel.id);
//            param.Add("@business_id", channel.business_id);
//            param.Add("@name", channel.name);
//            param.Add("@type", channel.type);
//            param.Add("@ext_id", channel.ext_id);
//            param.Add("@token", channel.token);
//            param.Add("@active", channel.active);
//            param.Add("@created_time", channel.created_time);
//            param.Add("@updated_time", channel.updated_time <= DateTime.MinValue ? null : channel.updated_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { channel.id });
//            return rowsAffected > 0;
//        }


//        public bool Delete(string business_id, string id)
//        {
//            var query = @"DELETE FROM [Channels] WHERE id = @id and business_id=@business_id";
//            var param = new DynamicParameters();
//            param.Add("@id", id);
//            param.Add("@business_id", business_id);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//            CacheBase.cacheDeleteCacheSame(new List<string>() { id });
//            return rowsAffected > 0;
//        }

//        public IEnumerable<Channel> GetAll(string business_id)
//        {
//            IEnumerable<Channel> list;
//            var key = "Channel_GetAll" + business_id;
//            var rs = CacheBase.cacheManagerGet<List<Channel>>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Channels"
//                               + " WHERE business_id = @business_id";
//                list = dbConnection.Query<Channel>(sQuery, new { business_id });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                  null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }

//        public Channel GetById(string business_id, string id)
//        {
//            Channel channel;
//            var key = "Agent_GetById01" + business_id + id;
//            var rs = CacheBase.cacheManagerGet<Channel>(key).Result;
//            if (rs != null)
//            {
//                return rs;
//            }
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT * FROM Channels"
//                               + " WHERE business_id=@business_id and id=@id";
//                channel = dbConnection.Query<Channel>(sQuery, new { business_id, id }).FirstOrDefault();
//                CacheBase.cacheManagerSet(key, channel, DateTime.Now.AddMinutes(10),
//                  null, null, "", false, new List<string>() { });
//            }
//            return channel;
//        }

//        public async Task<IEnumerable<Channel>> GetChannels(string business_id, Paging page)
//        {
//            IEnumerable<Channel> list;
//            var key = "GetChannels" + business_id;
//            var rs = await CacheBase.cacheManagerGet<List<Channel>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Channels"
//                               + " WHERE business_id = @business_id";
//                list = await dbConnection.QueryAsync<Channel>(sQuery, new { business_id, limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                  null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }

//        public async Task<IEnumerable<Channel>> GetChannelsByExtId(string ext_id, Paging page)
//        {
//            IEnumerable<Channel> list;
//            var key = "GetChannelsByExtId" + ext_id;
//            var rs = await CacheBase.cacheManagerGet<List<Channel>>(key);
//            if (rs != null)
//            {
//                return rs;
//            }

//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Channels"
//                               + " WHERE ext_id = @ext_id";
//                list = await dbConnection.QueryAsync<Channel>(sQuery, new { ext_id, limit = page.Limit });
//                CacheBase.cacheManagerSet(key, list, DateTime.Now.AddMinutes(10),
//                  null, null, "", false, new List<string>() { });
//            }
//            return list;
//        }


//    }
//}
