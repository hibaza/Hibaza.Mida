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
    public class NodeService : INodeService
    {
        private readonly INodeRepository _nodeRepository;
        private readonly IMessageService _messageService;
        private readonly ICustomerService _userService;
        public NodeService(INodeRepository nodeRepository, IMessageService messageService, ICustomerService userService)
        {
            _nodeRepository = nodeRepository;
            _messageService = messageService;
            _userService = userService;
        }

        public bool UpdateStatus(string business_id, string id, string status)
        {
            _nodeRepository.UpdateStatus(business_id, id, status);
            return true;
        }


        public bool CreateNode(Node node)
        {
           _nodeRepository.CreateNode(node);
            return true;
        }

        public async Task<IEnumerable<Node>> GetPendingNodes(string business_id, string channel_id, string type, Paging page)
        {
            return await _nodeRepository.GetPendingNodes(business_id, channel_id, type, page);
        }

        public IEnumerable<Node> GetAll()
        {
            return _nodeRepository.GetAll();
        }

    }
}
