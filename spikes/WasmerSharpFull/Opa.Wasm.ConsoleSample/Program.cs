using System;
using WasmerSharp;

namespace Opa.Wasm.ConsoleSample
{
	// See https://github.com/open-policy-agent/npm-opa-wasm/blob/master/examples/nodejs-app/app.js
	class Program
	{
		static void Main(string[] args)
		{
			Module m = OpaPolicyLoader.LoadFromDisk("example.wasm");

			var policy = new Opa.Wasm.OpaPolicy();
			policy.ReserveMemory();
			policy.Load(m);

			policy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";
			string output = policy.Evaluate(input);

			Console.WriteLine($"eval output: {output}");
			Console.Read();
		}
	}
}
