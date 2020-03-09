using System;

namespace Opa.Wasm
{
	static class OpaFunc
	{
		public static string Abort = "opa_abort";

		public static string Builtin0 = "opa_builtin0";
		public static string Builtin1 = "opa_builtin1";
		public static string Builtin2 = "opa_builtin2";
		public static string Builtin3 = "opa_builtin3";
		public static string Builtin4 = "opa_builtin4";

		public static string Builtins = "builtins";

		public static string HeapPtrGet = "opa_heap_ptr_get";
		public static string HeapPtrSet = "opa_heap_ptr_set";
		public static string HeapTopGet = "opa_heap_top_get";
		public static string HeapTopSet = "opa_heap_top_set";
		
		public static string EvalCtxNew = "opa_eval_ctx_new";
		public static string EvalCtxSetInput = "opa_eval_ctx_set_input";
		public static string EvalCtxSetData = "opa_eval_ctx_set_data";
		public static string EvalCtxGetResult = "opa_eval_ctx_get_result";
		public static string Eval = "eval";

		public static string Malloc = "opa_malloc";
		public static string JsonParse = "opa_json_parse";
		public static string JsonDump = "opa_json_dump";
	}
}
