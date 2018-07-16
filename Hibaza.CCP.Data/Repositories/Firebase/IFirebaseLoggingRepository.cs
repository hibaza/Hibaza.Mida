using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public interface IFirebaseLoggingRepository
    {
        Log GetById(string id);
        IEnumerable<Log> GetAll();
        void Add(Log entity);
        bool Delete(string id);
        void Update(Log entity);
    }
}
