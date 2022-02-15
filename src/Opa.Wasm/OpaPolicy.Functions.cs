using System;
using System.Collections.Generic;
using System.Text;

namespace Opa.Wasm
{
	public partial class OpaPolicy
	{
		private int? Policy_opa_wasm_abi_version()
		{
			var global = _instance.GetGlobal(_store, "opa_wasm_abi_version");
			return (int?)global?.GetValue(_store);
		}

		private int? Policy_opa_wasm_abi_minor_version()
		{
			var global = _instance.GetGlobal(_store, "opa_wasm_abi_minor_version");
			return (int?)global?.GetValue(_store);
		}

		private int Policy_Builtins()
		{
			var run = _instance.GetFunction(_store, "builtins");
			return (int)run?.Invoke(_store);
		}

		private int Policy_Entrypoints()
		{
			var run = _instance.GetFunction(_store, "entrypoints");
			return (int)run?.Invoke(_store);
		}

		private int Policy_opa_heap_ptr_get()
		{
			var run = _instance.GetFunction(_store, "opa_heap_ptr_get");
			return (int)run?.Invoke(_store);
		}

		private void Policy_opa_heap_ptr_set(int ptr)
		{
			var run = _instance.GetFunction(_store, "opa_heap_ptr_set");
			run?.Invoke(_store, ptr);
		}

		private int Policy_opa_eval_ctx_new()
		{
			var run = _instance.GetFunction(_store, "opa_eval_ctx_new");
			return (int)run?.Invoke(_store);
		}

		private void Policy_opa_eval_ctx_set_input(int ctxAddr, int inputAddr)
		{
			var run = _instance.GetFunction(_store, "opa_eval_ctx_set_input");
			run?.Invoke(_store, ctxAddr, inputAddr);
		}

		private void Policy_opa_eval_ctx_set_data(int ctxAddr, int dataAddr)
		{
			var run = _instance.GetFunction(_store, "opa_eval_ctx_set_data");
			run?.Invoke(_store, ctxAddr, dataAddr);
		}

		private void Policy_opa_eval_ctx_set_entrypoint(int ctxAddr, int entrypoint)
		{
			var run = _instance.GetFunction(_store, "opa_eval_ctx_set_entrypoint");
			run?.Invoke(_store, ctxAddr, entrypoint);
		}

		private void Policy_eval(int ctxAddr)
		{
			var run = _instance.GetFunction(_store, "eval");
			run?.Invoke(_store, ctxAddr);
		}

		private int Policy_opa_eval(/*int addr, */
			int entrypoint_id, int dataaddr, int jsonaddr, int jsonlength, int heapaddr/*, int format*/)
		{
			var run = _instance.GetFunction(_store, "opa_eval");
			return (int)run?.Invoke(_store, 0 /* always 0 */, entrypoint_id,
				dataaddr, jsonaddr, jsonlength, heapaddr, 0 /* json format */);
		}

		private int Policy_opa_eval_ctx_get_result(int ctxAddr)
		{
			var run = _instance.GetFunction(_store, "opa_eval_ctx_get_result");
			return (int)run?.Invoke(_store, ctxAddr);
		}

		private int Policy_opa_malloc(int length)
		{
			var run = _instance.GetFunction(_store, "opa_malloc");
			return (int)run?.Invoke(_store, length);
		}

		private void Policy_opa_free(int addr)
		{
			var run = _instance.GetFunction(_store, "opa_free");
			run?.Invoke(_store, addr);
		}

		private int Policy_opa_json_parse(int addr, int length)
		{
			var run = _instance.GetFunction(_store, "opa_json_parse");
			return (int)run?.Invoke(_store, addr, length);
		}

		private int Policy_opa_json_dump(int addrResult)
		{
			var run = _instance.GetFunction(_store, "opa_json_dump");
			return (int)run?.Invoke(_store, addrResult);
		}
	}
}
