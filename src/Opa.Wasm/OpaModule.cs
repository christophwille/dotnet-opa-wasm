using System;
using Wasmtime;

namespace Opa.Wasm
{
	public class OpaModule : IDisposable
	{
		private Engine _engine;

		public OpaModule()
		{
			_engine = new Engine();
		}

		public Linker CreateLinker() => new Linker(_engine);
		public Store CreateStore() => new Store(_engine);

		public Module Load(string fileName)
		{
			return Module.FromFile(_engine, fileName);
		}

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
