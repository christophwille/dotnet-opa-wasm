namespace Opa.Wasm
{
	public interface IOpaSerializer
	{
		string Serialize(object obj);
		T Deserialize<T>(string json);
	}
}
