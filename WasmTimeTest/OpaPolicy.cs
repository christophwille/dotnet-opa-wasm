using System;
using System.Collections.Generic;
using System.Text;
using Wasmtime;

namespace WasmTimeTest
{
	public static class OpaExtensions
	{
		public static OpaPolicy CreateOpaPolicy(this Module module)
		{
			var op = new OpaPolicy();
			op.Load(module);
			return op;
		}
	}

	public class OpaPolicy
	{
		private int _dataAddr;
		private int _baseHeapPtr;
		private int _baseHeapTop;
		private int _dataHeapPtr;
		private int _dataHeapTop;

		private Instance _instance;
		private OpaHost _host;
		private dynamic _policy;

		internal OpaPolicy()
		{
		}

		internal void Load(Module module)
		{
			_host = new OpaHost();
			_instance = module.Instantiate(_host);
			_policy = (dynamic)_instance;

			string builtins = DumpJson(_policy.builtins());

			_dataAddr = LoadJson("{}");
			_baseHeapPtr = _policy.opa_heap_ptr_get();
			_baseHeapTop = _policy.opa_heap_top_get();
			_dataHeapPtr = _baseHeapPtr;
			_dataHeapTop = _baseHeapTop;

			// endof js ctor LoadedPolicy
		}

		private int LoadJson(string json)
		{
			int addr = _policy.opa_malloc(json.Length);
			_host.EnvMemory.WriteString(addr, json);

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
			return DecodeNullTerminatedString(_host.EnvMemory, addr);
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
	}
}
