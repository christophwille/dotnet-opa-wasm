using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Wasmtime;

namespace Opa.Wasm
{
	public partial class OpaPolicy : IDisposable
	{
		private int _dataAddr;
		private int _baseHeapPtr;
		private int _dataHeapPtr;

		private Linker _linker;
		private Store _store;
		private Memory _envMemory;
		private Instance _instance;

		private Dictionary<string, Object> _registeredBuiltins = new Dictionary<string, object>();

		public IReadOnlyDictionary<string, int> Entrypoints { get; private set; }
		public IReadOnlyDictionary<int, string> Builtins { get; private set; }

		public int? AbiVersion { get; private set; }
		public int? AbiMinorVersion { get; private set; }

		/// <summary>
		/// This ctor is intended for scenarios where you want to cache a precompiled WASM module
		/// </summary>
		/// <param name="module"></param>
		/// <param name="wasmModule"></param>
		public OpaPolicy(OpaModule module, Module wasmModule)
		{
			Setup(module, wasmModule);
		}

		/// <summary>
		/// Load OPA policy from a .wasm file on disk. Incurs compilation penalty.
		/// </summary>
		/// <param name="fileName"></param>
		public OpaPolicy(string fileName)
		{
			using var module = new OpaModule();
			var wasmModule = module.Load(fileName);

			Setup(module, wasmModule);
		}

		/// <summary>
		/// Load OPA policy from an in-memory byte[] (eg Cache or other non-disk location). Incurs compilation penalty.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="content"></param>
		public OpaPolicy(string name, byte[] content)
		{
			using var module = new OpaModule();
			var wasmModule = module.Load(name, content);

			Setup(module, wasmModule);
		}

		private void Setup(OpaModule module, Module wasmModule)
		{
			_linker = module.CreateLinker();
			_store = module.CreateStore();
			LinkImports();

			Initialize(wasmModule);
		}

		/*  https://webassembly.github.io/wabt/demo/wasm2wat/
			(type $t0 (func (param i32 i32 i32) (result i32)))
			(type $t1 (func (param i32 i32) (result i32)))
			(type $t4 (func (param i32)))
			(type $t9 (func (param i32 i32 i32 i32) (result i32)))
			(type $t10 (func (param i32 i32 i32 i32 i32 i32) (result i32)))
			(type $t11 (func (param i32 i32 i32 i32 i32) (result i32)))
			(import "env" "memory" (memory $env.memory 2))
			(import "env" "opa_abort" (func $opa_abort (type $t4)))
			(import "env" "opa_builtin0" (func $opa_builtin0 (type $t1)))
			(import "env" "opa_builtin1" (func $opa_builtin1 (type $t0)))
			(import "env" "opa_builtin2" (func $opa_builtin2 (type $t9)))
			(import "env" "opa_builtin3" (func $opa_builtin3 (type $t11)))
			(import "env" "opa_builtin4" (func $opa_builtin4 (type $t10)))
		*/
		private void LinkImports()
		{
			_envMemory = new Memory(_store, 2);
			_linker.Define(OpaConstants.Module, OpaConstants.MemoryName, _envMemory);

			_linker.Define(OpaConstants.Module, OpaConstants.Abort, Function.FromCallback(_store,
				(Caller caller, int addr) =>
				{
					string info = _envMemory.ReadNullTerminatedString(_store, addr);

					// NOTE: the generic class will do, as it is unwrapped again by Policy_eval/run?.Invoke
					throw new Exception(info);
				})
			);

			_linker.Define(OpaConstants.Module, OpaConstants.Builtin0, Function.FromCallback(_store,
				(Caller caller, int builtinId, int opaCtxReserved) =>
				{
					return CallBuiltin(builtinId, opaCtxReserved);
				})
			);

			_linker.Define(OpaConstants.Module, OpaConstants.Builtin1, Function.FromCallback(_store,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1) =>
				{
					return CallBuiltin(builtinId, opaCtxReserved, addr1);
				})
			);

			_linker.Define(OpaConstants.Module, OpaConstants.Builtin2, Function.FromCallback(_store,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2) =>
				{
					return CallBuiltin(builtinId, opaCtxReserved, addr1, addr2);
				})
			);

