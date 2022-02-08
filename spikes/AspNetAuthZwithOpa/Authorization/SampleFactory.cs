using Opa.Wasm;
using System.IO;

namespace AspNetAuthZwithOpa.Authorization
{
    public class SampleFactory
    {
        public SampleFactory()
        {
        }

        // No caching of wasm bytes, no caching of module
        public OpaPolicyModule Get(string name)
        {
            string wasmLoadPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string fileName = Path.Combine(wasmLoadPath, name + ".wasm");
            return OpaPolicyModule.Load(fileName);
        }
    }
}
