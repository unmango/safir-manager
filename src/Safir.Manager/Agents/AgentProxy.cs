using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Safir.Agent.Client;
using Safir.Agent.Protos;
using Safir.Manager.Configuration;

namespace Safir.Manager.Agents
{
    public class AgentProxy : IAgents, IAgent
    {
        private readonly IOptionsMonitor<ManagerOptions> _optionsMonitor;

        public AgentProxy(IOptionsMonitor<ManagerOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        public IFileSystemClient FileSystem { get; }

        public IHostClient Host { get; }

        public string Name => "Proxy";

        public IEnumerator<IAgent> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IAgent this[string name] => this;

        private class FileSystemProxy : IFileSystemClient
        {
            private readonly JsonFileRequestProxy _requestProxy;

            public FileSystemProxy(JsonFileRequestProxy requestProxy)
            {
                _requestProxy = requestProxy ?? throw new ArgumentNullException(nameof(requestProxy));
            }
            
            public AsyncServerStreamingCall<FileSystemEntry> ListFiles(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public IAsyncEnumerable<FileSystemEntry> ListFilesAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}
