using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public interface IFirebaseBusinessRepository
    {
        Business GetById(string id);
        IEnumerable<Business> GetAll();
        void Add(Business entity);
        bool Delete(string id);
        void Update(Business entity);
        IEnumerable<Business> GetBusinesses(Paging page);
        bool DeleteData(string id);
    }
}
