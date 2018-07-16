using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface INodeRepository
    {
        Node GetById(string business_id, string id);
        IEnumerable<Node> GetAll();
        bool Insert(Node node);
        bool Delete(string id);
        bool Update(Node entity);
        Task<IEnumerable<Node>> GetNodes(string business_id, string channel_id, Paging page);
        Task<IEnumerable<Node>> GetPendingNodes(string business_id, string channel_id, string type, Paging page);
        void CreateNode(Node link);
        bool UpdateStatus(string business_id, string id, string status);
    }
}
