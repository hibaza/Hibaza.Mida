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
    public class FirebaseTicketRepository : IFirebaseTicketRepository
    {
        IFirebaseFactory _connectionFactory;
        private const string TICKETS = "tickets";
        public FirebaseTicketRepository(IFirebaseFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Ticket GetById(string business_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(business_id)
              .Child(TICKETS).Child(id).OnceSingleAsync<Ticket>().Result;
            return c;
        }


        public async Task<dynamic> GetCustomerTickets(string business_id, string customer_id, Paging page)
        {
            return _connectionFactory.GetConnection.Child(business_id).Child(TICKETS).OrderBy("customer_id").EqualTo(customer_id).LimitToLast(page.Limit).OnceAsync<Ticket>().Result;
        }

          
        public void Upsert(string business_id, Ticket ticket)
        {
            var rs = _connectionFactory.GetConnection.Child(business_id).Child(TICKETS).Child(ticket.id).PutAsync(ticket);
        }

        public bool Delete(string bussiness_id, string id)
        {
            var c = _connectionFactory.GetConnection.Child(bussiness_id).Child(TICKETS).Child(id).DeleteAsync();
            return true;
        }

        public IEnumerable<Ticket> GetAll(string business_id, Paging page)
        {
            return _connectionFactory.GetConnection.Child(business_id).Child(TICKETS).OrderBy("created_time").LimitToLast(page.Limit).OnceAsync<Ticket>().Result.Select(t => t.Object);
        }

        public void Update(string business_id, Ticket entity)
        {
            throw new NotImplementedException();
        }
    }
}
