using Opa.Wasm;
using System.Text.Json;
using Wasmtime;

namespace HigherLevelApisSpike;

public class Policy : IDisposable
{
    private bool disposedValue;

    internal Module? _module;
    internal string _policyName;
    private readonly IOpaSerializer _serde;

    public OpaPolicy Opa { get; }

    public Policy(OpaPolicy opaPolicy, Module module, string policyName, IOpaSerializer serde)
    {
        Opa = opaPolicy;

        _module = module;
        _policyName = policyName;

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
                // TODO: dispose managed state (managed objects)

                // When disposing, we are not telling the PolicyFactory
                Opa.Dispose();
                if (null != _module) _module.Dispose();
                _module = null;
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
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
public interface IModuleCache
{
    public Module? GetAndRemove(string policyName);
    public void Add(string policyName, Module module);
}

public class NoOpModuleCache : IModuleCache
{
    public void Add(string policyName, Module module)
    {
    }

    public Module? GetAndRemove(string policyName)
    {
        return null;
    }
}

public class PolicyFactory
{
    private readonly IPolicyStore _store;
    private readonly IOpaSerializer _serde;
    private readonly IModuleCache _moduleCache;

    public PolicyFactory(IPolicyStore store, IModuleCache moduleCache, IOpaSerializer serde)
    {
        _store = store;
        _serde = serde;
        _moduleCache = moduleCache;
    }

    // Idea from ArrayPool
    public async Task<Policy> RentAsync(string policyName)
    {
        using var opaRuntime = new OpaRuntime();

        Module? module = null;
        if (null == (module = _moduleCache.GetAndRemove(policyName)))
        {
            // Non-cache case
            var (wasmBytes, err) = await _store.LoadPolicyAsync(policyName);

            // disposing is the duty of the consumer
            module = opaRuntime.Load(policyName, wasmBytes);
        }

        // disposing is the duty of the consumer
        var opaPolicy = new OpaPolicy(opaRuntime, module);

        return new Policy(opaPolicy, module, policyName, _serde);
    }

    // intentionally void because the cache if any is going to be on the local machine!
    public void Return(Policy p)
    {
        var moduleToReturn = p._module;
        if (null == moduleToReturn) return;

        p._module = null;

        var nameofPolicy = p._policyName;
        _moduleCache.Add(nameofPolicy, moduleToReturn);

        p.Dispose();
    }
}
