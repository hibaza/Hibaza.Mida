using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public interface IFirebaseNoteRepository
    {
        Note GetById(string business_id, string id);
        IEnumerable<Note> GetAll(string business_id, Paging page);
        void Upsert(string business_id, Note note);
        bool Delete(string business_id, string id);
        void Update(string business_id, Note note);

        Task<dynamic> GetCustomerNotes(string business_id, string customer_id, Paging page);
    }
}
