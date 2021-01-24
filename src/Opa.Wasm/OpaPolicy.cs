using System;
using System.Diagnostics;
using Wasmtime;

namespace Opa.Wasm
{
	public class OpaPolicy : IDisposable
	{
		private int _dataAddr;
		private int _baseHeapPtr;
		private int _dataHeapPtr;

		private Host _host;
		private Memory _envMemory;
		private Instance _instance;
		private dynamic _policy;

		/// <summary>
		/// This ctor is intended for scenarios where you want to cache a precompiled WASM module
		/// </summary>
		/// <param name="module"></param>
		/// <param name="wasmModule"></param>
		public OpaPolicy(OpaModule module, Module wasmModule)
		{
			SetupHost(module, wasmModule);
		}

		/// <summary>
		/// Load OPA policy from a .wasm file on disk. Incurs compilation penalty.
		/// </summary>
		/// <param name="fileName"></param>
		public OpaPolicy(string fileName)
		{
			using var module = new OpaModule();
			var wasmModule = module.Load(fileName);
			SetupHost(module, wasmModule);
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
			SetupHost(module, wasmModule);
		}

		private void SetupHost(OpaModule module, Module wasmModule)
		{
			_host = module.CreateHost();
			BuildHost();
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
		private void BuildHost()
		{
			_envMemory = _host.DefineMemory(OpaConstants.Module, OpaConstants.MemoryName, 2);

			var funcAbort = _host.DefineFunction(OpaConstants.Module, OpaConstants.Abort,
				(Caller caller, int addr) =>
				{
					Debugger.Break();
				}
			);
			funcAbort.Dispose();

			var funcBuiltin0 = _host.DefineFunction(OpaConstants.Module, OpaConstants.Builtin0,
				(Caller caller, int builtinId, int opaCtxReserved) =>
				{
					Debugger.Break();
					return 0;
				}
			);
			funcBuiltin0.Dispose();

			var funcBuiltin1 = _host.DefineFunction(OpaConstants.Module, OpaConstants.Builtin1,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1) =>
				{
					Debugger.Break();
					return 0;
				}
			);
			funcBuiltin1.Dispose();

			var funcBuiltin2 = _host.DefineFunction(OpaConstants.Module, OpaConstants.Builtin2,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2) =>
				{
					Debugger.Break();
					return 0;
				}
			);
			funcBuiltin2.Dispose();

			var funcBuiltin3 = _host.DefineFunction(OpaConstants.Module, OpaConstants.Builtin3,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3) =>
				{
					Debugger.Break();
					return 0;
				}
			);
			funcBuiltin3.Dispose();

			var funcBuiltin4 = _host.DefineFunction(OpaConstants.Module, OpaConstants.Builtin4,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3, int addr4) =>
				{
					Debugger.Break();
					return 0;
				}
			);
			funcBuiltin4.Dispose();
		}

		private void Initialize(Module module)
		{
			_instance = _host.Instantiate(module);
			_policy = (dynamic)_instance;

			string builtins = DumpJson(_policy.builtins());

			_dataAddr = LoadJson("{}");
			_baseHeapPtr = _policy.opa_heap_ptr_get();
			_dataHeapPtr = _baseHeapPtr;

			// endof js ctor LoadedPolicy
		}

		public string Evaluate(string json)
		{
			// Reset the heap pointer before each evaluation
			_policy.opa_heap_ptr_set(_dataHeapPtr);

			// Load the input data
			int inputAddr = LoadJson(json);

			// Setup the evaluation context
			int ctxAddr = _policy.opa_eval_ctx_new();
			_policy.opa_eval_ctx_set_input(ctxAddr, inputAddr);
			_policy.opa_eval_ctx_set_data(ctxAddr, _dataAddr);

			// Actually evaluate the policy
			_policy.eval(ctxAddr);

			// Retrieve the result
			int resultAddr = _policy.opa_eval_ctx_get_result(ctxAddr);
			return DumpJson(resultAddr);
		}

		public void SetData(string json)
		{
			_policy.opa_heap_ptr_set(_baseHeapPtr);
			_dataAddr = LoadJson(json);
			_dataHeapPtr = _policy.opa_heap_ptr_get();
		}

		private int LoadJson(string json)
		{
			int addr = _policy.opa_malloc(json.Length);
			_envMemory.WriteString(addr, json);

			int parseAddr = _policy.opa_json_parse(addr, json.Length);

			if (0 == parseAddr)
			{
				throw new ArgumentNullException("Parsing failed");
			}

			return parseAddr;
		}

		private string DumpJson(int addrResult)
		{
			int addr = _policy.opa_json_dump(addrResult);
			return DecodeNullTerminatedString(_envMemory, addr);
		}

		private static string DecodeNullTerminatedString(Memory memory, int addr)
		{
			var buf = memory.Span;
			int idx = addr;

			while (buf[idx] != 0)
			{
				idx++;
			}

			return memory.ReadString(addr, idx - addr);
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
				_envMemory.Dispose();
				_envMemory = null;
				_policy = null;
				_instance.Dispose();
				_instance = null;
				_host.Dispose();
				_host = null;
			}
		}
	}
}
