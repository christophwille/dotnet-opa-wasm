using System.Text.Json.Serialization;

namespace Opa.Wasm
{
	public interface IOpaResult<T>
	{
		public T Value { get; }
		public string JsonOutput { get; }
	}

	public class OpaResult<T> : IOpaResult<T>
	{
		// This is intentionally a lowercase property
		public T result { get; set; }

		[JsonIgnore]
		public T Value { get { return result; } }
		[JsonIgnore]
		public string JsonOutput { get; internal set; }
	}
}
