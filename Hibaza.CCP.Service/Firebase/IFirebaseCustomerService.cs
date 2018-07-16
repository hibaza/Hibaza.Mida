using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service.Firebase
{
    public interface IFirebaseCustomerService
    {
        bool CreateCustomer(Domain.Entities.Customer customer);
        Customer CreateCustomer(string business_id, string customer_id, Domain.Entities.Message message);
        Task<IEnumerable<Domain.Entities.Customer>> All(string business_id);
        Customer GetById(string business_id, string id);
        void Delete(string business_id, string id);
        Task<IEnumerable<Domain.Entities.Customer>> GetCustomers(string business_id, Paging page, string channelId, string agentId, string status, string flag);
        bool UpdateReferral(FacebookMessagingEvent referralEvent);
        string GetAppIdByExtId(string business_id, string ext_id);
        void MapExtIdAndAppId(string business_id, string ext_id, string app_id, string @ref);
    }
}
