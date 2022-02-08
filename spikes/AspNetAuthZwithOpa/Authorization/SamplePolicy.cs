using System.Threading.Tasks;

namespace AspNetAuthZwithOpa.Authorization
{
    public class SamplePolicy
    {
        private readonly SampleFactory _factory;

        public SamplePolicy(SampleFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> EvaluateAsync(/* provide whatever input you want here */)
        {
            bool isPolicyAvailable = await _factory.EnsurePolicyWasmLoadedAsync("example");
            using var policy = _factory.CreatePolicyInstance("example");

            policy.SetData(new SamplePolicyData("world"));
            var result = policy.Evaluate<bool>(new SamplePolicyInput("world"));

            return result.Value;
        }
    }

    record SamplePolicyData(string World);
    record SamplePolicyInput(string Message);
}
