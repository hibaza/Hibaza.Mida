//using Dapper;
//using Hibaza.CCP.Data.Providers.SQLServer;
//using Hibaza.CCP.Domain.Entities;
//using Hibaza.CCP.Domain.Models;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Hibaza.CCP.Data.Repositories
//{
//    public class AttachmentRepository : IAttachmentRepository
//    {
//        IConnectionFactory _connectionFactory;

//        public AttachmentRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }
//        public async Task<IEnumerable<Attachment>> GetAttachments(string business_id, string channel_id, string product_id, Paging page)
//        {
//            IEnumerable<Attachment> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Attachments"
//                               + " WHERE business_id=@business_id and channel_id=@channel_id and product_id=@product_id and timestamp<=@until  and timestamp>=@since ORDER BY timestamp desc";
//                list = await dbConnection.QueryAsync<Attachment>(sQuery, new { business_id, channel_id, product_id, limit = page.Limit, since = long.Parse(page.Previous ?? "0"), until = long.Parse(page.Next ?? "9999999999") });
//            }
//            return list;
//        }


//        public Attachment GetById(string business_id, string channel_id, string id)
//        {
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = @"SELECT * FROM Attachments WHERE business_id=@business_id and channel_id=@channel_id and id = @id";
//                return dbConnection.Query<Attachment>(sQuery, new { business_id, channel_id, id }).FirstOrDefault();
//            }
//        }


//        public async Task<IEnumerable<Attachment>> GetAll(string business_id, string channel_id, Paging page)
//        {
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                var sQuery = @"SELECT TOP(@limit) * FROM Attachments WHERE business_id=@business_id and channel_id=@channel_id";
//                return await dbConnection.QueryAsync<Attachment>(sQuery, new { business_id, channel_id, limit = page.Limit});
//            }
//        }

//        public bool Update(Attachment attachment)
//        {
//            var query = @"UPDATE [Attachments] SET
//                       [source_url] = @source_url
//                      ,[attachment_id] = @attachment_id
//                      ,[attachment_url] = @attachment_url
//                      ,[type] = @type
//                      ,[timestamp] = @timestamp
//                      ,[tag] = @tag
//                      ,[target] = @target
//             WHERE id=@id and business_id=@business_id and channel_id=@channel_id";

//            var param = new DynamicParameters();
//            param.Add("@id", attachment.id);
//            param.Add("@business_id", attachment.business_id);
//            param.Add("@channel_id", attachment.channel_id);
//            param.Add("@source_url", attachment.source_url);
//            param.Add("@type", attachment.type);
//            param.Add("@attachment_id", attachment.attachment_id);
//            param.Add("@attachment_url", attachment.attachment_url);
//            param.Add("@timestamp", attachment.timestamp);
//            param.Add("@tag", attachment.tag);
//            param.Add("@target", attachment.target);
//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }

//            return rowsAffected > 0;
//        }

//        public bool Insert(Attachment attachment)
//        {
//            var query = @"INSERT into Attachments
//                       ([id]
//                       ,[business_id]
//                       ,[channel_id]
//                       ,[source_url]
//                       ,[attachment_id]
//                       ,[attachment_url]
//                       ,[type]
//                       ,[timestamp]
//                       ,[tag]
//                       ,[target])
//                        VALUES
//                       (@id
//                       ,@business_id
//                       ,@channel_id
//                       ,@source_url
//                       ,@attachment_id
//                       ,@attachment_url
//                       ,@type
//                       ,@timestamp
//                       ,@tag
//                       ,@target)";
//            var param = new DynamicParameters();
//            param.Add("@id", attachment.id);
//            param.Add("@business_id", attachment.business_id);
//            param.Add("@channel_id", attachment.channel_id);
//            param.Add("@source_url", attachment.source_url);
//            param.Add("@type", attachment.type);
//            param.Add("@attachment_id", attachment.attachment_id);
//            param.Add("@attachment_url", attachment.attachment_url);
//            param.Add("@timestamp", attachment.timestamp);
//            param.Add("@tag", attachment.tag);
//            param.Add("@target", attachment.target);
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


//        public bool Upsert(Attachment attachment)
//        {
//            if (!Update(attachment)) Insert(attachment);
//            return true;
//        }

//        public bool Delete(string business_id, string channel_id, string id)
//        {
//            throw new NotImplementedException();
//        }

//    }
//}
