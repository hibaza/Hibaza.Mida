using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IChannelRepository
    {
        void Add(Channel channel);
        void UpsertId(Channel channel);
        Channel GetById(string business_id, string id);
        IEnumerable<Channel> GetAll(string business_id);
        bool Delete(string business_id, string id);

        Task<IEnumerable<Channel>> GetChannels(string business_id, Paging page);
        Task<List<Channel>> GetChannelsByExtId(string ext_id, Paging page);
        Task<List<Channel>> GetByIdFromTrunk(string business_id, string trunk);
        Task<List<Channel>> GetChannelsType(string business_id, string type, Paging page);
        
    }
}
