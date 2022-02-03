using NUnit.Framework;

namespace Opa.Wasm.UnitTests
{
	public class AbiVersionTests
	{
		[Test]
		public void ReadCurrentWasmFileTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = module.CreatePolicyInstance();

			var abiVersion = opaPolicy.AbiVersion;
			var abiMinorVersion = opaPolicy.AbiMinorVersion;

			Assert.AreEqual(1, abiVersion);
			Assert.IsNotNull(abiMinorVersion);
		}
	}
}
