using NUnit.Framework;
using System.IO;

namespace Opa.Wasm.UnitTests
{
	public class AbortTests
	{
		[Test]
		public void AbortTest()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load(WasmFiles.AbortExample);
			using var opaPolicy = new OpaPolicy(opaModule, module);

			Wasmtime.TrapException ex = Assert.Throws<Wasmtime.TrapException>(
				() =>
				{
					string outputJson = opaPolicy.Evaluate("{}");
				});

			StringAssert.StartsWith("abort.rego:4:1: var assignment conflict", ex.Message);
		}

		[Test]
		public void FastEvaluateAbortTest()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load(WasmFiles.AbortExample);
			using var opaPolicy = new OpaPolicy(opaModule, module);

			Wasmtime.TrapException ex = Assert.Throws<Wasmtime.TrapException>(
				() =>
				{
					string outputJson = opaPolicy.FastEvaluate("{}");
				});

			StringAssert.StartsWith("abort.rego:4:1: var assignment conflict", ex.Message);
		}
	}
}
