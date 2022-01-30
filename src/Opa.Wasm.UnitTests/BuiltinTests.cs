using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using NUnit.Framework;

namespace Opa.Wasm.UnitTests
{
	public class BuiltinTests
	{
		[Test]
		public void DumpBuiltinsTest()
		{
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.BuiltinExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			var builtins = opaPolicy.Builtins;

			Assert.AreEqual(1, builtins.Count);
			Assert.AreEqual("custom.func", builtins[0]);
		}

		[Test]
		public void SimpleTest()
		{
			int callCountOfBuiltin = 0;

			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.BuiltinExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			opaPolicy.RegisterBuiltin("custom.func", (string input) =>
			{
				callCountOfBuiltin++;
				return input + " Doe";
			});
			string outputJson = opaPolicy.Evaluate("{}");

			dynamic output = outputJson.ToDynamic();
			Assert.AreEqual("Jane Doe", output[0].result.result);
			Assert.AreEqual(1, callCountOfBuiltin);
		}

		[Test]
		[SetCulture("de-DE")]
		public void NumberSerializationTest()
		{
			int callCountOfBuiltin = 0;

			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.MathBuiltinExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			opaPolicy.RegisterBuiltin("custom.func2", (float firstNumber, float secondNumber) =>
			{
				callCountOfBuiltin++;
				return firstNumber + secondNumber;
			});

			var firstNumber = decimal.Parse("3,1"); // proving this test is running in german culture.
			var input = new { firstNumber, secondNumber = 2.2 };
			string outputJson = opaPolicy.Evaluate(input.ToJson());

			dynamic output = outputJson.ToDynamic();
			Assert.AreEqual(5.3, output[0].result);
			Assert.AreEqual(1, callCountOfBuiltin);
		}

		[Test]
		public void AllTest()
		{
			int[] callCountOfBuiltin = new int[5];

			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.AllBuiltinsExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			opaPolicy.RegisterBuiltin("custom.func0", () =>
			{
				callCountOfBuiltin[0]++;
				return 0;
			});

			opaPolicy.RegisterBuiltin("custom.func1", (string arg1) =>
			{
				callCountOfBuiltin[1]++;
				return arg1;
			});

			opaPolicy.RegisterBuiltin("custom.func2", (int arg1, int arg2) =>
			{
				callCountOfBuiltin[2]++;
				return arg1 < arg2;
			});

			opaPolicy.RegisterBuiltin("custom.func3", (string arg1, string arg2, bool arg3) =>
			{
				callCountOfBuiltin[3]++;
				return arg1 + arg2 + arg3;
			});

			opaPolicy.RegisterBuiltin("custom.func4", (IEnumerable<string> arg1, string arg2, string arg3, string arg4) =>
			{
				callCountOfBuiltin[4]++;
				return string.Join(",", arg1) + arg2 + arg3 + arg4;
			});

			string outputJson = opaPolicy.Evaluate("{}");

			dynamic output = outputJson.ToDynamic();
			Assert.AreEqual(0, output[0].result.result0);
			Assert.AreEqual("s1", output[0].result.result1);
			Assert.AreEqual(true, output[0].result.result2);
			Assert.AreEqual("s1s2False", output[0].result.result3);
			Assert.AreEqual("s1-1,s1-2s2s3s4", output[0].result.result4);
			Assert.AreEqual(1, callCountOfBuiltin[0]);
			Assert.AreEqual(1, callCountOfBuiltin[1]);
			Assert.AreEqual(1, callCountOfBuiltin[2]);
			Assert.AreEqual(1, callCountOfBuiltin[3]);
			Assert.AreEqual(1, callCountOfBuiltin[4]);
		}
	}
}
