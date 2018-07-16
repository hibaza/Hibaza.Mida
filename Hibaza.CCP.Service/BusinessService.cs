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
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _businessRepository;
        public BusinessService(IBusinessRepository businessRepository)
        {
            _businessRepository = businessRepository;
        }

        public string Create(Business data)
        {
            _businessRepository.Upsert(data);
            return data.id;
        }
        public async Task<IEnumerable<Business>> All()
        {
            var list = _businessRepository.GetAll();
            return list;
        }

        public async Task<IEnumerable<Business>> GetBusinesses(int pageIndex, int pageSize)
        {
            var list = _businessRepository.GetBusinesses(new Domain.Models.Paging { Limit = pageSize });
            return list;
        }

        public bool Delete(string id)
        {
            return _businessRepository.Delete(id);
        }

        public Business GetById(string id)
        {
            return _businessRepository.GetById(id);
        }
        public Business GetByEmail(string email)
        {
            return _businessRepository.GetByEmail(email);
        }
        public async Task<Business> GetBusinessFromTokenClient(string token_client)
        {
         return  await  _businessRepository.GetBusinessFromTokenClient(token_client);
        }
    }
}
