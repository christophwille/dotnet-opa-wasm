using NUnit.Framework;
using System.IO;

namespace Opa.Wasm.UnitTests
{
	public class BasicTests
	{
		[Test]
		public void HelloWorldTest()
		{
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

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
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.RbacExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

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
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

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

		[Test]
		public void HelloWorldTest_FromBytes()
		{
			var policyBytes = System.IO.File.ReadAllBytes(WasmFiles.HelloWorldExample);

			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load("example", policyBytes);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

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
		public void HelloWorldTest_FromFile_NoEngine()
		{
			using var opaPolicy = new OpaPolicy(WasmFiles.HelloWorldExample);

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
		public void HelloWorldTest_FromBytes_NoEngine()
		{
			var policyBytes = System.IO.File.ReadAllBytes(WasmFiles.HelloWorldExample);
			using var opaPolicy = new OpaPolicy("example", policyBytes);

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
	}
}
