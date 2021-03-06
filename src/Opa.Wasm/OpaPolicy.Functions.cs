﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Opa.Wasm
{
	public partial class OpaPolicy
	{
		private int Policy_Builtins()
		{
			var run = _instance.GetFunction(_store, "builtins");
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

		private void Policy_eval(int ctxAddr)
		{
			var run = _instance.GetFunction(_store, "eval");
			run?.Invoke(_store, ctxAddr);
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
