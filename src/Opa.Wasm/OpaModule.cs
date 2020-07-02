using System;
using Wasmtime;

namespace Opa.Wasm
{
	public class OpaModule : IDisposable
	{
		private Engine _engine;
		private Store _store;

		public OpaModule()
		{
			_engine = new Engine();
			_store = new Store(_engine);
		}

		public Host CreateHost() => new Host(_engine);

		public Module Load(string fileName)
		{
			return _store.LoadModule(fileName);
		}

		public Module Load(string name, byte[] content)
		{
			return _store.LoadModule(name, content);
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
				_store.Dispose();
				_store = null;
				_engine.Dispose();
				_engine = null;
			}
		}
	}
}
