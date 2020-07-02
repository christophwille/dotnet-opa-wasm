using System;

namespace Opa.Wasm.ConsoleSample
{
	// See https://github.com/open-policy-agent/npm-opa-wasm/blob/master/examples/nodejs-app/app.js
	class Program
	{
		static void Main(string[] args)
		{
			EvaluateHelloWorld();
			EvaluateRbac();

			Console.Read();
		}

		// https://play.openpolicyagent.org/ "Role-based" example stripped down to minimum
		static void EvaluateRbac()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load("rbac.wasm");
			using var opaPolicy = new OpaPolicy(opaModule, module);

			opaPolicy.SetData(@"{""user_roles"": { ""alice"": [""admin""],""bob"": [""employee"",""billing""],""eve"": [""customer""]}}");

			string input = @"{ ""user"": ""alice"", ""action"": ""read"", ""object"": ""id123"", ""type"": ""dog"" }";
			string output = opaPolicy.Evaluate(input);

			Console.WriteLine($"RBAC output: {output}");
		}

		static void EvaluateHelloWorld()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load("example.wasm");
			using var opaPolicy = new OpaPolicy(opaModule, module);

			opaPolicy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";
			string output = opaPolicy.Evaluate(input);

			Console.WriteLine($"Hello world output: {output}");
		}
	}
}
