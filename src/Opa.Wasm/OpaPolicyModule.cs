using System;
using Wasmtime;

namespace Opa.Wasm
{
	public class OpaPolicyModule : IDisposable
	{
		private bool disposedValue;

		private Engine _engine;
		private bool _ownsEngine;

		private Module _module;

		private OpaPolicyModule()
		{
		}

		public IOpaPolicy CreatePolicyInstance(IOpaSerializer serializer = null, long minMemSize = 2)
		{
			if (null == serializer) serializer = DefaultOpaSerializer.Instance;

			return new OpaPolicy(_engine, _module, serializer, minMemSize);
		}

		/// <summary>
		/// Use when you want one Engine to load/manage multiple Modules
		/// </summary>
		/// <returns></returns>
		public static Engine CreateEngine()
		{
			return new Engine();
		}

		/// <summary>
		/// Loads a WASM module from a file on disk
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static OpaPolicyModule Load(string fileName, Engine engine = null)
		{
			var module = new OpaPolicyModule();
			module.LoadFromFile(fileName, engine);
			return module;
		}

		/// <summary>
		/// Loads a WASM module from a byte array
		/// </summary>
		/// <param name="name"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		public static OpaPolicyModule Load(string name, byte[] content, Engine engine = null)
		{
			var module = new OpaPolicyModule();
			module.LoadFromBytes(name, content, engine);
			return module;
		}

		private void LoadFromFile(string fileName, Engine engine)
		{
			StoreOrCreateEngine(engine);
			_module = Module.FromFile(_engine, fileName);
		}

		private void LoadFromBytes(string name, byte[] content, Engine engine)
		{
			StoreOrCreateEngine(engine);
			_module = Module.FromBytes(_engine, name, content);
		}

		private void StoreOrCreateEngine(Engine engine)
		{
			if (null == engine)
			{
				_engine = new Engine();
				_ownsEngine = true;
			}
			else
			{
				_engine = engine;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_module.Dispose();
					_module = null;

					if (_ownsEngine)
					{
						_engine.Dispose();
						_engine = null;
					}
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
