using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface INodeService
    {
        Task<IEnumerable<Node>> GetPendingNodes(string business_id, string channel_id, string type, Paging page);
        bool CreateNode(Node node);
        bool UpdateStatus(string business_id, string id, string status);
    }
}
