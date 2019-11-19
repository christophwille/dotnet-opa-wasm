using System;

namespace PlaygroundApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var policy = new OpaPolicy();
			policy.ReserveMemory();
			policy.LoadFromDisk();

			Console.Read();
		}
	}
}
