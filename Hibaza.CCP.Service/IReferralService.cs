using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface IReferralService
    {
        Referral GetById(string business_id, string id);
        Task<IEnumerable<Referral>> GetReferrals(string business_id, string thread_id, Paging page);
        IEnumerable<Referral> GetAll(string business_id, Paging page);
        void Create(Referral referral);
        Referral CreateReferral(string business_id, Thread threads, string referral_id, string sender_ext_id, string recipient_ext_id, string data,string thread_id);
        Task<IEnumerable<Referral>> GetReferralsByCustomer(string business_id, string customer_id, Paging page);
        int UpdateCustomerId();
        Task<IEnumerable<Referral>> GetReferralsByCustomerIsNull(int limit);
    }
}
