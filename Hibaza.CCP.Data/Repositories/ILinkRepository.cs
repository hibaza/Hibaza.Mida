using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface ILinkRepository
    {
        Link GetById(string business_id, string id);
        IEnumerable<Link> GetAll();
        bool Insert(Link entity);
        bool Delete(string id);
        bool Update(Link entity);
        Task<IEnumerable<Link>> GetLinks(string business_id, string channel_id, Paging page);
        void CreateLink(Link link);
        bool UpdateStatus(string business_id, string id, string status);
        bool UpdateTimestamp(string business_id, string id, long timestamp);
    }
}
