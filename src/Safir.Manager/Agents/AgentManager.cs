using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Safir.Agent.Client.DependencyInjection;
using Safir.Manager.Configuration;

namespace Safir.Manager.Agents
{
    internal class AgentManager : IAgents, IDisposable
    {
        private readonly ILogger<AgentManager> _logger;
        private readonly Lazy<Dictionary<string, IAgent>> _agents;
        private readonly IDisposable _changeToken;

        public AgentManager(IOptionsMonitor<ManagerOptions> optionsMonitor, ILogger<AgentManager> logger)
        {
            if (optionsMonitor == null) throw new ArgumentNullException(nameof(optionsMonitor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _agents = new(() => CreateAgents(optionsMonitor.CurrentValue));
            _changeToken = optionsMonitor.OnChange(o => {
                if (_agents.IsValueCreated) CreateAgents(o, _agents.Value);
            });
        }

        public IAgent this[string name] => _agents.Value[name];

        public IEnumerator<IAgent> GetEnumerator() => _agents.Value.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose() => _changeToken.Dispose();

        // Impure BS, but DRY
        private Dictionary<string, IAgent> CreateAgents(ManagerOptions options, Dictionary<string, IAgent>? existing = null)
        {
            var agents = existing ?? new Dictionary<string, IAgent>();
            _logger.LogTrace("Creating agent proxies");

            foreach (var config in options.Agents)
            {
                _logger.LogDebug($"Creating agent proxy for {config.BaseUrl}");
                agents[config.BaseUrl] = CreateAgent(config, config.BaseUrl);
            }

            _logger.LogTrace("Finished creating agent proxies");
            return agents;
        }

        private static IAgent CreateAgent(AgentOptions options, string name)
        {
            // TODO: Does this belong in the factory?
            // Could change factory to accept a name and options.
            // Abstract away, and then also potential to re-use service collections
            var services = new ServiceCollection()
                .AddSafirAgentClient(name, o => {
                    o.Address = new Uri(options.BaseUrl);
                })
                .AddSingleton<AgentFactory>()
                .BuildServiceProvider();

            return services.GetRequiredService<AgentFactory>()
                .Create(name);
        }
    }
}
