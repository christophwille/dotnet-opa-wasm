using System;
using System.Collections.Generic;
using System.Text;
using Wasmtime;

namespace Opa.Wasm
{
	public class OpaPolicyStore : IDisposable
	{
		private Store _store;

		public OpaPolicyStore()
		{
			_store = new Store();
		}

		public Store Store { get { return _store; } }

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
			}
		}
	}
}
