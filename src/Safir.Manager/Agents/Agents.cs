using System.Collections;
using System.Collections.Generic;

namespace Safir.Manager.Agents
{
    internal class Agents : IAgents
    {
        private readonly Dictionary<string, IAgent> _agents;

        public Agents(IEnumerable<IAgent> agents)
        {
            _agents = new();
        }

        public IEnumerator<IAgent> GetEnumerator() => _agents.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IAgent this[string name] => _agents[name];
    }
}
