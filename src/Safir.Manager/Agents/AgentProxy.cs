using System;
using Grpc.Net.ClientFactory;
using Safir.Agent.Client;

namespace Safir.Manager.Agents
{
    public class AgentProxy : IAgent
    {
        private readonly Lazy<IFileSystemClient> _fileSystem;
        private readonly Lazy<IHostClient> _host;

        public AgentProxy(string name, GrpcClientFactory factory)
        {
            _fileSystem = new(() => factory.CreateFileSystemClient(name));
            _host = new(() => factory.CreateHostClient(name));
        }

        public IFileSystemClient FileSystem => _fileSystem.Value;

        public IHostClient Host => _host.Value;
    }
}
