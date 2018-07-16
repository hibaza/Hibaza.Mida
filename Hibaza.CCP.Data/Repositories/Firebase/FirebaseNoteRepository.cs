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
    public class FirebaseNoteRepository : IFirebaseNoteRepository
    {
        IFirebaseFactory _connectionFactory;
        private const string NOTES = "notes";
        public FirebaseNoteRepository(IFirebaseFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Note GetById(string business_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(business_id)
              .Child(NOTES).Child(id).OnceSingleAsync<Note>().Result;
            return c;
        }


        public async Task<dynamic> GetCustomerNotes(string business_id, string customer_id, Paging page)
        {
            return await _connectionFactory.GetConnection.Child(business_id).Child(NOTES).OrderBy("customer_id").EqualTo(customer_id).LimitToLast(page.Limit).OnceAsync<Note>();
        }

          
        public void Upsert(string business_id, Note note)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(NOTES).Child(note.id).PutAsync(note);
        }

        public bool Delete(string bussiness_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(bussiness_id).Child(NOTES).Child(id).DeleteAsync();
            return true;
        }

        public IEnumerable<Note> GetAll(string business_id, Paging page)
        {
            return _connectionFactory.GetConnection.Child(business_id).Child(NOTES).OrderBy("created_time").LimitToLast(page.Limit).OnceAsync<Note>().Result.Select(n=>n.Object);
        }

        public void Update(string business_id, Note entity)
        {
            throw new NotImplementedException();
        }
    }
}
