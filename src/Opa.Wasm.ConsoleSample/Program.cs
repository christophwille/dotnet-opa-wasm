using System;

namespace Opa.Wasm.ConsoleSample
{
	// See https://github.com/open-policy-agent/npm-opa-wasm/blob/master/examples/nodejs-app/app.js
	class Program
	{
		static void Main(string[] args)
		{
			var policy = new Opa.Wasm.OpaPolicy();
			policy.ReserveMemory();
			policy.LoadFromDisk("policy.wasm");

			policy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";
			string output = policy.Evaluate(input);

			Console.WriteLine($"eval output: {output}");
			Console.Read();
		}
	}
}
