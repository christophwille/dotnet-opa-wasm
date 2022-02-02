using NUnit.Framework;
using System.IO;

namespace Opa.Wasm.UnitTests
{
	public class ThreadSafetyTests
	{
		[Test]
		// ref: https://docs.nunit.org/articles/nunit/writing-tests/assertions/classic-assertions/Assert.Throws.html
		public void SameEngineMustBeUsedForLoadAndExecute()
		{
			using var opaRuntimeToLoad = new OpaRuntime();
			using var module = opaRuntimeToLoad.Load(WasmFiles.HelloWorldExample);

			using var opaRuntime = new OpaRuntime();
			Assert.Throws<Wasmtime.WasmtimeException>(() =>
				{
					using var opaPolicy = new OpaPolicy(opaRuntime, module);
				});
			// Wasmtime.WasmtimeException
			// HResult = 0x80131500
			// Message = incompatible import type for `env::opa_builtin1`
			// Caused by: function types incompatible: expected func of type `(i32, i32, i32)-> (i32)`, found func of type `(i32, i32, i32)-> (i32)`
		}
	}
}
