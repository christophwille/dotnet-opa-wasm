using System;

namespace Opa.Wasm
{
	public class PolicyEvaluationAbortedException : Exception
	{
		public PolicyEvaluationAbortedException() : base()
		{
		}

		public PolicyEvaluationAbortedException(string message) : base(message)
		{
		}

		public PolicyEvaluationAbortedException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
