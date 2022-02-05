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

			var data = new
			{
				world = "world"
			};
			opaPolicy.SetData(data);

			var input = new
			{
				message = "world"
			};

			// JSON: [{"result":true}]
			var output = opaPolicy.Evaluate<HelloWorldResult[]>(input, disableFastEvaluate: true);
			Assert.IsTrue(output[0].Result);
		}
	}
}
