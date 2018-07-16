using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories.Firebase
{
    public interface IFirebaseChannelRepository : IGenericRepository<Channel>
    {
        Channel GetById(string id);
        IEnumerable<Channel> GetAll();
        void Add(Channel entity);
        bool Delete(string id);
        void Update(Channel entity);
        Task<dynamic> GetChannels(Paging page);
        Task<dynamic> GetChannels(string business_id, Paging page);
        Task<dynamic> GetChannelsByExtId(string ext_id, Paging page);
    }
}
