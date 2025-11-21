using NUnit.Framework;

namespace Opa.Wasm.UnitTests
{
	public class MemoryTests
	{
		[Test]
		public void parsing_input_exceeds_memory()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MemoryTestExample);
			using var opaPolicy = module.CreatePolicyInstance(3, 4);

			string input = new
			{
				input = new string('a', 2 * 65536)
			}.ToJson();

			Wasmtime.WasmtimeException ex = Assert.Throws<Wasmtime.WasmtimeException>(
				() =>
				{
					string outputJson = opaPolicy.EvaluateJson(input);
				});

			Assert.That(ex.InnerException.Message, Does.StartWith("opa_malloc: failed"));
		}

		[Test]
		public void large_input_host_and_guest_grow_successfully()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.MemoryTestExample);
			using var opaPolicy = module.CreatePolicyInstance(2, 8);

			string input = new
			{
				input = new string('a', 2 * 65536)
			}.ToJson();

			string outputJson = opaPolicy.EvaluateJson(input);

		}
	}
}
