using System.Collections.Generic;
using NUnit.Framework;

namespace Opa.Wasm.UnitTests
{
	public class BuiltinTests
	{
		[Test]
		public void DumpBuiltinsTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.BuiltinExample);
			using var opaPolicy = module.CreatePolicyInstance();

			var builtins = opaPolicy.Builtins;

			Assert.That(builtins.Count, Is.EqualTo(1));
			Assert.That(builtins[0], Is.EqualTo("custom.func"));
		}

		[Test]
		public void SimpleTest()
		{
			int callCountOfBuiltin = 0;

			using var module = OpaPolicyModule.Load(WasmFiles.BuiltinExample);
			using var opaPolicy = module.CreatePolicyInstance();

			opaPolicy.RegisterBuiltin("custom.func", (string input) =>
			{
				callCountOfBuiltin++;
				return input + " Doe";
			});
			string outputJson = opaPolicy.EvaluateJson("{}");

			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result.result, Is.EqualTo("Jane Doe"));
			Assert.That(callCountOfBuiltin, Is.EqualTo(1));
		}

		[Test]
		[SetCulture("de-DE")]
		public void NumberSerializationTest()
		{
			int callCountOfBuiltin = 0;

			using var module = OpaPolicyModule.Load(WasmFiles.MathBuiltinExample);
			using var opaPolicy = module.CreatePolicyInstance();

			opaPolicy.RegisterBuiltin("custom.func2", (float firstNumber, float secondNumber) =>
			{
				callCountOfBuiltin++;
				return firstNumber + secondNumber;
			});

			var firstNumber = decimal.Parse("3,1"); // proving this test is running in german culture.
			var input = new { firstNumber, secondNumber = 2.2 };
			string outputJson = opaPolicy.EvaluateJson(input.ToJson());

			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result, Is.EqualTo(5.3));
			Assert.That(callCountOfBuiltin, Is.EqualTo(1));
		}

		[Test]
		public void AllTest()
		{
			int[] callCountOfBuiltin = new int[5];

			using var module = OpaPolicyModule.Load(WasmFiles.AllBuiltinsExample);
			using var opaPolicy = module.CreatePolicyInstance();

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

			string outputJson = opaPolicy.EvaluateJson("{}");

			dynamic output = outputJson.ToDynamic();
			Assert.That(output[0].result.result0, Is.EqualTo(0));
			Assert.That(output[0].result.result1, Is.EqualTo("s1"));
			Assert.That(output[0].result.result2, Is.True);
			Assert.That(output[0].result.result3, Is.EqualTo("s1s2False"));
			Assert.That(output[0].result.result4, Is.EqualTo("s1-1,s1-2s2s3s4"));
			Assert.That(callCountOfBuiltin[0], Is.EqualTo(1));
			Assert.That(callCountOfBuiltin[1], Is.EqualTo(1));
			Assert.That(callCountOfBuiltin[2], Is.EqualTo(1));
			Assert.That(callCountOfBuiltin[3], Is.EqualTo(1));
			Assert.That(callCountOfBuiltin[4], Is.EqualTo(1));
		}
	}
}
