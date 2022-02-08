namespace AspNetAuthZwithOpa.Authorization
{
    public class SamplePolicy
    {
        private readonly SampleFactory _factory;

        public SamplePolicy(SampleFactory factory)
        {
            _factory = factory;
        }

        public bool Evaluate(/* provide whatever input you want here */)
        {
            // Module is owned by Factory
            var module = _factory.Get("example");

            using var policy = module.CreatePolicyInstance();
            policy.SetData(new SamplePolicyData("world"));
            var result = policy.Evaluate<bool>(new SamplePolicyInput("world"));

            return result.Value;
        }
    }

    record SamplePolicyData(string World);
    record SamplePolicyInput(string Message);
}
