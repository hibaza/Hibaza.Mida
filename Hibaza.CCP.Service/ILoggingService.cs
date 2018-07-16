using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public interface ILoggingService
    {
        System.Threading.Tasks.Task<string> CreateAsync(Log data);
        string Create(Log data);
        IEnumerable<Log> GetLogs(Paging page);
    }
}
