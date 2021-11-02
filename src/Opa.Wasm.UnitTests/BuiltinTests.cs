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
			Assert.AreEqual(0, builtins["custom.func"]);
		}
	}
}
