using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface IReportService
    {
        Task<IEnumerable<Agent>> GetTopAgents(string business_id, Paging page);
        Task<IEnumerable<Thread>> GetTopCustomers(string business_id, Paging page);

        Task<IEnumerable<ReportAgentDataLine>> GetAgents(string business_id, ReportDataFilter filter);
        Task<IEnumerable<ReportCustomerModel>> GetCustomers(string business_id, ReportDataFilter filter);
        Task<IEnumerable<ReportTicketModel>> GetTickets(string business_id, ReportDataFilter filter);
        Task<IEnumerable<ReportAgentDataLine>> GetAgentChartData(string business_id, ReportDataFilter filter);
        Task<IEnumerable<ReportMessageModel>> GetMessageChartData(string business_id, ReportDataFilter filter);
        Task<IEnumerable<ReportTicketDataLine>> GetTicketChartData(string business_id, ReportDataFilter filter);
        Task<IEnumerable<ReportResponseModel>> GetFirstResponseChartData(string business_id, ReportDataFilter filter);
        Task<IEnumerable<ReportChatDataLine>> GetAgentChatChartData(string business_id, ReportDataFilter filter);
        Task<IEnumerable<ReportChatDataLine>> GetCustomerChatChartData(string business_id, ReportDataFilter filter);
       void UpsertReportAll();
    }
}
