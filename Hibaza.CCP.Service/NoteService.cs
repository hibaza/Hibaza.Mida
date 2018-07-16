using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;
        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }
       
        public string Create(Note data)
        {
            if (!string.IsNullOrWhiteSpace(data.business_id))
            {
                _noteRepository.Upsert(data);
            }
            return data.id;
        }

        public IEnumerable<Note> GetCustomerNotes(string business_id, string customer_id, Paging page)
        {
            return _noteRepository.GetCustomerNotes(business_id, customer_id, new Paging { Limit = 1000 }).Result;
        }

        public int UpdateCustomerId()
        {
            return _noteRepository.UpdateCustomerId();
        }


        public bool Delete(string business_id, string id)
        {
            if (!string.IsNullOrWhiteSpace(business_id) && !string.IsNullOrWhiteSpace(id))
            {
                return _noteRepository.Delete(business_id, id);
            }
            return false;
        }

        public Note GetById(string business_id, string id)
        {
            return _noteRepository.GetById(business_id, id);
        }
    }
}
