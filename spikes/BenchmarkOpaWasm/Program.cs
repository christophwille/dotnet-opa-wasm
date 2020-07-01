using System;
using Wasmtime;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Opa.Wasm;

namespace BenchmarkOpaWasm
{
	public class WasmPolicyExecution
	{
		private readonly OpaPolicyStore _policyStore;
		private readonly Module _module;

		public WasmPolicyExecution()
		{
			var policyBytes = System.IO.File.ReadAllBytes("example.wasm");

			_policyStore = new OpaPolicyStore();
			_module = _policyStore.Load("example", policyBytes);
		}

		[Benchmark]
		public string RunPolicy()
		{
			using var opaPolicy = new OpaPolicy(_policyStore.Store, _module);

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
