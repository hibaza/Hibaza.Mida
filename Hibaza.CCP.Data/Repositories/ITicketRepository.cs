using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface ITicketRepository
    {
        Ticket GetById(string id);
        Ticket GetById(string business_id, string id);
        Task<List<Ticket>> GetAll(string business_id, Paging page);
        void Upsert(Ticket ticket);
        bool Insert(Ticket ticket);
        bool Delete(string business_id, string id);
        bool Update(Ticket ticket);
        Task<IEnumerable<Ticket>> GetCustomerTickets(string business_id, string customer_id, int status, Paging page);
        Task<Ticket> GetCustomerLastTicket(string business_id, string customer_id, int status);
        int UpdateCustomerId();
        Task<IEnumerable<Ticket>> GetTickets(string business_id, int status, Paging page);
    }
}
