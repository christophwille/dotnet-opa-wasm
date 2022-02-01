using HigherLevelApisSpike;

var factory = new PolicyFactory();

var policy = factory.Rent("mysamplepolicy");

// if you want to be fancy use the string version on the root object
policy.Opa.SetData("");
policy.SetData(new { blah = "blups" });

policy.Evaluate(new { myName = "" });
