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
    public class FirebaseLoggingRepository : IFirebaseLoggingRepository
    {
        IFirebaseFactory _connectionFactory;
        private const string LOGS = "applogs";
        public FirebaseLoggingRepository(IFirebaseFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Add(Log entity)
        {
            var rs = _connectionFactory.GetConnection.Child(LOGS).Child(entity.id).PutAsync(entity);
        }


        public void Update(Log entity)
        {
            throw new NotImplementedException();
        }

        public Log GetById(string key)
        {
            var c = _connectionFactory.GetConnection
              .Child(LOGS).Child(key).OnceSingleAsync<Log>().Result;
            return c;
        }


        public async Task<dynamic> GetErrorLogs(Paging page)
        {
            return _connectionFactory.GetConnection.Child(LOGS).OrderBy("updated_time").LimitToLast(page.Limit).OnceAsync<Log>().Result;
        }

        public bool Delete(string key)
        {
            var c = _connectionFactory.GetConnection.Child(LOGS).Child(key).DeleteAsync();
            return true;
        }

        public IEnumerable<Log> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
