using Hibaza.CCP.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IChannelRepository _channelRepository;
        private readonly ILinkRepository _linkRepository;
        private readonly ITaskRepository _taskRepository;
        public UnitOfWork(IChannelRepository channelRepository, ILinkRepository linkRepository, ITaskRepository taskRepository)
        {
            _channelRepository = channelRepository;
            _linkRepository = linkRepository;
            _taskRepository = taskRepository;
        }

        public IChannelRepository ChannelRepository
        {
            get
            {
                return _channelRepository;
            }
        }

        public ILinkRepository LinkRepository
        {
            get
            {
                return _linkRepository;
            }
        }

        public void Commit()
        {
           
        }

        public void Dispose()
        {
           
        }
    }
}
