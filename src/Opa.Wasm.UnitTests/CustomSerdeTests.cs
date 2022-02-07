using Newtonsoft.Json;
using NUnit.Framework;

namespace Opa.Wasm.UnitTests
{
	public class CustomSerdeTests
	{
		[Test]
		public void HelloWorldTest()
		{
			using var module = OpaPolicyModule.Load(WasmFiles.HelloWorldExample);
			using var opaPolicy = module.CreatePolicyInstance(new CustomSerdeTestsNewtonsoft());

			opaPolicy.SetData(new { world = "world" });

			var output = opaPolicy.Evaluate<bool>(
				new CustomSerdeTestsHelloWorldPolicyInput { Message = "world" },
				disableFastEvaluate: true);

			Assert.IsTrue(output.Value);
		}
	}

	internal class CustomSerdeTestsHelloWorldPolicyInput
	{
		[JsonProperty("message")]
		public string Message { get; set; }
	}

	internal class CustomSerdeTestsNewtonsoft : IOpaSerializer
	{
		public T Deserialize<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		public string Serialize(object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}
	}
}
