using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Data.Repositories.Firebase;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IFirebaseTicketRepository _fbTicketRepository;
        public TicketService(ITicketRepository ticketRepository, IFirebaseTicketRepository fbTicketRepository)
        {
            _ticketRepository = ticketRepository;
            _fbTicketRepository = fbTicketRepository;
        }
       
        public string Create(Ticket data)
        {
            if (!string.IsNullOrWhiteSpace(data.business_id))
            {
                _ticketRepository.Upsert(data);
                _fbTicketRepository.Upsert(data.business_id, data);
            }
            return data.id;
        }

        public int UpdateCustomerId()
        {
            return _ticketRepository.UpdateCustomerId();
        }

        public IEnumerable<Ticket> GetCustomerTickets(string business_id, string customer_id, Paging page)
        {
            return _ticketRepository.GetCustomerTickets(business_id, customer_id, -1, page).Result;
        }

        public async Task<IEnumerable<TicketModel>> GetTickets(string business_id, Paging page)
        {
            return (await _ticketRepository.GetTickets(business_id, -1,  page)).Select(t=>new TicketModel(t));
        }

        public async Task<Ticket> GetCustomerLastActiveTicket(string business_id, string customer_id)
        {
            var ticket = await _ticketRepository.GetCustomerLastTicket(business_id, customer_id, 0);
            if (ticket == null)
            {
                ticket = await _ticketRepository.GetCustomerLastTicket(business_id, customer_id, -1);
            }
            return ticket;
        }


        public bool Delete(string business_id, string id)
        {
            if (!string.IsNullOrWhiteSpace(business_id) && !string.IsNullOrWhiteSpace(id))
            {
                _fbTicketRepository.Delete(business_id, id);
                return _ticketRepository.Delete(business_id, id);
            }
            return false;
        }

        public Ticket GetById(string id)
        {
            return _ticketRepository.GetById(id);
        }

        public Ticket GetById(string business_id, string id)
        {
            return _ticketRepository.GetById(business_id, id);
        }

        public Ticket GetLastForCustomer(string business_id, string customer_id)
        {
            return this.GetCustomerTickets(business_id, customer_id, new Paging { Limit = 1 }).FirstOrDefault();
        }

        public async Task<List<Ticket>> GetAll(string business_id, Paging page)
        {
            return (await _ticketRepository.GetAll(business_id, page));
        }
    }
}
