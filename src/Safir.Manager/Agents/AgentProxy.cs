using System;
using Safir.Agent.Client;

namespace Safir.Manager.Agents
{
    public class AgentProxy : IAgent
    {
        public AgentProxy(IFileSystemClient fileSystem, IHostClient host)
        {
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            Host = host ?? throw new ArgumentNullException(nameof(host));
        }

        public IFileSystemClient FileSystem { get; }
        
        public IHostClient Host { get; }
    }
}
