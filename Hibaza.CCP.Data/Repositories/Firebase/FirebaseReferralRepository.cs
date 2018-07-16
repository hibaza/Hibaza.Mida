using Dapper;
using Firebase.Database.Query;
using Hibaza.CCP.Data.Providers.Firebase;
using Hibaza.CCP.Data.Providers.SQLServer;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public class FirebaseReferralRepository : IFirebaseReferralRepository
    {
        IFirebaseFactory _connectionFactory;
        public static string REFERRALS = "referrals";

        public FirebaseReferralRepository(IFirebaseFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Upsert(string business_id, Referral entity)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(REFERRALS).Child(entity.id).PutAsync(entity);
        }


        public void Update(string business_id, Referral entity)
        {
            throw new NotImplementedException();
        }

        public Referral GetById(string business_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(business_id)
              .Child(REFERRALS).Child(id).OnceSingleAsync<Referral>().Result;
            return c;
        }


        public IEnumerable<Referral> GetReferrals(string business_id, string thread_id, Paging page)
        {
            List<Referral> list = new List<Referral>();
            var data = _connectionFactory.GetConnection.Child(business_id).Child(REFERRALS).OrderBy("thread_id").EqualTo(thread_id).LimitToLast(page.Limit).OnceAsync<Referral>().Result;
            foreach (var c in data)
            {
                list.Add(c.Object);
            }
            return list;
        }

        public bool Delete(string business_id, string id)
        {
            if (string.IsNullOrWhiteSpace(business_id) || string.IsNullOrWhiteSpace(id)) return false;
            var c = _connectionFactory.GetConnection.Child(business_id).Child(REFERRALS).Child(id).DeleteAsync();
            return true;
        }

        public IEnumerable<Referral> GetAll(string business_id, Paging page)
        {
            List<Referral> list = new List<Referral>();
            var data = _connectionFactory.GetConnection.Child(business_id).Child(REFERRALS).OrderBy("created_time").LimitToLast(page.Limit).OnceAsync<Referral>().Result;
            foreach (var c in data)
            {
                list.Add(c.Object);
            }
            return list;
        }
    }
}
