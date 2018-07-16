using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IShortcutRepository
    {
        Shortcut GetById(string business_id, string id);
        Task<IEnumerable<Shortcut>> GetAll(string business_id, Paging page);
        void Upsert(Shortcut shortcut);
        bool Delete(string business_id, string id);
        bool Update(Shortcut shortcut);
        bool Insert(Shortcut shortcut);
        Task<List<Shortcut>> GetByAgent(string business_id, string agent_id);
    }
}
