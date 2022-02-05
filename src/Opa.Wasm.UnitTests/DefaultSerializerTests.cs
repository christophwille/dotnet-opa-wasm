using NUnit.Framework;

namespace Opa.Wasm.UnitTests
{
	record HelloWorldResult(bool Result);

	public class DefaultSerializerTests
	{
		[Test]
		public void HelloWorldTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = module.CreatePolicyInstance();

			opaPolicy.SetData(new { world = "world" });

			var output = opaPolicy.Evaluate<HelloWorldResult>(
				new { message = "world" },
				disableFastEvaluate: true);

			Assert.IsTrue(output.Result);
		}
	}
}
