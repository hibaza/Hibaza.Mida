using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public interface IFirebaseReferralRepository
    {

        Referral GetById(string business_id, string id);
        IEnumerable<Referral> GetAll(string business_id, Paging page);
        void Upsert(string business_id, Referral referral);
        bool Delete(string business_id, string id);
        void Update(string business_id, Referral referral);

        IEnumerable<Referral> GetReferrals(string business_id, string thread_id, Paging page);
    }
}
