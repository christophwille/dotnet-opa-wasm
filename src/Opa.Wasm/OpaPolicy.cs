using System;
using System.Diagnostics;
using System.IO;
using WasmerSharp;

namespace Opa.Wasm
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

		public void LoadFromDisk(string filename)
		{
			byte[] wasm = File.ReadAllBytes(filename);
			Load(wasm);
		}

		public void Load(byte[] wasm)
		{
			Import memoryImport = new Import("env", "memory", _memory);

			var funcOpaAbort = new Import("env", "opa_abort", new ImportFunction((opaabortCallback)(opa_abort)));
			var funcOpaBuiltin0 = new Import("env", "opa_builtin0", new ImportFunction((builtin0Callback)(opa_builtin0)));
			var funcOpaBuiltin1 = new Import("env", "opa_builtin1", new ImportFunction((builtin1Callback)(opa_builtin1)));
			var funcOpaBuiltin2 = new Import("env", "opa_builtin2", new ImportFunction((builtin2Callback)(opa_builtin2)));
			var funcOpaBuiltin3 = new Import("env", "opa_builtin3", new ImportFunction((builtin3Callback)(opa_builtin3)));
			var funcOpaBuiltin4 = new Import("env", "opa_builtin4", new ImportFunction((builtin4Callback)(opa_builtin4)));

			_instance = new Instance(wasm, memoryImport, funcOpaAbort
				, funcOpaBuiltin0, funcOpaBuiltin1, funcOpaBuiltin2, funcOpaBuiltin3, funcOpaBuiltin4);

			string builtins = DumpJson(_memory, _instance.Call("builtins"));
			// Console.WriteLine($"builtins: {builtins}");
			// TODO: Builtins are not implemented, not necessary for basic sample

			_dataAddr = LoadJson(_memory, "{}");
			_baseHeapPtr = AddrReturn("opa_heap_ptr_get");
			_baseHeapTop = AddrReturn("opa_heap_top_get");
			_dataHeapPtr = _baseHeapPtr;
			_dataHeapTop = _baseHeapTop;

			// endof js ctor LoadedPolicy
		}

		public string Evaluate(string json)
		{
			// Reset the heap pointer before each evaluation
			AddrReturn("opa_heap_ptr_set", _dataHeapPtr);
			AddrReturn("opa_heap_top_set", _dataHeapTop);

			// Load the input data
			var inputAddr = LoadJson(_memory, json);

			// Setup the evaluation context
			var ctxAddr = AddrReturn("opa_eval_ctx_new");
			AddrReturn("opa_eval_ctx_set_input", ctxAddr, inputAddr);
			AddrReturn("opa_eval_ctx_set_data", ctxAddr, _dataAddr);

			// Actually evaluate the policy
			AddrReturn("eval", ctxAddr);

			// Retrieve the result
			var resultAddr = _instance.Call("opa_eval_ctx_get_result", ctxAddr);
			return DumpJson(_memory, resultAddr);
		}

		public void SetData(string json)
		{
			AddrReturn("opa_heap_ptr_set", _baseHeapPtr);
			AddrReturn("opa_heap_top_set", _baseHeapTop);
			_dataAddr = LoadJson(_memory, json);
			_dataHeapPtr = AddrReturn("opa_heap_ptr_get");
			_dataHeapTop = AddrReturn("opa_heap_top_get");
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
			int addr = AddrReturn("opa_malloc", length);
			byte[] jsonAsBytes = System.Text.Encoding.UTF8.GetBytes(json);

			unsafe
			{
				var buf = (byte*)memory.Data;
				for (int i = 0; i < length; i++)
				{
					buf[addr + i] = jsonAsBytes[i];
				}
			}

			int parseAddr = AddrReturn("opa_json_parse", addr, json.Length);

			if (0 == parseAddr)
			{
				throw new ArgumentNullException("Parsing failed");
			}

			return parseAddr;
		}

		private string DumpJson(Memory memory, object[] addrResult)
		{
			int addr = AddrReturn("opa_json_dump", (int)addrResult[0]);
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
