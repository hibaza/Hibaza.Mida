//using Dapper;
//using Firebase.Database.Query;
//using Hibaza.CCP.Data.Providers.Firebase;
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
//    public class LoggingRepository : ILoggingRepository
//    {
//        IConnectionFactory _connectionFactory;
//        private const string LOGS = "applogs";
//        public LoggingRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public void Add(Log log)
//        {
//            var query = @"INSERT INTO [Logs]
//           ([id]
//           ,[business_id]
//           ,[link]
//           ,[details]
//           ,[message]
//           ,[name]
//           ,[category]
//           ,[created_time])
//     VALUES
//           (@id
//           ,@business_id
//           ,@link
//           ,@details
//           ,@message
//           ,@name
//           ,@category
//           ,@created_time)";

//            var param = new DynamicParameters();
//            param.Add("@id", log.id);
//            param.Add("@business_id", log.business_id);
//            param.Add("@link", log.link);
//            param.Add("@details", log.details);
//            param.Add("@message", log.message);
//            param.Add("@name", log.name);
//            param.Add("@category", log.category);
//            param.Add("@created_time", log.created_time);

//            int rowsAffected;
//            using (var connection = _connectionFactory.GetConnection)
//            {
//                rowsAffected = SqlMapper.Execute(connection, query, param, commandType: CommandType.Text);
//            }
//        }

//        public void Update(Log entity)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<Log> GetLogs(Paging page)
//        {
//            IEnumerable<Log> list;
//            using (var dbConnection = _connectionFactory.GetConnection)
//            {
//                string sQuery = "SELECT TOP(@limit) * FROM Logs where message='Webhook' and [key]<@key ORDER BY created_time DESC";
//                list = dbConnection.Query<Log>(sQuery, new { limit = page.Limit, key = long.Parse(page.Next) });
//            }
//            return list;
//        }

//        public IEnumerable<Log> GetAll()
//        {
//            throw new NotImplementedException();
//        }

//        public Log GetById(string id)
//        {
//            throw new NotImplementedException();
//        }

//        public bool Delete(string id)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
