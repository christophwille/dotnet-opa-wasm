﻿using System;
using Wasmtime;

namespace Opa.Wasm.ConsoleSample
{
	// See https://github.com/open-policy-agent/npm-opa-wasm/blob/master/examples/nodejs-app/app.js
	class Program
	{
		static void Main(string[] args)
		{
			using var engine = new Engine();
			using var store = engine.CreateStore();
			using var module = store.CreateModule("policy.wasm");

			var opaPolicy = module.CreateOpaPolicy();

			opaPolicy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";
			string output = opaPolicy.Evaluate(input);

			Console.WriteLine($"eval output: {output}");
			Console.Read();
		}
	}
}
