using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public interface IFirebaseCustomerRepository : IGenericRepository<Domain.Entities.Customer>
    {
        Task<dynamic> GetCustomers(string businessId, Paging page);
        Task<dynamic> GetCustomersByChannel(string businessId, Paging page, string channelId);
        Task<dynamic> GetCustomersByChannelAndAgent(string businessId, Paging page, string channelId, string agentId);
        Task<dynamic> GetCustomersByChannelAndStatus(string businessId, Paging page, string channelId, string status);
        Task<dynamic> GetCustomersByChannelAndAgentAndStatus(string businessId, Paging page, string channelId, string agentId, string status);
        Task<dynamic> GetCustomersByAgentAndStatus(string businessId, Paging page, string agentId, string status);
        Task<dynamic> GetCustomersByAgent(string businessId, Paging page, string agentId);
        Task<dynamic> GetCustomersByStatus(string businessId, Paging page, string status);

        Task<dynamic> GetCustomersByFlag(string businessId, Paging page, string flag);
        Task<dynamic> GetCustomersByAgentAndFlag(string businessId, Paging page, string agentId, string flag);
        Task<dynamic> GetCustomersByChannelAndFlag(string businessId, Paging page, string channelId, string flag);
        Task<dynamic> GetCustomersByChannelAndAgentAndFlag(string businessId, Paging page, string channelId, string agentId, string flag);


        string GetBusinessUIDByPageUID(string puid);
        void UpdatePageBusinessMapping(string buid, string puid, string @ref);

        string GetAppUIDByPageUID(string business_id, string puid);
        void UpdateAppPageMapping(string business_id, string puid, string auid, string @ref);
        string GetPageReferalParam(string business_id, string puid);
    }
}
