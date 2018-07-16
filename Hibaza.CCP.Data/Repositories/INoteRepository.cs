using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface INoteRepository
    {
        Note GetById(string business_id, string id);
        Task<IEnumerable<Note>> GetAll(string business_id, Paging page);
        void Upsert(Note note);
        bool Delete(string business_id, string id);
        bool Insert(Note note);
        bool Update(Note note);
        Task<IEnumerable<Note>> GetCustomerNotes(string business_id, string customer_id, Paging page);
        int UpdateCustomerId();
    }
}
