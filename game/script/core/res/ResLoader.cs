using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace core.res;

/// <summary>
/// Singleton class responsible for asynchronously loading Godot resources
/// </summary>
public class ResLoader
{
    // Singleton instance
    private static ResLoader? _instance;
    private static readonly object Lock = new();

    // Tracks currently loading resources and their completion sources
    private readonly Dictionary<string, TaskCompletionSource<Resource>?> _loadingDict = new();

    /// <summary>
    /// Thread-safe singleton accessor
    /// </summary>
    public static ResLoader Instance
    {
        get
        {
            if (_instance != null) return _instance;
            lock (Lock)
            {
                _instance ??= new ResLoader();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Clears all pending resource loads
    /// </summary>
    public void Clear()
    {
        _loadingDict.Clear();
    }

    /// <summary>
    /// Waits for a resource to finish loading and returns it when complete
    /// </summary>
    /// <param name="path">Resource path</param>
    /// <param name="tcs">TaskCompletionSource tracking the load</param>
    /// <returns>Loaded resource of type T, or null if loading failed</returns>
    private async Task<T?> WaitForResource<T>(string path, TaskCompletionSource<Resource>? tcs = null)
        where T : Resource
    {
        // Early exit if no valid TaskCompletionSource
        if (tcs == null && !_loadingDict.TryGetValue(path, out tcs)) return null;
        if (tcs == null) return null;

        var resource = await tcs.Task;
        _loadingDict.Remove(path);

        return resource is T result ? result : null;
    }

    /// <summary>
    /// Checks the loading status of all pending resources. 
    /// TODO: Find a way to be called from main thread each frame.
    /// </summary>
    internal void CheckResourceLoading()
    {
        if (_loadingDict.Count == 0) return;

        foreach (var (path, tcs) in _loadingDict)
        {
            var status = ResourceLoader.LoadThreadedGetStatus(path);
            switch (status)
            {
                case ResourceLoader.ThreadLoadStatus.InProgress:
                    continue;
                case ResourceLoader.ThreadLoadStatus.InvalidResource:
                case ResourceLoader.ThreadLoadStatus.Failed:
                    GD.PrintErr($"Failed to load resource: {path}");
                    tcs?.TrySetCanceled();
                    break;
                case ResourceLoader.ThreadLoadStatus.Loaded:
                    tcs?.TrySetResult(ResourceLoader.LoadThreadedGet(path));
                    break;
            }
        }
    }

    /// <summary>
    /// Asynchronously loads a resource of type T
    /// </summary>
    /// <param name="path">Path to the resource</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Loaded resource of type T, or null if loading failed</returns>
    public async Task<T?> LoadResource<T>(string path, CancellationToken cancellationToken = default) where T : Resource
    {
        if (!ResourceLoader.Exists(path)) return null;

        var status = ResourceLoader.LoadThreadedGetStatus(path);
        switch (status)
        {
            case ResourceLoader.ThreadLoadStatus.InvalidResource:
                ResourceLoader.LoadThreadedRequest(path);
                var tcs = new TaskCompletionSource<Resource>(cancellationToken);
                _loadingDict.TryAdd(path, tcs);
                return await WaitForResource<T>(path, tcs);

            case ResourceLoader.ThreadLoadStatus.InProgress:
                var tcsInProgress = _loadingDict.TryGetValue(path, out var existing)
                    ? existing
                    : new TaskCompletionSource<Resource>(cancellationToken);

                if (tcsInProgress == null) return null;
                if (!_loadingDict.ContainsKey(path))
                {
                    _loadingDict.TryAdd(path, tcsInProgress);
                }

                return await WaitForResource<T>(path, tcsInProgress);

            case ResourceLoader.ThreadLoadStatus.Failed:
                GD.PrintErr($"Failed to load resource: {path}");
                return null;

            case ResourceLoader.ThreadLoadStatus.Loaded:
                var resource = ResourceLoader.LoadThreadedGet(path);
                return resource is T result ? result : null;
        }

        return null;
    }
}