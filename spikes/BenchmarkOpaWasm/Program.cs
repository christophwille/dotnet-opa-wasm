using System;
using Wasmtime;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Opa.Wasm;

namespace BenchmarkOpaWasm
{
	public class WasmPolicyExecution
	{
		private readonly Module _module;

		public WasmPolicyExecution()
		{
			var engine = new Engine();
			var store = engine.CreateStore();
			_module = store.CreateModule("policy.wasm");
		}

		[Benchmark]
		public string RunPolicy()
		{
			var opaPolicy = _module.CreateOpaPolicy();

			opaPolicy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";
			string output = opaPolicy.Evaluate(input);

			return output;
		}
	}

    class Program
	{
		static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
		}
	}
}
