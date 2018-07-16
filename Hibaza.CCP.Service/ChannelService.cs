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
    public class ChannelService : IChannelService
    {
        private readonly IChannelRepository _channelRepository;
        public ChannelService(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public static string FormatId(string parent, string key)
        {
            return Core.Helpers.CommonHelper.FormatKey(parent, key);
        }

        public string Create(Channel data)
        {
            _channelRepository.Add(data);
            if (!string.IsNullOrWhiteSpace(data.business_id))
            {
                _channelRepository.Add(data);
            }
            return data.id;
        }

        public string UpsertId(Channel data)
        {
            _channelRepository.UpsertId(data);            
            return data.id;
        }

        public async Task<IEnumerable<Channel>> GetChannels(string business_id, int pageIndex, int pageSize)
        {
            return await _channelRepository.GetChannels(business_id, new Domain.Models.Paging { Limit = pageSize });

        }

        public async Task<List<Channel>> GetChannelsType(string business_id,string type,  int pageIndex, int pageSize)
        {
            return await _channelRepository.GetChannelsType(business_id, type, new Domain.Models.Paging { Limit = pageSize });

        }

        public async Task<List<Channel>> GetChannelsByExtId(string ext_id)
        {
            return await _channelRepository.GetChannelsByExtId(ext_id, new Domain.Models.Paging { Limit = 50 });
        }

        public bool Delete(string business_id, string id)
        {
            if (!string.IsNullOrWhiteSpace(business_id) && !string.IsNullOrWhiteSpace(id))
            {
                return _channelRepository.Delete(business_id, id);
            }
            else return false;
        }


        public Channel GetById(string business_id, string id)
        {
            return _channelRepository.GetById(business_id, id);
        }

        public async Task<List<Channel>> GetByIdFromTrunk(string business_id, string trunk)
        {
            return await _channelRepository.GetByIdFromTrunk(business_id, trunk);
        }
        
    }
}
