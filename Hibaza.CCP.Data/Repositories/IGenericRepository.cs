using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity GetById(string business_id, string id);
        IEnumerable<TEntity> GetAll(string business_id);
        void Upsert(string business_id, TEntity entity);
        bool Delete(string business_id, string id);
        void Update(string business_id, TEntity entity);
    }
}
