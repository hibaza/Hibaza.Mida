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
    public class FirebaseBusinessRepository : IFirebaseBusinessRepository
    {
        IFirebaseFactory _connectionFactory;
        private const string BUSINESS = "business";
        public FirebaseBusinessRepository(IFirebaseFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Add(Business entity)
        {
            var rs = _connectionFactory.GetConnection.Child(BUSINESS).Child(entity.id).PutAsync(entity);
        }


        public void Update(Business entity)
        {
            throw new NotImplementedException();
        }

        public Business GetById(string id)
        {
            var c = _connectionFactory.GetConnection
              .Child(BUSINESS).Child(id).OnceSingleAsync<Business>().Result;
            return c;
        }

        public IEnumerable<Business> GetAll()
        {
            return _connectionFactory.GetConnection.Child(BUSINESS).OrderBy("created_time").OnceAsync<Business>().Result.Select(b => b.Object);
        }

        public IEnumerable<Business> GetBusinesses(Paging page)
        {
            return _connectionFactory.GetConnection.Child(BUSINESS).OrderBy("created_time").LimitToLast(page.Limit).OnceAsync<Business>().Result.Select(b => b.Object);
        }
        public bool Delete(string id)
        {
            var c = _connectionFactory.GetConnection.Child(BUSINESS).Child(id).DeleteAsync();
            return true;
        }

        public bool DeleteData(string id)
        {
            var c = _connectionFactory.GetConnection.Child(id).DeleteAsync();
            return true;
        }
    }
}
