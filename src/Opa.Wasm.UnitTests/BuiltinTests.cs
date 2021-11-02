using System;
using NUnit.Framework;

namespace Opa.Wasm.UnitTests
{
	public class BuiltinTests
	{
		[Test]
		public void DumpBuiltinsTest()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load(WasmFiles.BuiltinExample);
			using var opaPolicy = new OpaPolicy(opaModule, module);

			var builtins = opaPolicy.Builtins;

			Assert.AreEqual(1, builtins.Count);
			Assert.AreEqual("custom.func", builtins[0]);
		}

		[Test]
		public void SimpleTest()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load(WasmFiles.BuiltinExample);
			using var opaPolicy = new OpaPolicy(opaModule, module);

			opaPolicy.RegisterBuiltin("custom.func", input => "Hello world");
			string outputJson = opaPolicy.Evaluate("{}");

			dynamic output = outputJson.ToDynamic();
			Assert.AreEqual("Hello world", output[0].result.result);
		}
	}
}
