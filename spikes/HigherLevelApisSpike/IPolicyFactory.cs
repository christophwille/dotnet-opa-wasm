using Opa.Wasm;
using System.Text.Json;
using Wasmtime;

namespace HigherLevelApisSpike;

public class Policy : IDisposable
{
    private bool disposedValue;

    internal Module? _module;
    internal string _policyName;

    public OpaPolicy Opa { get; }

    public Policy(OpaPolicy opaPolicy, Module module, string policyName)
    {
        Opa = opaPolicy;

        _module = module;
        _policyName = policyName;
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

public class PolicyFactory
{
    private readonly IPolicyStore _store;

    // inject cache if any, inject opa wasm loader, inject data service (SetData) if any
    // inject serializer if any
    public PolicyFactory(IPolicyStore store)
    {
        _store = store;
    }

    // Idea from ArrayPool
    public async Task<Policy> RentAsync(string policyName)
    {
        // disposing of those objects is the duty of the consumer
        var opaRuntime = new OpaRuntime();

        // Non-cache case
        var (wasmBytes, err) = await _store.LoadPolicyAsync(policyName);
        var module = opaRuntime.Load(policyName, wasmBytes);

        var opaPolicy = new OpaPolicy(opaRuntime, module);

        // remove from cache if we are using it

        // opaPolicy.SetData - from either the Rent param or maybe a service?

        return new Policy(opaPolicy, module, policyName);
    }

    // intentionally void because the cache if any is going to be on the local machine!
    public void Return(Policy p)
    {
        // re-add to cache if we do caching
        var moduleToReturn = p._module;
        p._module = null;

        var nameofPolicy = p._policyName;

        // DO MAGIC
    }
}
