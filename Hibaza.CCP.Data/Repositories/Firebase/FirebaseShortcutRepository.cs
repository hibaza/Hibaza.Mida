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
    public class FirebaseShortcutRepository : IFirebaseShortcutRepository
    {
        IFirebaseFactory _connectionFactory;
        private const string SHORTCUTS = "shortcuts";
        public FirebaseShortcutRepository(IFirebaseFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Shortcut GetById(string business_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(business_id)
              .Child(SHORTCUTS).Child(id).OnceSingleAsync<Shortcut>().Result;
            return c;
        }


        public async Task<dynamic> GetShortcuts(string business_id, Paging page)
        {
            return _connectionFactory.GetConnection.Child(business_id).Child(SHORTCUTS).OrderBy("created_time").LimitToLast(page.Limit).OnceAsync<Shortcut>().Result;
        }

          
        public void Upsert(string business_id, Shortcut shortcut)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(SHORTCUTS).Child(shortcut.id).PutAsync(shortcut);
        }

        public bool Delete(string bussiness_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(bussiness_id).Child(SHORTCUTS).Child(id).DeleteAsync();
            return true;
        }

        public IEnumerable<Shortcut> GetAll(string business_id)
        {
            throw new NotImplementedException();
        }

        public void Update(string business_id, Shortcut entity)
        {
            throw new NotImplementedException();
        }
    }
}
