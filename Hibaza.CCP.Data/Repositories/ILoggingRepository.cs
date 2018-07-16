using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface ILoggingRepository
    {
        Log GetById(string id);
        IEnumerable<Log> GetAll();
        void Add(Log entity);
        bool Delete(string id);
        void Update(Log entity);
        IEnumerable<Log> GetLogs(Paging page);
    }
}
