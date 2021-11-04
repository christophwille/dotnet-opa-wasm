using NUnit.Framework;
using System;
using System.IO;

namespace Opa.Wasm.UnitTests
{
	public class MultiEntryPointTests
	{
		[Test]
		public void DumpEntryPointsForMultiTest()
		{
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			var entrypoints = opaPolicy.Entrypoints;

			Assert.AreEqual(3, entrypoints.Count);
			Assert.AreEqual(1, entrypoints["example/one"]);
			Assert.AreEqual(2, entrypoints["example/one/myCompositeRule"]);
			Assert.AreEqual(0, entrypoints["example"]);
		}

		[Test]
		public void DefaultEntrypointMultiTest()
		{
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			string input = new
			{
				someProp = "thisValue",
				anotherProp = "thatValue"
			}.ToJson();
			string outputJson = opaPolicy.Evaluate(input);

			// [{"result":{"one":{"myOtherRule":true,"myRule":true,"myCompositeRule":true}}}]
			dynamic output = outputJson.ToDynamic();
			Assert.IsTrue(output[0].result.one.myOtherRule);
			Assert.IsTrue(output[0].result.one.myRule);
			Assert.IsTrue(output[0].result.one.myCompositeRule);
		}

		[Test]
		public void ExampleOneEntrypointByNumberMultiTest()
		{
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			string input = new
			{
				someProp = "thisValue",
				anotherProp = "thatValue"
			}.ToJson();
			string outputJson = opaPolicy.Evaluate(input, 1);

			// [{"result":{"myOtherRule":true,"myRule":true,"myCompositeRule":true}}]
			dynamic output = outputJson.ToDynamic();
			Assert.IsTrue(output[0].result.myOtherRule);
			Assert.IsTrue(output[0].result.myRule);
			Assert.IsTrue(output[0].result.myCompositeRule);
		}

		[Test]
		public void ExampleOneEntrypointByNameMultiTest()
		{
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			string input = new
			{
				someProp = "thisValue",
				anotherProp = "thatValue"
			}.ToJson();
			string outputJson = opaPolicy.Evaluate(input, "example/one");

			// [{"result":{"myOtherRule":true,"myRule":true,"myCompositeRule":true}}]
			dynamic output = outputJson.ToDynamic();
			Assert.IsTrue(output[0].result.myOtherRule);
			Assert.IsTrue(output[0].result.myRule);
			Assert.IsTrue(output[0].result.myCompositeRule);
		}

		[Test]
		public void InvalidNumericalEntrypointMultiTest()
		{
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			string input = new
			{
				someProp = "thisValue",
				anotherProp = "thatValue"
			}.ToJson();

			var ex = Assert.Throws<ArgumentOutOfRangeException>(() => opaPolicy.Evaluate(input, 5));
		}

		[Test]
		public void ExampleOneRuleEntrypointMultiTest()
		{
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.MultiEntrypointExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			string input = new
			{
				someProp = "thisValue",
				anotherProp = "thatValue"
			}.ToJson();
			string outputJson = opaPolicy.Evaluate(input, "example/one/myCompositeRule");

			// [{"result":true}]
			dynamic output = outputJson.ToDynamic();
			Assert.IsTrue(output[0].result);
		}
	}
}
