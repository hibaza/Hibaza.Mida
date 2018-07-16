using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<ReportAgentDataLine>> GetAgentChartData(string business_id, Paging page);
        Task<IEnumerable<ReportChatDataLine>> GetAgentChatChartData(string business_id, Paging page);
        Task<IEnumerable<ReportChatDataLine>> GetCustomerChatChartData(string business_id, Paging page);
        Task<IEnumerable<ReportTicketDataLine>> GetTicketChartData(string business_id, Paging page);
        void UpsertReportAll();
    }
}
