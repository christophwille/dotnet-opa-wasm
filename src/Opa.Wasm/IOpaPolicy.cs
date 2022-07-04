using System;
using System.Collections.Generic;

namespace Opa.Wasm
{
	public interface IOpaPolicy : IOpaPolicyRuntime, IDisposable
	{
		int? AbiMinorVersion { get; }
		int? AbiVersion { get; }
		IReadOnlyDictionary<int, string> Builtins { get; }
		IReadOnlyDictionary<string, int> Entrypoints { get; }
		IOpaSerializer Serializer { get; set; }

		void RegisterBuiltin<TArg1, TArg2, TArg3, TArg4, TResult>(string name, Func<TArg1, TArg2, TArg3, TArg4, TResult> callback);
		void RegisterBuiltin<TArg1, TArg2, TArg3, TResult>(string name, Func<TArg1, TArg2, TArg3, TResult> callback);
		void RegisterBuiltin<TArg1, TArg2, TResult>(string name, Func<TArg1, TArg2, TResult> callback);
		void RegisterBuiltin<TArg1, TResult>(string name, Func<TArg1, TResult> callback);
		void RegisterBuiltin<TResult>(string name, Func<TResult> callback);
		void RegisterSdkBuiltins();
	}

	public interface IOpaPolicyRuntime : IOpaPolicyRuntimeWithT, IOpaPolicyRuntimeWithJson
	{
	}

	public interface IOpaPolicyRuntimeWithT
	{
		IOpaResult<T> Evaluate<T>(object input, bool disableFastEvaluate = false);
		IOpaResult<T> Evaluate<T>(object input, int entrypoint, bool disableFastEvaluate = false);
		IOpaResult<T> Evaluate<T>(object input, string entrypoint, bool disableFastEvaluate = false);
		void SetData(object data);
	}

	public interface IOpaPolicyRuntimeWithJson
	{
		string EvaluateJson(string json, bool disableFastEvaluate = false);
		string EvaluateJson(string json, int entrypoint, bool disableFastEvaluate = false);
		string EvaluateJson(string json, string entrypoint, bool disableFastEvaluate = false);
		void SetDataJson(string json);
	}
}
