using NUnit.Framework;
using System.IO;

namespace Opa.Wasm.UnitTests
{
	public class AbortTests
	{
		[Test]
		public void AbortTest()
		{
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.AbortExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			Wasmtime.TrapException ex = Assert.Throws<Wasmtime.TrapException>(
				() =>
				{
					string outputJson = opaPolicy.Evaluate("{}", disableFastEvaluate: true);
				});

			StringAssert.StartsWith("abort.rego:4:1: var assignment conflict", ex.Message);
		}

		[Test]
		public void FastEvaluateAbortTest()
		{
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.AbortExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			Wasmtime.TrapException ex = Assert.Throws<Wasmtime.TrapException>(
				() =>
				{
					string outputJson = opaPolicy.Evaluate("{}", disableFastEvaluate: false);
				});

			StringAssert.StartsWith("abort.rego:4:1: var assignment conflict", ex.Message);
		}
	}
}
