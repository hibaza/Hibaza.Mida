using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface IBusinessService
    {
        Task<IEnumerable<Business>> All();
        Task<IEnumerable<Business>> GetBusinesses(int pageIndex, int pageSize);
        string Create(Domain.Entities.Business data);
        Business GetById(string id  );
        Business GetByEmail(string email);
        bool Delete(string id);
        Task<Business> GetBusinessFromTokenClient(string token_client);
    }
}
