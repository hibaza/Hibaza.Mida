using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IHotlineRepository
    {
        Task<Phone> GetById(string business_id, string id);
        Task<List<Phone>> GetAll(string business_id, Paging page);
        Task<dynamic> Upsert(Phone note);
        Task<dynamic> getCustomerInfoFromPhone(string phone);
    }
}
