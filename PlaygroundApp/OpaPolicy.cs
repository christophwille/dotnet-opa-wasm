using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using WasmerSharp;

namespace PlaygroundApp
{
	public class OpaPolicy
	{
		private Memory _memory;
		private Instance _instance;

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

			_instance = new Instance(wasm, memoryImport, funcOpaAbort
				, funcOpaBuiltin0, funcOpaBuiltin1, funcOpaBuiltin2, funcOpaBuiltin3, funcOpaBuiltin4);

			string builtins = DumpJson(_memory, _instance.Call("builtins"));
			Console.WriteLine(builtins);
		}

		private string DumpJson(Memory memory, object[] addrResult)
		{
			var jsondumpResult = _instance.Call("opa_json_dump", (int)addrResult[0]);
			int addr = (int)jsondumpResult[0];

			return DumpString(memory, addr);
		}

		private static string DumpString(Memory memory, int addr)
		{
			unsafe
			{
				int idx = addr;
				var buf = (byte*)memory.Data;

				while (buf[idx] != 0)
				{
					idx++;
				}

				var str = System.Text.Encoding.UTF8.GetString(buf + addr, idx - addr);
				return str;
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
			// TODO: impl stringDecoder
			Trace.TraceError("opa_abort was called");
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
