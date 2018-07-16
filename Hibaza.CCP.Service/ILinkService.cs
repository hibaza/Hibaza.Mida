using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface ILinkService
    {
        Task<IEnumerable<Link>> GetLinks(string business_id, string channel_id, Paging page);
        bool CreateLink(Link link);
        bool Insert(Link link);
        Link GetById(string business_id, string id);
        bool UpdateStatus(string business_id, string id, string status);
        bool UpdatTimestamp(string buiness_id, string id, long timestamp);
    }
}
