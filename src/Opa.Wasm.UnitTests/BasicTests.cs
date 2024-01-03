using NUnit.Framework;
using System.IO;

namespace Opa.Wasm.UnitTests
{
	public class BasicTests
	{
		[Test]
		public void HelloWorldTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = module.CreatePolicyInstance();

			string data = new
			{
				world = "world"
			}.ToJson();
			opaPolicy.SetDataJson(data);

			string input = new
			{
				message = "world"
			}.ToJson();
			string outputJson = opaPolicy.EvaluateJson(input, disableFastEvaluate: true);

			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result, Is.True);
		}

		[Test]
		public void RbacTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.RbacExample);
			using var opaPolicy = module.CreatePolicyInstance();

			string data = File.ReadAllText(Path.Combine("TestData", "basic_rbac_data.json"));
			opaPolicy.SetDataJson(data);

			string input = File.ReadAllText(Path.Combine("TestData", "basic_rbac_input.json"));
			string outputJson = opaPolicy.EvaluateJson(input);

			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result.allow, Is.True);
			Assert.That(output[0].result.user_is_admin, Is.True);
		}

		[Test]
		public void HelloWorldTest_FastEvaluate()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = module.CreatePolicyInstance();

			string data = new
			{
				world = "world"
			}.ToJson();
			opaPolicy.SetDataJson(data);

			string input = new
			{
				message = "world"
			}.ToJson();
			string outputJson = opaPolicy.EvaluateJson(input, disableFastEvaluate: false);

			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result, Is.True);
		}

		[Test]
		public void HelloWorldTest_FromBytes()
		{
			var policyBytes = System.IO.File.ReadAllBytes(WasmFiles.HelloWorldExample);

			using var module = OpaPolicyModule.Load("example", policyBytes);
			using var opaPolicy = module.CreatePolicyInstance();

			string data = new
			{
				world = "world"
			}.ToJson();
			opaPolicy.SetDataJson(data);

			string input = new
			{
				message = "world"
			}.ToJson();
			string outputJson = opaPolicy.EvaluateJson(input, disableFastEvaluate: true);

			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result, Is.True);
		}

		[Test]
		public void HelloWorldTest_FromFile_ExplicitEngine()
		{
			using var engine = OpaPolicyModule.CreateEngine();
			using var opaPolicyModule = OpaPolicyModule.Load(WasmFiles.HelloWorldExample, engine);
			using var opaPolicy = opaPolicyModule.CreatePolicyInstance();

			string data = new
			{
				world = "world"
			}.ToJson();
			opaPolicy.SetDataJson(data);

			string input = new
			{
				message = "world"
			}.ToJson();
			string outputJson = opaPolicy.EvaluateJson(input, disableFastEvaluate: true);

			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result, Is.True);
		}

		[Test]
		public void HelloWorldTest_FromBytes_ExplicitEngine()
		{
			var policyBytes = System.IO.File.ReadAllBytes(WasmFiles.HelloWorldExample);

			using var engine = OpaPolicyModule.CreateEngine();
			using var opaPolicyModule = OpaPolicyModule.Load("example", policyBytes, engine);
			using var opaPolicy = opaPolicyModule.CreatePolicyInstance();

			string data = new
			{
				world = "world"
			}.ToJson();
			opaPolicy.SetDataJson(data);

			string input = new
			{
				message = "world"
			}.ToJson();
			string outputJson = opaPolicy.EvaluateJson(input, disableFastEvaluate: true);

			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result, Is.True);
		}
	}
}
