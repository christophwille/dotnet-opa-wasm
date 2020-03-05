using System;
using System.Diagnostics;
using System.Linq;
using Wasmtime;
using Wasmtime.Exports;
using Wasmtime.Externs;

namespace WasmTimeTest
{
	class Program
	{
		static void Main(string[] args)
		{
			using var engine = new Engine();
			using var store = engine.CreateStore();
			using var module = store.CreateModule("policy.wasm");

			using var instance = module.Instantiate(new OpaHost());
			// my guess: would need access to _handle in Memory which is internal (Memory in WasmerSharp is derived from WasmerNativeHandle)
			/*
			Wasmtime.WasmtimeException
			  HResult=0x80131500
			  Message=Unable to bind 'OpaHost.EnvMemory' to WebAssembly import 'env.memory': Memory does not have the expected minimum of 2 page(s).
			  Source=Wasmtime.Dotnet
			  StackTrace:
			   at Wasmtime.Bindings.MemoryBinding.Bind(Store store, IHost host)
			   at Wasmtime.Instance.<>c__DisplayClass0_0.<.ctor>b__0(Binding b)
			   at System.Linq.Enumerable.SelectListIterator`2.ToArray()
			   at System.Linq.Enumerable.ToArray[TSource](IEnumerable`1 source)
			   at Wasmtime.Instance..ctor(Module module, Wasi wasi, IHost host)
			   at Wasmtime.Module.Instantiate(Wasi wasi, IHost host)
			   at Wasmtime.Module.Instantiate(IHost host)
			   at WasmTimeTest.Program.Main(String[] args) in D:\GitWorkspace\csharp-opa-wasm\WasmTimeTest\Program.cs:line 17
			 */


			var memory = instance.Externs.Memories.FirstOrDefault();
		}
	}

	class OpaHost : IHost
	{
		public OpaHost()
		{
			EnvMemory = new Memory(Memory.PageSize * 2, UInt32.MaxValue); // min page size = 2
		}

		public Instance Instance { get; set; }

		[Import("memory", Module = "env")]
		public readonly Memory EnvMemory;

		[Import("opa_abort", Module = "env")]
		public virtual void Abort(int addr)
		{
			Debugger.Break();
			// TODO: impl stringDecoder
		}

		[Import("opa_builtin0", Module = "env")]
		public virtual int Builtin0(int builtinId, int opaCtxReserved)
		{
			Debugger.Break();
			return 0;
		}

		[Import("opa_builtin1", Module = "env")]
		public virtual int Builtin1(int builtinId, int opaCtxReserved, int addr1)
		{
			Debugger.Break();
			return 0;
		}

		[Import("opa_builtin2", Module = "env")]
		public virtual int Builtin2(int builtinId, int opaCtxReserved, int addr1, int addr2)
		{
			Debugger.Break();
			return 0;
		}

		[Import("opa_builtin3", Module = "env")]
		public virtual int Builtin3(int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3)
		{
			Debugger.Break();
			return 0;
		}

		[Import("opa_builtin4", Module = "env")]
		public virtual int Builtin4(int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3, int addr4)
		{
			Debugger.Break();
			return 0;
		}
	}

	/* https://webassembly.github.io/wabt/demo/wasm2wat/

	  (type $t0 (func (param i32 i32) (result i32)))
	  (type $t2 (func (param i32 i32 i32) (result i32)))
	  (type $t3 (func (param i32)))
	  (type $t4 (func (param i32 i32 i32 i32) (result i32)))
	  (type $t5 (func (param i32 i32 i32 i32 i32) (result i32)))
	  (type $t6 (func (param i32 i32 i32 i32 i32 i32) (result i32)))
	  (import "env" "memory" (memory $env.memory 2))
	  (import "env" "opa_abort" (func $env.opa_abort (type $t3)))
	  (import "env" "opa_builtin0" (func $env.opa_builtin0 (type $t0)))
	  (import "env" "opa_builtin1" (func $env.opa_builtin1 (type $t2)))
	  (import "env" "opa_builtin2" (func $env.opa_builtin2 (type $t4)))
	  (import "env" "opa_builtin3" (func $env.opa_builtin3 (type $t5)))
	  (import "env" "opa_builtin4" (func $env.opa_builtin4 (type $t6)))
	*/
}
