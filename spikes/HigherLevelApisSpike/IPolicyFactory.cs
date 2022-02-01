using Opa.Wasm;
using System.Text.Json;
using Wasmtime;

namespace HigherLevelApisSpike;

public class Policy : IDisposable
{
    private bool disposedValue;

    public OpaPolicy Opa { get; }

    public Policy(PolicyFactory factory, OpaRuntime opaRuntime, OpaPolicy opaPolicy, Module module, string policyName)
    {
        Opa = opaPolicy;
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

public class PolicyFactory
{
    // inject cache if any, inject opa wasm loader, inject data service (SetData) if any
    // inject serializer if any
    public PolicyFactory()
    {
    }

    // Idea from ArrayPool
    Policy Rent(string policyName)
    {
        // disposing of those objects is the duty of the consumer
        var opaRuntime = new OpaRuntime();
        var opaPolicy = new OpaPolicy(opaRuntime, module);

        // remove from cache if we are using it

        // opaPolicy.SetData - from either the Rent param or maybe a service?

        return new Policy(this, opaRuntime, opaPolicy, );
    }

    void Return(Policy p)
    {
        // re-add to cache if we do caching
    }
}
