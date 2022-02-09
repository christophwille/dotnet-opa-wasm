using Opa.Wasm;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AspNetAuthZwithOpa.Authorization
{
    // TODO: IDisposable for _opaModules
    public class SampleFactory
    {
        private readonly ConcurrentDictionary<string, byte[]> _wasmCache = new();
        private readonly ConcurrentDictionary<string, OpaPolicyModule> _opaModules = new();
        private readonly IOpaWasmStore _store;

        public SampleFactory(IOpaWasmStore store)
        {
            _store = store;
        }

        public async Task<bool> EnsurePolicyWasmLoadedAsync(string name)
        {
            if (_wasmCache.ContainsKey(name))
            {
                return true;
            }

            // Needs: exception handling
            var bytes = await _store.LoadPolicyModuleAsync(name);
            _wasmCache.TryAdd(name, bytes);

            return true;
        }

        // Should that be public at all?
        public OpaPolicyModule GetModuleRef(string name)
        {
            if (_opaModules.TryGetValue(name, out var module))
            {
                return module;
            }

            if (_wasmCache.TryGetValue(name, out var bytes))
            {
                var newModule = OpaPolicyModule.Load(name, bytes);
                _opaModules.TryAdd(name, newModule);
                return newModule;
            }

            throw new ArgumentException(nameof(name), "Policy bytes not cached, did you call EnsurePolicyWasmBytesLoadedAsync?");
        }

        // Could be async to avoid calling EnsurePolicyWasmLoadedAsync separately
        // Intention of EnsurePolicyWasmLoadedAsync
        //     - a global preload could also be done and calling this method would be unnecessary
        //     - have an entirely sync code path for policy evaluation starting with CreatePolicyInstance
        // Non-async might be pointless anyways if SetData/input requires db data access or the like
        public OpaPolicy CreatePolicyInstance(string name)
        {
            var module = GetModuleRef(name);
            return module.CreatePolicyInstance();
        }
    }
}
