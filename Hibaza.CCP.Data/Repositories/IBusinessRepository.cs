using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IBusinessRepository
    {
        Business GetById(string id);
        Business GetByEmail(string email);
        IEnumerable<Business> GetAll();
        void Upsert(Business business);
        bool Delete(string id);
        IEnumerable<Business> GetBusinesses(Paging page);
        Task<Business> GetBusinessFromTokenClient(string token_client);
    }
}
