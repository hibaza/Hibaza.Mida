using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public class ShortcutService : IShortcutService
    {
        private readonly IShortcutRepository _shortcutRepository;
        public ShortcutService(IShortcutRepository shortcutRepository)
        {
            _shortcutRepository = shortcutRepository;
        }
       
        public string Create(Shortcut data)
        {
            if (!string.IsNullOrWhiteSpace(data.business_id))
            {
                _shortcutRepository.Upsert(data);
            }
            return data.id;
        }

        public async Task<IEnumerable<Shortcut>> GetShortcuts(string business_id, int pageIndex, int pageSize)
        {
            return await _shortcutRepository.GetAll(business_id, new Domain.Models.Paging { Limit = pageSize });
        }
        public async Task<List<Shortcut>> GetByAgent(string business_id, string agent_id)
        {
            return await _shortcutRepository.GetByAgent(business_id, agent_id);
        }
        
        public bool Delete(string business_id, string id)
        {
            if (!string.IsNullOrWhiteSpace(business_id) && !string.IsNullOrWhiteSpace(id))
            {
                return _shortcutRepository.Delete(business_id, id);
            }
            return false;
        }

        public Shortcut GetById(string business_id, string id)
        {
            return _shortcutRepository.GetById(business_id, id);
        }
    }
}
