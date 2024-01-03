using NUnit.Framework;
using System.IO;

namespace Opa.Wasm.UnitTests
{
	public class AbortTests
	{
		[Test]
		public void AbortTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.AbortExample);
			using var opaPolicy = module.CreatePolicyInstance();

			Wasmtime.WasmtimeException ex = Assert.Throws<Wasmtime.WasmtimeException>(
				() =>
				{
					string outputJson = opaPolicy.EvaluateJson("{}", disableFastEvaluate: true);
				});

			Assert.That(ex.InnerException.Message, Does.StartWith("abort.rego:4:1: var assignment conflict"));
		}

		[Test]
		public void FastEvaluateAbortTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.AbortExample);
			using var opaPolicy = module.CreatePolicyInstance();

			Wasmtime.WasmtimeException ex = Assert.Throws<Wasmtime.WasmtimeException>(
				() =>
				{
					string outputJson = opaPolicy.EvaluateJson("{}", disableFastEvaluate: false);
				});

			Assert.That(ex.InnerException.Message, Does.StartWith("abort.rego:4:1: var assignment conflict"));
		}
	}
}
