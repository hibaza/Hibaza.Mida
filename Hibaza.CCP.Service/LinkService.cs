using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using Hibaza.CCP.Domain.Models.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public class LinkService : ILinkService
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IMessageService _messageService;
        private readonly ICustomerService _userService;
        public LinkService(ILinkRepository linkRepository, IMessageService messageService, ICustomerService userService)
        {
            _linkRepository = linkRepository;
            _messageService = messageService;
            _userService = userService;
        }

        public bool UpdateStatus(string business_id, string id, string status)
        {
            _linkRepository.UpdateStatus(business_id, id, status);
            return true;
        }

        public Link GetById(string business_id, string id)
        {
            return _linkRepository.GetById(business_id, id);
        }

        public bool UpdatTimestamp(string buiness_id, string id, long timestamp)
        {
            return _linkRepository.UpdateTimestamp(buiness_id, id, timestamp);
        }


        public bool Insert(Link link)
        {
            var l = GetById(link.business_id, link.id);
            if (l == null)
            {
                _linkRepository.Insert(link);
                return true;
            }
            return false;
        }

        public bool CreateLink(Link link)
        {
           _linkRepository.CreateLink(link);
            return true;
        }

        public async Task<IEnumerable<Link>> GetLinks(string business_id, string channel_id, Paging page)
        {
            return await _linkRepository.GetLinks(business_id, channel_id, page);
        }

        public IEnumerable<Link> GetAll()
        {
            return _linkRepository.GetAll();
        }

    }
}
