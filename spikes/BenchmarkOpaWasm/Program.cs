using System;
using Wasmtime;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Opa.Wasm;

namespace BenchmarkOpaWasm
{
	public class WasmPolicyExecution
	{
		private readonly byte[] _module;

		public WasmPolicyExecution()
		{
			_module = System.IO.File.ReadAllBytes("example.wasm");
		}

		[Benchmark]
		public string RunPolicy()
		{
			using var opaPolicy = new OpaPolicy();
			opaPolicy.Load("example", _module);

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
