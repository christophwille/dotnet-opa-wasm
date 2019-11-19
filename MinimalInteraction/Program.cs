using System;
using System.IO;
using WasmerSharp;

namespace MinimalInteraction
{
	class Program
	{
		static void Main(string[] args)
		{
			byte[] wasm = File.ReadAllBytes("policy.wasm");
			Module m = Module.Create(wasm);

			Console.WriteLine("The loaded wasm has the following imports listed:");
			foreach (ImportDescriptor import in m.ImportDescriptors)
			{
				Console.WriteLine($"import: {import.Kind} {import.ModuleName}::{import.Name} ");
			}

			Console.Read();
		}
	}
}
