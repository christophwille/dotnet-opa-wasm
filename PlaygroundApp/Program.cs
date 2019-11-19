using System;

namespace PlaygroundApp
{
	// See https://github.com/open-policy-agent/npm-opa-wasm/blob/master/examples/nodejs-app/app.js
	class Program
	{
		static void Main(string[] args)
		{
			var policy = new OpaPolicy();
			policy.ReserveMemory();
			policy.LoadFromDisk();

			policy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";
			string output = policy.Evaluate(input);

			Console.WriteLine($"eval output: {output}");
			Console.Read();
		}
	}
}
