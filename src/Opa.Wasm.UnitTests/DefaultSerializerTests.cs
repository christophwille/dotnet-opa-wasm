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

			Assert.IsTrue(output.Value);
		}

		[Test]
		public void ExampleOneEntrypointByNumberMultiTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = module.CreatePolicyInstance();

			var input = new ExampleOneInputModel(SomeProp: "thisValue", AnotherProp: "thatValue");
			var result = opaPolicy.Evaluate<ExampleOneResultModel>(input, 1);

			// [{"result":{"myOtherRule":true,"myRule":true,"myCompositeRule":true}}]
			Assert.IsTrue(result.Value.MyOtherRule);
			Assert.IsTrue(result.Value.myRule);
			Assert.IsTrue(result.Value.MyCompositeRule);
		}

		[Test]
		public void ExampleOneEntrypointByNameMultiTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = module.CreatePolicyInstance();

			var input = new ExampleOneInputModel(SomeProp: "thisValue", AnotherProp: "thatValue");
			var result = opaPolicy.Evaluate<ExampleOneResultModel>(input, "example/one");

			// [{"result":{"myOtherRule":true,"myRule":true,"myCompositeRule":true}}]
			Assert.IsTrue(result.Value.MyOtherRule);
			Assert.IsTrue(result.Value.myRule);
			Assert.IsTrue(result.Value.MyCompositeRule);
		}
	}

	record ExampleOneInputModel(string SomeProp, string AnotherProp);
	record ExampleOneResultModel(bool MyOtherRule, bool myRule, bool MyCompositeRule);
}
