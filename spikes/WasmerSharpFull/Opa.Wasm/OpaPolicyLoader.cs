using System;
using System.IO;
using WasmerSharp;

namespace Opa.Wasm
{
	public static class OpaPolicyLoader
	{
		public static Module LoadFromDisk(string filename)
		{
			byte[] wasm = File.ReadAllBytes(filename);
			return LoadFromBytes(wasm);
		}

		public static Module LoadFromBytes(byte[] wasm)
		{
			return Module.Create(wasm);
		}
	}
}
