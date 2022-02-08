using System.Threading.Tasks;

namespace AspNetAuthZwithOpa.Authorization
{
    public interface IOpaWasmStore
    {
        Task<byte[]> LoadPolicyModuleAsync(string name);
    }
}
