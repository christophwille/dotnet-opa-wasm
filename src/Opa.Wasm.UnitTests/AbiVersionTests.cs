using NUnit.Framework;

namespace Opa.Wasm.UnitTests
{
	public class AbiVersionTests
	{
		[Test]
		public void ReadCurrentWasmFileTest()
		{
			using var opaRuntime = new OpaRuntime();
			using var module = opaRuntime.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = new OpaPolicy(opaRuntime, module);

			var abiVersion = opaPolicy.AbiVersion;
			var abiMinorVersion = opaPolicy.AbiMinorVersion;

			Assert.AreEqual(1, abiVersion);
			Assert.IsNotNull(abiMinorVersion);
		}
	}
}
