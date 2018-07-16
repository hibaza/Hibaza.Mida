using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IReferralRepository
    {

        Referral GetById(string business_id, string id);
        Task<IEnumerable<Referral>> GetAll(string business_id, Paging page);
        void Upsert(Referral referral);
        bool Delete(string business_id, string id);
        bool Update(Referral referral);
        bool Insert(Referral referral);
        Task<IEnumerable<Referral>> GetReferrals(string business_id, string thread_id, Paging page);
        Task<IEnumerable<Referral>> GetReferralsByCustomer(string business_id, string customer_id, Paging page);
        int UpdateCustomerId();
        Task<List<Referral>> GetReferralsByCustomerIsNull(int limit);
        
    }
}
