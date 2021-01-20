using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;

namespace Opa.Wasm.UnitTests
{
	public static class CommonExtensionMethods
	{
		public static string ToJson(this object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}

		public static dynamic ToDynamic(this string json)
		{
			return JsonConvert.DeserializeObject<ExpandoObject[]>(json, new ExpandoObjectConverter());
		}
	}
}
