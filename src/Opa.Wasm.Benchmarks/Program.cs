using System;
using Wasmtime;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Opa.Wasm;

namespace Opa.Wasm.Benchmarks
{
	public class WasmPolicyExecution
	{
		private readonly OpaPolicyModule _opaModule;

		public WasmPolicyExecution()
		{
			var policyBytes = System.IO.File.ReadAllBytes("example.wasm");
			_opaModule = OpaPolicyModule.Load("example", policyBytes);
		}

		[Benchmark]
		public string RunPolicy()
		{
			using var opaPolicy = _opaModule.CreatePolicyInstance();

			opaPolicy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";
			string output = opaPolicy.Evaluate(input, disableFastEvaluate: true);

			return output;
		}

		[Benchmark]
		public string FastEvaluatePolicy()
		{
			using var opaPolicy = _opaModule.CreatePolicyInstance();

			opaPolicy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";
			string output = opaPolicy.Evaluate(input, disableFastEvaluate: false);

			return output;
		}

		const int runXTimes = 100;

		[Benchmark]
		public void RunPolicyX()
		{
			using var opaPolicy = _opaModule.CreatePolicyInstance();

			opaPolicy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";
			for (int i = 1; i <= runXTimes; i++)
			{
				string output = opaPolicy.Evaluate(input, disableFastEvaluate: true);
			}
		}

		[Benchmark]
		public void FastEvaluatePolicyX()
		{
			using var opaPolicy = _opaModule.CreatePolicyInstance();

			opaPolicy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";

			for (int i = 1; i <= runXTimes; i++)
			{
				string output = opaPolicy.Evaluate(input, disableFastEvaluate: false);
			}
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
