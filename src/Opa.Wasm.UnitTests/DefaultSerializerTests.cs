using NUnit.Framework;

namespace Opa.Wasm.UnitTests
{
	public class DefaultSerializerTests
	{
		[Test]
		public void HelloWorldTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = module.CreatePolicyInstance();

			opaPolicy.SetData(new { world = "world" });

			var output = opaPolicy.Evaluate<bool>(
				new { message = "world" },
				disableFastEvaluate: true);

			Assert.That(output.Value, Is.True);
		}

		[Test]
		public void ExampleOneEntrypointByNumberMultiTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = module.CreatePolicyInstance();

			var input = new ExampleOneInputModel(SomeProp: "thisValue", AnotherProp: "thatValue");
			var result = opaPolicy.Evaluate<ExampleOneResultModel>(input, 1);

			// [{"result":{"myOtherRule":true,"myRule":true,"myCompositeRule":true}}]
			Assert.That(result.Value.MyOtherRule, Is.True);
			Assert.That(result.Value.myRule, Is.True);
			Assert.That(result.Value.MyCompositeRule, Is.True);
		}

		[Test]
		public void ExampleOneEntrypointByNameMultiTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = module.CreatePolicyInstance();

			var input = new ExampleOneInputModel(SomeProp: "thisValue", AnotherProp: "thatValue");
			var result = opaPolicy.Evaluate<ExampleOneResultModel>(input, "example/one");

			// [{"result":{"myOtherRule":true,"myRule":true,"myCompositeRule":true}}]
			Assert.That(result.Value.MyOtherRule, Is.True);
			Assert.That(result.Value.myRule, Is.True);
			Assert.That(result.Value.MyCompositeRule, Is.True);
		}
	}

	record ExampleOneInputModel(string SomeProp, string AnotherProp);
	record ExampleOneResultModel(bool MyOtherRule, bool myRule, bool MyCompositeRule);
}