			_linker.Define(OpaConstants.Module, OpaConstants.Builtin3, Function.FromCallback(_store,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3) =>
				{
					return CallBuiltin(builtinId, opaCtxReserved, addr1, addr2, addr3);
				})
			);

			_linker.Define(OpaConstants.Module, OpaConstants.Builtin4, Function.FromCallback(_store,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3, int addr4) =>
				{
					return CallBuiltin(builtinId, opaCtxReserved, addr1, addr2, addr3, addr4);
				})
			);
		}

		private void Initialize(Module module)
		{
			_instance = _linker.Instantiate(_store, module);

			string builtins = DumpJson(Policy_Builtins());
			Builtins = ParseBuiltinsJson(builtins);

			_dataAddr = LoadJson("{}");
			_baseHeapPtr = Policy_opa_heap_ptr_get();
			_dataHeapPtr = _baseHeapPtr;

			string entrypoints = DumpJson(Policy_Entrypoints());
			Entrypoints = ParseEntryPointsJson(entrypoints);

			ReadAbiVersionGlobals();
		}

		/* Format of JSON
		{
		   "example/one":1,
		   "example/one/myCompositeRule":2,
		   "example":0
		} */
		private Dictionary<string, int> ParseEntryPointsJson(string json)
		{
			using JsonDocument document = JsonDocument.Parse(json, GetSTJDefaultOptions());

			var dict = new Dictionary<string, int>();
			foreach (JsonProperty prop in document.RootElement.EnumerateObject())
			{
				dict.Add(prop.Name, prop.Value.GetInt32());
			}

			return dict;
		}

		private Dictionary<int, string> ParseBuiltinsJson(string json)
		{
			if ("{}" == json) return new Dictionary<int, string>();

			using JsonDocument document = JsonDocument.Parse(json, GetSTJDefaultOptions());

			var dict = new Dictionary<int, string>();
			foreach (JsonProperty prop in document.RootElement.EnumerateObject())
			{
				dict.Add(prop.Value.GetInt32(), prop.Name);
			}

			return dict;
		}

		private JsonDocumentOptions GetSTJDefaultOptions()
		{
			return new JsonDocumentOptions
			{
				AllowTrailingCommas = true
			};
		}

		private void ReadAbiVersionGlobals()
		{
			var major = Policy_opa_wasm_abi_version();
			if (major.HasValue)
			{
				if (major != 1)
				{
					throw new BadImageFormatException($"{major} ABI version is unsupported");
				}

				AbiVersion = major;
			}
			else
			{
				// opa_wasm_abi_version undefined
			}

			var minor = Policy_opa_wasm_abi_minor_version();
			if (minor.HasValue)
			{
				AbiMinorVersion = minor;
			}
			else
			{
				// opa_wasm_abi_minor_version undefined
			}
		}

		public string Evaluate(string json, bool disableFastEvaluate = false)
		{
			return ExecuteEvaluate(json, null, disableFastEvaluate);
		}

		public string Evaluate(string json, int entrypoint, bool disableFastEvaluate = false)
		{
			bool found = false;
			foreach (int epId in Entrypoints.Values)
			{
				if (epId == entrypoint)
				{
					found = true;
					break;
				}
			}
			if (!found)
			{
				throw new ArgumentOutOfRangeException(nameof(entrypoint), $"{entrypoint} not found in Entrypoints table");
			}
			return ExecuteEvaluate(json, entrypoint, disableFastEvaluate);
		}

		public string Evaluate(string json, string entrypoint, bool disableFastEvaluate=false)
		{
			bool found = Entrypoints.TryGetValue(entrypoint, out var epId);
			if (!found)
			{
				throw new ArgumentOutOfRangeException(nameof(entrypoint), $"{entrypoint} not found in Entrypoints table");
			}
			return ExecuteEvaluate(json, epId, disableFastEvaluate);
		}

		private string ExecuteEvaluate(string json, int? entrypoint, bool disableFastEvaluate)
		{
			if (!disableFastEvaluate && AbiMinorVersion.Value >= 2)
			{
				return FastEvaluate(json, entrypoint);
			}

			// Reset the heap pointer before each evaluation
			Policy_opa_heap_ptr_set(_dataHeapPtr);

			// Load the input data
			int inputAddr = LoadJson(json);

			// Setup the evaluation context
			int ctxAddr = Policy_opa_eval_ctx_new();
			Policy_opa_eval_ctx_set_input(ctxAddr, inputAddr);
			Policy_opa_eval_ctx_set_data(ctxAddr, _dataAddr);

			if (entrypoint.HasValue)
			{
				Policy_opa_eval_ctx_set_entrypoint(ctxAddr, entrypoint.Value);
			}

			// Actually evaluate the policy
			Policy_eval(ctxAddr);

			// Retrieve the result
			int resultAddr = Policy_opa_eval_ctx_get_result(ctxAddr);
			return DumpJson(resultAddr);
		}

		// https://github.com/open-policy-agent/opa/issues/3696#issuecomment-891662230
		private string FastEvaluate(string json, int? entrypoint)
		{
			if (!entrypoint.HasValue) entrypoint = 0; // use default entry point

			_envMemory.WriteString(_store, _dataHeapPtr, json);

			int resultaddr = Policy_opa_eval(entrypoint.Value, _dataAddr, _dataHeapPtr, json.Length, _dataHeapPtr + json.Length);

			return _envMemory.ReadNullTerminatedString(_store, resultaddr);
		}

		public void SetData(string json)
		{
			Policy_opa_heap_ptr_set(_baseHeapPtr);
			_dataAddr = LoadJson(json);
			_dataHeapPtr = Policy_opa_heap_ptr_get();
		}

		private int LoadJson(string json)
		{
			int addr = Policy_opa_malloc(json.Length);
			_envMemory.WriteString(_store, addr, json);

			int parseAddr = Policy_opa_json_parse(addr, json.Length);

			if (0 == parseAddr)
			{
				throw new ArgumentNullException("Parsing failed");
			}

			return parseAddr;
		}

		private string DumpJson(int addrResult)
		{
			int addr = Policy_opa_json_dump(addrResult);
			return _envMemory.ReadNullTerminatedString(_store, addr);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_envMemory = null;
				_instance = null;
				_store.Dispose();
				_store = null;
				_linker.Dispose();
				_linker = null;
				_registeredBuiltins.Clear();
			}
		}

		public void RegisterSdkBuiltins()
		{
			// See https://github.com/christophwille/dotnet-opa-wasm/issues/3#issuecomment-957579119
			// Wire-up here, implementations to go into OpaPolicy.SdkBuiltins.cs
			throw new NotImplementedException();
		}

		public void RegisterBuiltin(string name, Func<string> func)
		{
			_registeredBuiltins.Add(name, func);
		}

		public void RegisterBuiltin(string name, Func<string, string> func)
		{
			_registeredBuiltins.Add(name, func);
		}

		public void RegisterBuiltin(string name, Func<string, string, string> func)
		{
			_registeredBuiltins.Add(name, func);
		}

		public void RegisterBuiltin(string name, Func<string, string, string, string> func)
		{
			_registeredBuiltins.Add(name, func);
		}

		public void RegisterBuiltin(string name, Func<string, string, string, string, string> func)
		{
			_registeredBuiltins.Add(name, func);
		}

		private int CallBuiltin(int builtinId, int opaCtxReserved)
		{
			string result = ((Func<string>)GetFuncForBuiltinId(builtinId))();
			return BuiltinResultToAddress(result);
		}

		private int CallBuiltin(int builtinId, int opaCtxReserved, int addr1)
		{
			string result = ((Func<string, string>)GetFuncForBuiltinId(builtinId))(BuiltinArgToString(addr1));
			return BuiltinResultToAddress(result);
		}

		private int CallBuiltin(int builtinId, int opaCtxReserved, int addr1, int addr2)
		{
			string result = ((Func<string, string, string>)GetFuncForBuiltinId(builtinId))(
				BuiltinArgToString(addr1), BuiltinArgToString(addr2));
			return BuiltinResultToAddress(result);
		}

		private int CallBuiltin(int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3)
		{
			string result = ((Func<string, string, string, string>)GetFuncForBuiltinId(builtinId))(
				BuiltinArgToString(addr1), BuiltinArgToString(addr2), BuiltinArgToString(addr3));
			return BuiltinResultToAddress(result);
		}

		private int CallBuiltin(int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3, int addr4)
		{
			string result = ((Func<string, string, string, string, string>)GetFuncForBuiltinId(builtinId))(
				BuiltinArgToString(addr1), BuiltinArgToString(addr2), BuiltinArgToString(addr3), BuiltinArgToString(addr4));
			return BuiltinResultToAddress(result);
		}

		private object GetFuncForBuiltinId(int builtinId)
		{
			if (Builtins.TryGetValue(builtinId, out string nameOfFunc))
			{
				if (_registeredBuiltins.TryGetValue(nameOfFunc, out var func))
				{
					return func;
				}
				else
				{
					throw new ArgumentOutOfRangeException(nameof(nameOfFunc), $"{nameOfFunc} not found in provided builtins. Did you register the builtin?");
				}
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(builtinId), $"{builtinId} not found in Builtins table");
			}
		}

		private string BuiltinArgToString(int addr)
		{
			var json = DumpJson(addr);
			return JsonSerializer.Deserialize<string>(json);
		}

		private int BuiltinResultToAddress(string result)
		{
			var json = JsonSerializer.Serialize(result);
			return LoadJson(json);
		}
	}
}
