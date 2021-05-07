using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Safir.Agent.Protos;
using Safir.Common.ConnectionPool;
using Safir.Messaging;
using StackExchange.Redis;

namespace Safir.Manager.Events
{
    public class TestService : IHostedService
    {
        private readonly IConnectionPool<IConnectionMultiplexer> _connectionPool;
        private readonly IEventBus _eventBus;
        private readonly ILogger<TestService> _logger;

        public TestService(
            IConnectionPool<IConnectionMultiplexer> connectionPool,
            IEventBus eventBus,
            ILogger<TestService> logger)
        {
            _connectionPool = connectionPool;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // _eventBus.GetObservable<FileCreated>().Subscribe(x => {
            //     _logger.LogInformation("File created event: {Path}", x.Path);
            // });

            var connection = await _connectionPool.GetConnectionAsync(cancellationToken);
            var subscriber = connection.GetSubscriber();
            await subscriber.SubscribeAsync(typeof(FileCreated).Name, (channel, value) => {
                _logger.LogInformation(value);
            });

            _logger.LogInformation("Exiting test service start async");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
