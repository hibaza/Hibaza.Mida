using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface IChannelService
    {
        Task<IEnumerable<Channel>> GetChannels(string business_id, int pageIndex, int pageSize);

        Task<List<Channel>> GetChannelsType(string business_id, string type, int pageIndex, int pageSize);

        Task<List<Channel>> GetChannelsByExtId(string ext_id);

        string Create(Domain.Entities.Channel data);
        string UpsertId(Domain.Entities.Channel data);
        Channel GetById(string business_id, string id);
        bool Delete(string business_id, string id);
        Task<List<Channel>> GetByIdFromTrunk(string business_id, string trunk);
    }
}
