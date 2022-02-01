using HigherLevelApisSpike;

var factory = new PolicyFactory(
                    new DummyPolicyStore(),
                    new NoOpModuleCache(),
                    new DefaultOpaSerializer());

var policy = await factory.GetAsync("mysamplepolicy");

// if you want to be fancy use the string version on the root object
// policy.Opa.SetData("");
policy.SetData(new { blah = "blups" });

policy.Evaluate(new { myName = "" });

