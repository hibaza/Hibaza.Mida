using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IAttachmentRepository
    {
        Attachment GetById(string business_id, string channel_id, string id);
        Task<IEnumerable<Attachment>> GetAll(string business_id, string channel_id, Paging page);
        bool Upsert(Attachment attachment);
        bool Delete(string business_id, string channel_id, string id);
        bool Insert(Attachment attachment);
        bool Update(Attachment attachment);
        Task<IEnumerable<Attachment>> GetAttachments(string business_id, string channel_id, string product_id, Paging page);
    }
}
