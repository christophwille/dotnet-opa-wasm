using Opa.Wasm;
using System.Text.Json;
using Wasmtime;

namespace HigherLevelApisSpike;

public class Policy : IDisposable
{
    private bool disposedValue;
    private readonly IOpaSerializer _serde;
    public OpaPolicy Opa { get; }

    public Policy(OpaPolicy opaPolicy, IOpaSerializer serde)
    {
        Opa = opaPolicy;
        _serde = serde;
    }

    public void SetData(object o)
    {
        this.Opa.SetData(JsonSerializer.Serialize(o));
    }

    public string Evaluate(object o)
    {
        return this.Opa.Evaluate(JsonSerializer.Serialize(o));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Opa.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public interface IPolicyStore
{
    Task<(byte[], bool)> LoadPolicyAsync(string name, bool throwOnLoadError = false);
}

public class DummyPolicyStore : IPolicyStore
{
    public Task<(byte[], bool)> LoadPolicyAsync(string name, bool throwOnLoadError = false)
    {
        throw new NotImplementedException();
    }
}

public interface IOpaSerializer
{
    string Serialize(object obj);
    T? Deserialize<T>(string json);
}

public class DefaultOpaSerializer : IOpaSerializer
{
    public T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

    public string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj);
    }
}

// Size of module cache is determined by implementation
public interface IOpaModuleCache
{
    public Module? Get(string policyName);
    public void Add(string policyName, Module module);
}

public class NoOpModuleCache : IOpaModuleCache
{
    public void Add(string policyName, Module module)
    {
    }

    public Module? Get(string policyName)
    {
        return null;
    }
}

public class PolicyFactory
{
    private readonly IPolicyStore _store;
    private readonly IOpaSerializer _serde;
    private readonly IOpaModuleCache _moduleCache;

    public PolicyFactory(IPolicyStore store, IOpaModuleCache moduleCache, IOpaSerializer serde)
    {
        _store = store;
        _serde = serde;
        _moduleCache = moduleCache;
    }

    public async Task<Policy> GetAsync(string policyName)
    {
        using var opaRuntime = new OpaRuntime();

        Module? module = null;
        if (null == (module = _moduleCache.Get(policyName)))
        {
            // Non-cache case
            var (wasmBytes, err) = await _store.LoadPolicyAsync(policyName);

            // disposing is the duty of the consumer
            module = opaRuntime.Load(policyName, wasmBytes);

            _moduleCache.Add(policyName, module);
        }

        // disposing is the duty of the consumer
        var opaPolicy = new OpaPolicy(opaRuntime, module);

        return new Policy(opaPolicy, _serde);
    }
}
