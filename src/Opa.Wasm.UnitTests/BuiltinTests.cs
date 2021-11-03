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
			int callCountOfBuiltin = 0;

			using var opaModule = new OpaModule();
			using var module = opaModule.Load(WasmFiles.BuiltinExample);
			using var opaPolicy = new OpaPolicy(opaModule, module);

			opaPolicy.RegisterBuiltin("custom.func", input => {
				callCountOfBuiltin++;
				return input + " Doe";
				});
			string outputJson = opaPolicy.Evaluate("{}");

			dynamic output = outputJson.ToDynamic();
			Assert.AreEqual("Jane Doe", output[0].result.result);
			Assert.AreEqual(1, callCountOfBuiltin);
		}
	}
}
