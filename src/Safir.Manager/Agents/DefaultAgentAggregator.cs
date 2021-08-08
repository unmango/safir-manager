using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Safir.Agent.Protos;

namespace Safir.Manager.Agents
{
    internal class DefaultAgentAggregator
    {
        private readonly IEnumerable<IAgent> _agents;

        public DefaultAgentAggregator(IEnumerable<IAgent> agents)
        {
            _agents = agents ?? throw new ArgumentNullException(nameof(agents));
        }

        public IAsyncEnumerable<FileSystemEntry> ListAsync(CancellationToken cancellationToken)
        {
            return _agents.ToAsyncEnumerable()
                .SelectMany(x => x.FileSystem.ListAsync(cancellationToken))
                // Janky DistinctBy
                .GroupBy(x => x.Path)
                .SelectAwait(x => x.FirstAsync(cancellationToken));
        }
    }
}
