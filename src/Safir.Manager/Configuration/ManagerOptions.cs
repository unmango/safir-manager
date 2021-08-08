using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Safir.Agent.Client;

namespace Safir.Manager.Configuration
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ManagerOptions
    {
        public IEnumerable<SafirAgentClientOptions> Agents { get; set; } = Enumerable.Empty<SafirAgentClientOptions>();

        public string Redis { get; set; } = string.Empty;
    }
}
