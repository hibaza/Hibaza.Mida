using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface ITicketService
    {
        IEnumerable<Ticket> GetCustomerTickets(string business_id, string customer_id, Paging page);
        Task<Ticket> GetCustomerLastActiveTicket(string business_id, string customer_id);
        string Create(Domain.Entities.Ticket data);
        Ticket GetById(string business_id, string id);
        Ticket GetById(string id);
        bool Delete(string business_id, string id);
        int UpdateCustomerId();
        Task<IEnumerable<TicketModel>> GetTickets(string business_id, Paging page);
        Task<List<Ticket>> GetAll(string business_id, Paging page);
    }
}
