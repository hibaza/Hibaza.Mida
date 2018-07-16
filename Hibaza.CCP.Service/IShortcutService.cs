using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface IShortcutService
    {
        Task<IEnumerable<Shortcut>> GetShortcuts(string business_id, int pageIndex, int pageSize);
        string Create(Domain.Entities.Shortcut data);
        Shortcut GetById(string business_id, string id);
        bool Delete(string business_id, string id);
        Task<List<Shortcut>> GetByAgent(string business_id, string agent_id);
        

    }
}
