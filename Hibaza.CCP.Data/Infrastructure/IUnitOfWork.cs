using Hibaza.CCP.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Infrastructure
{

    public interface IUnitOfWork : IDisposable
    {
        IChannelRepository ChannelRepository { get; }
        ILinkRepository LinkRepository { get; }
        void Commit();
    }
}
