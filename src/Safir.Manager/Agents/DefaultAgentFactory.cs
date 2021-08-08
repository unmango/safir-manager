using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Safir.Agent.Client;
using Safir.Agent.Client.DependencyInjection;
using Safir.Manager.Configuration;

namespace Safir.Manager.Agents
{
    internal class DefaultAgentFactory
    {
        private readonly ManagerOptions _options;
        
        public DefaultAgentFactory(IOptions<ManagerOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public IEnumerable<IAgent> CreateAll()
        {
            return _options.Agents.Select(Create);
        }

        private static IAgent Create(SafirAgentClientOptions options)
        {
            var services = new ServiceCollection()
                .AddSafirAgentClient()
                .AddSingleton(Options.Create(options))
                .AddSingleton<IAgent, AgentProxy>()
                .BuildServiceProvider();

            return services.GetRequiredService<IAgent>();
        }
    }
}
