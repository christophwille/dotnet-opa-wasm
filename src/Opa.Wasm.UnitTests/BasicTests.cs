using NUnit.Framework;
using System.IO;

namespace Opa.Wasm.UnitTests
{
	public class BasicTests
	{
		[Test]
		public void HelloWorldTest()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = new OpaPolicy(opaModule, module);

			string data = new
			{
				world = "world"
			}.ToJson();
			opaPolicy.SetData(data);

			string input = new
			{
				message = "world"
			}.ToJson();
			string outputJson = opaPolicy.Evaluate(input, disableFastEvaluate: true);

			dynamic output = outputJson.ToDynamic();
			Assert.IsTrue(output[0].result);
		}

		[Test]
		public void RbacTest()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load(WasmFiles.RbacExample);
			using var opaPolicy = new OpaPolicy(opaModule, module);

			string data = File.ReadAllText(Path.Combine("TestData", "basic_rbac_data.json"));
			opaPolicy.SetData(data);

			string input = File.ReadAllText(Path.Combine("TestData", "basic_rbac_input.json"));
			string outputJson = opaPolicy.Evaluate(input);

			dynamic output = outputJson.ToDynamic();
			Assert.IsTrue(output[0].result.allow);
			Assert.IsTrue(output[0].result.user_is_admin);
		}

		[Test]
		public void HelloWorldTest_FastEvaluate()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = new OpaPolicy(opaModule, module);

			string data = new
			{
				world = "world"
			}.ToJson();
			opaPolicy.SetData(data);

			string input = new
			{
				message = "world"
			}.ToJson();
			string outputJson = opaPolicy.Evaluate(input, disableFastEvaluate: false);

			dynamic output = outputJson.ToDynamic();
			Assert.IsTrue(output[0].result);
		}
	}
}
