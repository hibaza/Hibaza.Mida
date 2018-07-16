using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface INoteService
    {
        IEnumerable<Note> GetCustomerNotes(string business_id, string customer_id, Paging page);
        string Create(Domain.Entities.Note data);
        Note GetById(string business_id, string id);
        bool Delete(string business_id, string id);
        int UpdateCustomerId();
    }
}
