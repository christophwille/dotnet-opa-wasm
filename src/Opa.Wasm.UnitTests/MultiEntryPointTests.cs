using NUnit.Framework;
using System;

namespace Opa.Wasm.UnitTests
{
	public class MultiEntryPointTests
	{
		[Test]
		public void DumpEntryPointsForMultiTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = module.CreatePolicyInstance();

			var entrypoints = opaPolicy.Entrypoints;

			Assert.That(entrypoints.Count, Is.EqualTo(3));
			Assert.That(entrypoints["example/one"], Is.EqualTo(1));
			Assert.That(entrypoints["example/one/myCompositeRule"], Is.EqualTo(2));
			Assert.That(entrypoints["example"], Is.EqualTo(0));
		}

		[Test]
		public void DefaultEntrypointMultiTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = module.CreatePolicyInstance();

			string input = new
			{
				someProp = "thisValue",
				anotherProp = "thatValue"
			}.ToJson();
			string outputJson = opaPolicy.EvaluateJson(input);

			// [{"result":{"one":{"myOtherRule":true,"myRule":true,"myCompositeRule":true}}}]
			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result.one.myOtherRule, Is.True);
			Assert.That(output[0].result.one.myRule, Is.True);
			Assert.That(output[0].result.one.myCompositeRule, Is.True);
		}

		[Test]
		public void ExampleOneEntrypointByNumberMultiTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = module.CreatePolicyInstance();

			string input = new
			{
				someProp = "thisValue",
				anotherProp = "thatValue"
			}.ToJson();
			string outputJson = opaPolicy.EvaluateJson(input, 1);

			// [{"result":{"myOtherRule":true,"myRule":true,"myCompositeRule":true}}]
			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result.myOtherRule, Is.True);
			Assert.That(output[0].result.myRule, Is.True);
			Assert.That(output[0].result.myCompositeRule, Is.True);
		}

		[Test]
		public void ExampleOneEntrypointByNameMultiTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = module.CreatePolicyInstance();

			string input = new
			{
				someProp = "thisValue",
				anotherProp = "thatValue"
			}.ToJson();
			string outputJson = opaPolicy.EvaluateJson(input, "example/one");

			// [{"result":{"myOtherRule":true,"myRule":true,"myCompositeRule":true}}]
			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result.myOtherRule, Is.True);
			Assert.That(output[0].result.myRule, Is.True);
			Assert.That(output[0].result.myCompositeRule, Is.True);
		}

		[Test]
		public void InvalidNumericalEntrypointMultiTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = module.CreatePolicyInstance();

			string input = new
			{
				someProp = "thisValue",
				anotherProp = "thatValue"
			}.ToJson();

			var ex = Assert.Throws<ArgumentOutOfRangeException>(() => opaPolicy.EvaluateJson(input, 5));
		}

		[Test]
		public void ExampleOneRuleEntrypointMultiTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = module.CreatePolicyInstance();

			string input = new
			{
				someProp = "thisValue",
				anotherProp = "thatValue"
			}.ToJson();
			string outputJson = opaPolicy.EvaluateJson(input, "example/one/myCompositeRule");

			// [{"result":true}]
			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result, Is.True);
		}
	}
}
