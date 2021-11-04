using System;
using Wasmtime;

namespace Opa.Wasm
{
	public class OpaRuntime : IDisposable
	{
		private Engine _engine;

		public OpaRuntime()
		{
			_engine = new Engine();
		}

		public Linker CreateLinker() => new Linker(_engine);
		public Store CreateStore() => new Store(_engine);

		/// <summary>
		/// Loads a WASM module from a file on disk
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public Module Load(string fileName)
		{
			return Module.FromFile(_engine, fileName);
		}

		/// <summary>
		/// Loads a WASM module from a byte array
		/// </summary>
		/// <param name="name"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		public Module Load(string name, byte[] content)
		{
			return Module.FromBytes(_engine, name, content);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_engine.Dispose();
				_engine = null;
			}
		}
	}
}
