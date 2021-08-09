using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Safir.Agent.Protos;
using Safir.Manager.Agents;
using Safir.Manager.Protos;

namespace Safir.Manager.Services
{
    internal class MediaService : Media.MediaBase
    {
        private readonly DefaultAgentAggregator _aggregator;

        public MediaService(DefaultAgentAggregator aggregator)
        {
            _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
        }

        public override async Task List(Empty request, IServerStreamWriter<MediaItem> responseStream, ServerCallContext context)
        {
            var files = _aggregator.ListAsync(context.CancellationToken);
            await foreach (var file in files)
            {
                await responseStream.WriteAsync(ToMedia(file));
            }
        }

        private static MediaItem ToMedia(FileSystemEntry entry)
        {
            return new() {
                Host = string.Empty, // TODO
                Path = entry.Path,
            };
        }
    }
}
