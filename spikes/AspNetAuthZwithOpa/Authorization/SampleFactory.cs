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

        public virtual string GenerateCacheKeyForModuleBytes(string name)
        {
            return "OPA_AuthZ_Policies_CachedBytes_" + name;
        }

        public virtual string GenerateCacheKeyForPolicyModule(string name)
        {
            return "OPA_AuthZ_Policies_CachedModule_" + name;
        }

        public async Task<bool> EnsurePolicyWasmLoadedAsync(string name)
        {
            string cacheKey = GenerateCacheKeyForModuleBytes(name);
            if (_wasmCache.ContainsKey(cacheKey))
            {
                return true;
            }

            var bytes = await _store.LoadPolicyModuleAsync(name);
            _wasmCache.TryAdd(cacheKey, bytes);

            return true;
        }

        public OpaPolicyModule GetModuleRef(string name)
        {
            string modcacheKey = GenerateCacheKeyForPolicyModule(name);
            if (_opaModules.TryGetValue(modcacheKey, out var module))
            {
                return module;
            }

            string cacheKey = GenerateCacheKeyForModuleBytes(name);
            if (_wasmCache.TryGetValue(cacheKey, out var bytes))
            {
                var newModule = OpaPolicyModule.Load(name, bytes);
                _opaModules.TryAdd(modcacheKey, newModule);
                return newModule;
            }

            throw new ArgumentException(nameof(name), "Policy bytes not cached, did you call EnsurePolicyWasmBytesLoadedAsync?");
        }

        public OpaPolicy CreatePolicyInstance(string name)
        {
            var module = GetModuleRef(name);
            return module.CreatePolicyInstance();
        }
    }
}
