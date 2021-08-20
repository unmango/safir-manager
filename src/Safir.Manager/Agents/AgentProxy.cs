using System.Collections;
using System.Collections.Generic;
using Safir.Agent.Client;

namespace Safir.Manager.Agents
{
    public class AgentProxy : IAgents, IAgent
    {

        public IFileSystemClient FileSystem { get; }

        public IHostClient Host { get; }

        public string Name { get; }

        public IEnumerator<IAgent> GetEnumerator()
        {
            yield return new AgentProxy();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IAgent this[string name] => throw new System.NotImplementedException();
    }
}
