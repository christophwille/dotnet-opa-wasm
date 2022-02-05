using System.Text.Json;

namespace Opa.Wasm
{
	public class DefaultOpaSerializer : IOpaSerializer
	{
		public static readonly DefaultOpaSerializer Instance = new DefaultOpaSerializer();

		private static JsonSerializerOptions _stjDefaultOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		public string Serialize(object obj)
		{
			return JsonSerializer.Serialize(obj, _stjDefaultOptions);
		}

		public T Deserialize<T>(string json)
		{
			return JsonSerializer.Deserialize<T>(json, _stjDefaultOptions);
		}
	}
}
