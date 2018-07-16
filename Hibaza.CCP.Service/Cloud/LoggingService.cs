using Hibaza.CCP.Data.Infrastructure;
using Hibaza.CCP.Data.Repositories;
using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Service
{
    public class LoggingService : ILoggingService
    {
        private readonly ILoggingRepository _logRepository;
        public LoggingService(ILoggingRepository logRepository)
        {
            _logRepository = logRepository;
        }
        public string Create(Log data)
        {
            _logRepository.Add(data);
            return "done";
        }

        public IEnumerable<Log> GetLogs(Paging page)
        {
            return _logRepository.GetLogs(page);
        }
        public async System.Threading.Tasks.Task<string> CreateAsync(Log data)
        {
            _logRepository.Add(data);
            return "done";
        }
    }
}
