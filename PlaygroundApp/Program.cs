using System;
using System.IO;
using WasmerSharp;

namespace PlaygroundApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Memory memory = Memory.Create(minPages: 5);
			Import memoryImport = new Import("env", "memory", memory);

			byte[] wasm = File.ReadAllBytes("policy.wasm");

			// This will crash until all imports are satisfied
			Instance instance = new Instance(wasm, memoryImport);

			
		}
	}
}
