using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WasmerSharp;

namespace PlaygroundApp
{
	public class OpaPolicy
	{
		private Memory _memory;

		public OpaPolicy()
		{
		}

		public void ReserveMemory()
		{
			_memory = Memory.Create(minPages: 5);
		}

		public void LoadFromDisk()
		{
			Import memoryImport = new Import("env", "memory", _memory);

			var funcOpaAbort = new Import("env", "opa_abort", new ImportFunction((opaabortCallback)(opa_abort)));
			var funcOpaBuiltin0 = new Import("env", "opa_builtin0", new ImportFunction((builtin0Callback)(opa_builtin0)));
			var funcOpaBuiltin1 = new Import("env", "opa_builtin1", new ImportFunction((builtin1Callback)(opa_builtin1)));
			var funcOpaBuiltin2 = new Import("env", "opa_builtin2", new ImportFunction((builtin2Callback)(opa_builtin2)));
			var funcOpaBuiltin3 = new Import("env", "opa_builtin3", new ImportFunction((builtin3Callback)(opa_builtin3)));
			var funcOpaBuiltin4 = new Import("env", "opa_builtin4", new ImportFunction((builtin4Callback)(opa_builtin4)));

			byte[] wasm = File.ReadAllBytes("policy.wasm");

			// This will crash until all imports are satisfied
			Instance instance = new Instance(wasm, memoryImport, funcOpaAbort
				, funcOpaBuiltin0, funcOpaBuiltin1, funcOpaBuiltin2, funcOpaBuiltin3, funcOpaBuiltin4);

			foreach (var export in instance.Exports)
			{
				Console.WriteLine($"export {export.Name}");
			}
		}

		delegate void opaabortCallback(InstanceContext ctx, int addr);
		delegate int builtin0Callback(InstanceContext ctx, int builtinId, int opaCtxReserved);
		delegate int builtin1Callback(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1);
		delegate int builtin2Callback(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1, int addr2);
		delegate int builtin3Callback(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3);
		delegate int builtin4Callback(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3, int addr4);

		public void opa_abort(InstanceContext ctx, int addr)
		{
			var x = "";
		}

		public int opa_builtin0(InstanceContext ctx, int builtinId, int opaCtxReserved)
		{
			return 0;
		}

		public int opa_builtin1(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1)
		{
			return 0;
		}

		public int opa_builtin2(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1, int addr2)
		{
			return 0;
		}

		public int opa_builtin3(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3)
		{
			return 0;
		}

		public int opa_builtin4(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3, int addr4)
		{
			return 0;
		}
	}
}
