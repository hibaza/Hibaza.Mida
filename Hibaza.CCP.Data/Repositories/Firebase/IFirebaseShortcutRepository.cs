using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public interface IFirebaseShortcutRepository : IGenericRepository<Shortcut>
    {
        Task<dynamic> GetShortcuts(string business_id, Paging page);
    }
}
