using System;
using System.Diagnostics;
using WasmerSharp;

namespace Opa.Wasm
{
	public class OpaPolicy
	{
		private Memory _memory;
		private Instance _instance;

		public void ReserveMemory()
		{
			_memory = Memory.Create(minPages: 5);
		}

		public void Load(Module m)
		{
			Import memoryImport = new Import("env", "memory", _memory);

			var funcOpaAbort = new Import("env", OpaFunc.Abort, new ImportFunction((opaabortCallback)(opa_abort)));
			var funcOpaBuiltin0 = new Import("env", OpaFunc.Builtin0, new ImportFunction((builtin0Callback)(opa_builtin0)));
			var funcOpaBuiltin1 = new Import("env", OpaFunc.Builtin1, new ImportFunction((builtin1Callback)(opa_builtin1)));
			var funcOpaBuiltin2 = new Import("env", OpaFunc.Builtin2, new ImportFunction((builtin2Callback)(opa_builtin2)));
			var funcOpaBuiltin3 = new Import("env", OpaFunc.Builtin3, new ImportFunction((builtin3Callback)(opa_builtin3)));
			var funcOpaBuiltin4 = new Import("env", OpaFunc.Builtin4, new ImportFunction((builtin4Callback)(opa_builtin4)));

			_instance = m.Instatiate(memoryImport, funcOpaAbort
				, funcOpaBuiltin0, funcOpaBuiltin1, funcOpaBuiltin2, funcOpaBuiltin3, funcOpaBuiltin4);

			string builtins = DumpJson(_memory, _instance.Call(OpaFunc.Builtins));
			// Console.WriteLine($"builtins: {builtins}");
			// TODO: Builtins are not implemented, not necessary for basic sample

			_dataAddr = LoadJson(_memory, "{}");
			_baseHeapPtr = AddrReturn(OpaFunc.HeapPtrGet);
			_baseHeapTop = AddrReturn(OpaFunc.HeapTopGet);
			_dataHeapPtr = _baseHeapPtr;
			_dataHeapTop = _baseHeapTop;

			// endof js ctor LoadedPolicy
		}

		public string Evaluate(string json)
		{
			// Reset the heap pointer before each evaluation
			AddrReturn(OpaFunc.HeapPtrSet, _dataHeapPtr);
			AddrReturn(OpaFunc.HeapTopSet, _dataHeapTop);

			// Load the input data
			var inputAddr = LoadJson(_memory, json);

			// Setup the evaluation context
			var ctxAddr = AddrReturn(OpaFunc.EvalCtxNew);
			AddrReturn(OpaFunc.EvalCtxSetInput, ctxAddr, inputAddr);
			AddrReturn(OpaFunc.EvalCtxSetData, ctxAddr, _dataAddr);

			// Actually evaluate the policy
			AddrReturn(OpaFunc.Eval, ctxAddr);

			// Retrieve the result
			var resultAddr = _instance.Call(OpaFunc.EvalCtxGetResult, ctxAddr);
			return DumpJson(_memory, resultAddr);
		}

		public void SetData(string json)
		{
			AddrReturn(OpaFunc.HeapPtrSet, _baseHeapPtr);
			AddrReturn(OpaFunc.HeapTopSet, _baseHeapTop);
			_dataAddr = LoadJson(_memory, json);
			_dataHeapPtr = AddrReturn(OpaFunc.HeapPtrGet);
			_dataHeapTop = AddrReturn(OpaFunc.HeapTopGet);
		}

		private int _dataAddr;
		private int _baseHeapPtr;
		private int _baseHeapTop;
		private int _dataHeapPtr;
		private int _dataHeapTop;

		// TODO: should be an extension method on Instance
		private int AddrReturn(string name, params object[] args)
		{
			var result = _instance.Call(name, args);
			return (int)result[0];
		}

		private int LoadJson(Memory memory, string json)
		{
			int length = json.Length;
			int addr = AddrReturn(OpaFunc.Malloc, length);
			byte[] jsonAsBytes = System.Text.Encoding.UTF8.GetBytes(json);

			unsafe
			{
				var buf = (byte*)memory.Data;
				for (int i = 0; i < length; i++)
				{
					buf[addr + i] = jsonAsBytes[i];
				}
			}

			int parseAddr = AddrReturn(OpaFunc.JsonParse, addr, json.Length);

			if (0 == parseAddr)
			{
				throw new ArgumentNullException("Parsing failed");
			}

			return parseAddr;
		}

		private string DumpJson(Memory memory, object[] addrResult)
		{
			int addr = AddrReturn(OpaFunc.JsonDump, (int)addrResult[0]);
			return DecodeNullTerminatedString(memory, addr);
		}

		private static string DecodeNullTerminatedString(Memory memory, int addr)
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
			Debugger.Break();
			// TODO: impl stringDecoder
		}

		public int opa_builtin0(InstanceContext ctx, int builtinId, int opaCtxReserved)
		{
			Debugger.Break();
			return 0;
		}

		public int opa_builtin1(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1)
		{
			Debugger.Break();
			return 0;
		}

		public int opa_builtin2(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1, int addr2)
		{
			Debugger.Break();
			return 0;
		}

		public int opa_builtin3(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3)
		{
			Debugger.Break();
			return 0;
		}

		public int opa_builtin4(InstanceContext ctx, int builtinId, int opaCtxReserved, int addr1, int addr2, int addr3, int addr4)
		{
			Debugger.Break();
			return 0;
		}
	}
}
