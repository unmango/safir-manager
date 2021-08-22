using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Safir.Manager.Agents
{
    internal class JsonFileRequestProxy
    {
        private static readonly ConcurrentDictionary<object, object?> _requestMap = new();
        private static readonly JsonSerializerOptions _serializerOptions = new();
        private readonly string _root;
        private readonly string _fileName;

        public JsonFileRequestProxy(string root, string? fileName = null)
        {
            _root = root.Trim();
            _fileName = fileName ?? "response.json";
        }

        public ValueTask<T?> RequestAsync<T>(string path, object? args = null, CancellationToken cancellationToken = default)
        {
            return RequestAsync<T>(_root, path, _fileName, args, cancellationToken);
        }

        public async IAsyncEnumerable<T> RequestAsyncEnumerable<T>(
            string path,
            object? args = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var result = await RequestAsync<List<T>>(path, args, cancellationToken);

            if (result == null) yield break;

            foreach (var fileSystemEntry in result)
                yield return fileSystemEntry;
        } 

        public static ValueTask<T?> RequestAsync<T>(
            string root,
            string path,
            string fileName,
            object? args = null,
            CancellationToken cancellationToken = default)
        {
            if (Path.EndsInDirectorySeparator(path))
                path = Path.Combine(path, fileName);
            
            var fullPath = Path.Combine(root, path);
            return _requestMap.TryGetValue(Key(fullPath, args), out var result)
                ? new ValueTask<T?>((T?)result)
                : RequestAsyncCore<T>(fullPath, args, cancellationToken);
        }

        private static async ValueTask<T?> RequestAsyncCore<T>(
            string path,
            object? args,
            CancellationToken cancellationToken = default)
        {
            if (!File.Exists(path)) return default;
            
            await using var stream = File.OpenRead(path);
            var result = await JsonSerializer.DeserializeAsync<T>(stream, _serializerOptions, cancellationToken);
            _requestMap.TryAdd(Key(path, args), result);
            return result;
        }

        private static string Key(string path, object? args) => $"{path}({args})";
    }
}
