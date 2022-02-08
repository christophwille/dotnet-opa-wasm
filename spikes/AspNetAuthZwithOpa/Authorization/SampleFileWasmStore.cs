using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;

namespace AspNetAuthZwithOpa.Authorization
{
    public class SampleFileWasmStore : IOpaWasmStore
    {
        private readonly ILogger<SampleFileWasmStore> _logger;
        private readonly string _wasmLoadPath;

        public SampleFileWasmStore(ILogger<SampleFileWasmStore> logger)
        {
            _logger = logger;
            // Set default load path
            _wasmLoadPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public Task<byte[]> LoadPolicyModuleAsync(string name)
        {
            string fileName = Path.Combine(_wasmLoadPath, name + ".wasm");
            return File.ReadAllBytesAsync(fileName);
        }
    }
}
