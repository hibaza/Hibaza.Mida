//using Dapper;
//using Hibaza.CCP.Data.Providers.SQLServer;
//using Hibaza.CCP.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using Hibaza.CCP.Domain.Models;
//using Hibaza.CCP.Domain.Models.Report;

//namespace Hibaza.CCP.Data.Repositories
//{
//    public class ReportRepository : IReportRepository
//    {
//        IConnectionFactory _connectionFactory;

//        public ReportRepository(IConnectionFactory connectionFactory)
//        {
//            _connectionFactory = connectionFactory;
//        }

//        public async Task<IEnumerable<ReportAgentDataLine>> GetAgentChartData(string business_id, Paging page)
//        {
//            IEnumerable<ReportAgentDataLine> list = null;
//            var query = "dbo.ReportsGetAgentsData";
//            var param = new DynamicParameters();
//            param.Add("@business_id", business_id);
//            param.Add("@channel_id", "");
//            param.Add("@agent_id", "");
//            param.Add("@limit", page.Limit);
//            param.Add("@start", page.Previous);
//            param.Add("@end", page.Next);

//            using (var connection = _connectionFactory.GetConnection)
//            {
//                list = await connection.QueryAsync<ReportAgentDataLine>(query, param, commandType: CommandType.StoredProcedure);
//            }

//            return list;
//        }

//        public async Task<IEnumerable<ReportChatDataLine>> GetAgentChatChartData(string business_id, Paging page)
//        {
//            IEnumerable<ReportChatDataLine> list = null;
//            var query = "dbo.ReportsGetChatsData";
//            var param = new DynamicParameters();
//            param.Add("@business_id", business_id);
//            param.Add("@channel_id", "");
//            param.Add("@agent_id", "");
//            param.Add("@limit", page.Limit);
//            param.Add("@start", page.Previous);
//            param.Add("@end", page.Next);

//            using (var connection = _connectionFactory.GetConnection)
//            {
//                list = await connection.QueryAsync<ReportChatDataLine>(query, param, commandType: CommandType.StoredProcedure);
//            }

//            return list;
//        }

//        public async Task<IEnumerable<ReportChatDataLine>> GetCustomerChatChartData(string business_id, Paging page)
//        {
//            IEnumerable<ReportChatDataLine> list = null;
//            var query = "dbo.ReportsGetCustomerChatsData";
//            var param = new DynamicParameters();
//            param.Add("@business_id", business_id);
//            param.Add("@channel_id", "");
//            param.Add("@agent_id", "");
//            param.Add("@limit", page.Limit);
//            param.Add("@start", page.Previous);
//            param.Add("@end", page.Next);

//            using (var connection = _connectionFactory.GetConnection)
//            {
//                list = await connection.QueryAsync<ReportChatDataLine>(query, param, commandType: CommandType.StoredProcedure);
//            }

//            return list;
//        }

//        public async Task<IEnumerable<ReportTicketDataLine>> GetTicketChartData(string business_id, Paging page)
//        {
//            IEnumerable<ReportTicketDataLine> list = null;
//            var query = "dbo.ReportsGetTicketsData";
//            var param = new DynamicParameters();
//            param.Add("@business_id", business_id);
//            param.Add("@channel_id", "");
//            param.Add("@agent_id", "");
//            param.Add("@limit", page.Limit);
//            param.Add("@start", DateTime.Parse(page.Previous));
//            param.Add("@end", DateTime.Parse(page.Next));

//            using (var connection = _connectionFactory.GetConnection)
//            {
//                list = await connection.QueryAsync<ReportTicketDataLine>(query, param, commandType: CommandType.StoredProcedure);
//            }

//            return list;
//        }
//    }
//}
