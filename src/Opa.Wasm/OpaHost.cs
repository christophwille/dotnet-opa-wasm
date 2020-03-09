using System.Diagnostics;
using Wasmtime;

namespace Opa.Wasm
{
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

	class OpaHost : IHost
	{
		public OpaHost()
		{
			EnvMemory = new Memory(2);
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
}