using NUnit.Framework;

namespace Opa.Wasm.UnitTests
{
	public class AbiVersionTests
	{
		[Test]
		public void ReadCurrentWasmFileTest()
		{
			using var opaModule = new OpaModule();
			using var module = opaModule.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = new OpaPolicy(opaModule, module);

			var abiVersion = opaPolicy.AbiVersion;
			var abiMinorVersion = opaPolicy.AbiMinorVersion;

			Assert.AreEqual(1, abiVersion);
			Assert.IsNotNull(abiMinorVersion);
		}
	}
}
