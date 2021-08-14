using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Safir.Manager.Agents;
using Safir.Manager.Configuration;
using Xunit;

namespace Safir.Manager.Tests.Agents
{
    public class AgentManagerTests
    {
        private readonly AutoMocker _mocker = new();
        private readonly Mock<IOptionsMonitor<ManagerOptions>> _optionsMonitor;
        private readonly AgentManager _manager;

        public AgentManagerTests()
        {
            _optionsMonitor = _mocker.GetMock<IOptionsMonitor<ManagerOptions>>();
            _manager = _mocker.CreateInstance<AgentManager>();
        }

        [Fact]
        public void Throws_WhenArgsAreNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new AgentManager(null!, new Mock<ILogger<AgentManager>>().Object));

            Assert.Throws<ArgumentNullException>(
                () => new AgentManager(new Mock<IOptionsMonitor<ManagerOptions>>().Object, null!));
        }

        [Theory]
        [MemberData(nameof(AgentOptionsTestData))]
        public void EnumeratesAgents(IEnumerable<AgentOptions> agentOptions, IEnumerable<string> urls)
        {
            _optionsMonitor.SetupGet(x => x.CurrentValue)
                .Returns(new ManagerOptions {
                    Agents = agentOptions.ToList()
                });

            var result = _manager.Count();

            Assert.Equal(urls.Count(), result);
        }

        [Theory]
        [MemberData(nameof(AgentOptionsTestData))]
        public void UsesBaseUrlAsName(IEnumerable<AgentOptions> agentOptions, IEnumerable<string> urls)
        {
            _optionsMonitor.SetupGet(x => x.CurrentValue)
                .Returns(new ManagerOptions {
                    Agents = agentOptions.ToList()
                });

            foreach (var url in urls)
            {
                Assert.NotNull(_manager[url]);
            }
        }

        [Theory]
        [MemberData(nameof(AgentOptionsTestData))]
        public void SetsNameOnProxy(IEnumerable<AgentOptions> agentOptions, IEnumerable<string> urls)
        {
            _optionsMonitor.SetupGet(x => x.CurrentValue)
                .Returns(new ManagerOptions {
                    Agents = agentOptions.ToList()
                });

            foreach (var url in urls)
            {
                Assert.Equal(url, _manager[url].Name);
            }
        }

        public static IEnumerable<object[]> AgentOptionsTestData()
        {
            yield return new object[] {
                new AgentOptions[] {
                    new() { BaseUrl = "https://example.com" }
                },
                new[] { "https://example.com" }
            };

            yield return new object[] {
                new AgentOptions[] {
                    new() { BaseUrl = "https://example.com" },
                    new() { BaseUrl = "https://example2.com" }
                },
                new[] {
                    "https://example.com",
                    "https://example2.com"
                }
            };
        }

        // TODO
        // [Fact]
        // public void CreatesNewAgentWhenOptionsChange()
        // {
        //     Action<ManagerOptions>? callback = null;
        //     _optionsMonitor.Setup(x => x.OnChange(It.IsAny<Action<ManagerOptions>>()))
        //         .Callback<Action<ManagerOptions>>(x => callback = x);
        //     
        //     Assert.NotNull(callback);
        //
        //     callback!(new() {
        //         Agents = new() {
        //             new() {
        //                 BaseUrl = "https://TestUrl69"
        //             }
        //         }
        //     });
        //     
        //     Assert.NotNull(_manager["https://TestUrl69"]);
        // }
    }
}
