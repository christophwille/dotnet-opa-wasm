﻿using System;
using System.Diagnostics;
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
					Debugger.Break();
				})
			);

			_linker.Define(OpaConstants.Module, OpaConstants.Builtin0, Function.FromCallback(_store,
				(Caller caller, int builtinId, int opaCtxReserved) =>
				{
					Debugger.Break();
					return 0;
				})
			);

			_linker.Define(OpaConstants.Module, OpaConstants.Builtin1, Function.FromCallback(_store,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1) =>
				{
					Debugger.Break();
					return 0;
				})
			);

			_linker.Define(OpaConstants.Module, OpaConstants.Builtin2, Function.FromCallback(_store,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2) =>
				{
					Debugger.Break();
					return 0;
				})
			);

			_linker.Define(OpaConstants.Module, OpaConstants.Builtin3, Function.FromCallback(_store,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3) =>
				{
					Debugger.Break();
					return 0;
				})
			);

			_linker.Define(OpaConstants.Module, OpaConstants.Builtin4, Function.FromCallback(_store,
				(Caller caller, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3, int addr4) =>
				{
					Debugger.Break();
					return 0;
				})
			);
		}

		private void Initialize(Module module)
		{
			_instance = _linker.Instantiate(_store, module);

			string builtins = DumpJson(Policy_Builtins());

			_dataAddr = LoadJson("{}");
			_baseHeapPtr = Policy_opa_heap_ptr_get();
			_dataHeapPtr = _baseHeapPtr;

			// endof js ctor LoadedPolicy
		}

		public string Evaluate(string json)
		{
			// Reset the heap pointer before each evaluation
			Policy_opa_heap_ptr_set(_dataHeapPtr);

			// Load the input data
			int inputAddr = LoadJson(json);

			// Setup the evaluation context
			int ctxAddr = Policy_opa_eval_ctx_new();
			Policy_opa_eval_ctx_set_input(ctxAddr, inputAddr);
			Policy_opa_eval_ctx_set_data(ctxAddr, _dataAddr);

			// Actually evaluate the policy
			Policy_eval(ctxAddr);

			// Retrieve the result
			int resultAddr = Policy_opa_eval_ctx_get_result(ctxAddr);
			return DumpJson(resultAddr);
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
			}
		}
	}
}
