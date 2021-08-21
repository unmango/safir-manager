using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Safir.Manager.Agents
{
    internal class JsonFileRequestProxy
    {
        private readonly Dictionary<object, object?> _requestMap = new();
        private readonly string _root;

        public JsonFileRequestProxy(string root)
        {
            _root = root.Trim() ?? throw new ArgumentNullException(nameof(root));
        }

        public ValueTask<T?> RequestAsync<T>(string path, object? args = null, CancellationToken cancellationToken = default)
        {
            return _requestMap.TryGetValue(Key(path, args), out var result)
                ? new ValueTask<T?>((T?)result)
                : RequestAsyncCore<T>(path, args, cancellationToken);
        }

        private async ValueTask<T?> RequestAsyncCore<T>(string path, object? args, CancellationToken cancellationToken = default)
        {
            await using var stream = File.OpenRead(Path.Combine(_root, path));
            var result = await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions(), cancellationToken);
            _requestMap[Key(path, args)] = result;
            return result;
        }

        private static string Key(string path, object? args) => $"{path}{args}";
    }
}
