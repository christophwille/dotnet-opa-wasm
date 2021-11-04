using System;
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

			opaPolicy.RegisterBuiltin("custom.func", input =>
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
		public void AllTest()
		{
			int[] callCountOfBuiltin = new int[5];

			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.AllBuiltinsExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			opaPolicy.RegisterBuiltin("custom.func0", () =>
			{
				callCountOfBuiltin[0]++;
				return "0";
			});

			opaPolicy.RegisterBuiltin("custom.func1", (arg1) =>
			{
				callCountOfBuiltin[1]++;
				return arg1;
			});

			opaPolicy.RegisterBuiltin("custom.func2", (arg1, arg2) =>
			{
				callCountOfBuiltin[2]++;
				return arg1 + arg2;
			});

			opaPolicy.RegisterBuiltin("custom.func3", (arg1, arg2, arg3) =>
			{
				callCountOfBuiltin[3]++;
				return arg1 + arg2 + arg3;
			});

			opaPolicy.RegisterBuiltin("custom.func4", (arg1, arg2, arg3, arg4) =>
			{
				callCountOfBuiltin[4]++;
				return arg1 + arg2 + arg3 + arg4;
			});

			string outputJson = opaPolicy.Evaluate("{}");

			dynamic output = outputJson.ToDynamic();
			Assert.AreEqual("0", output[0].result.result0);
			Assert.AreEqual("s1", output[0].result.result1);
			Assert.AreEqual("s1s2", output[0].result.result2);
			Assert.AreEqual("s1s2s3", output[0].result.result3);
			Assert.AreEqual("s1s2s3s4", output[0].result.result4);
			Assert.AreEqual(1, callCountOfBuiltin[0]);
			Assert.AreEqual(1, callCountOfBuiltin[1]);
			Assert.AreEqual(1, callCountOfBuiltin[2]);
			Assert.AreEqual(1, callCountOfBuiltin[3]);
			Assert.AreEqual(1, callCountOfBuiltin[4]);
		}
	}
}
