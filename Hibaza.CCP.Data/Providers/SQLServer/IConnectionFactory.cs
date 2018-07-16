using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Providers.SQLServer
{
    public interface IConnectionFactory : IDisposable
    {
        IDbConnection GetConnection { get; }
    }
}
