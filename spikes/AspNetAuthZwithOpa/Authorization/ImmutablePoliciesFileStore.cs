using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;

namespace AspNetAuthZwithOpa.Authorization
{
	public class ImmutablePoliciesFileStore : IPoliciesStore
	{
		private readonly ILogger<ImmutablePoliciesFileStore> _logger;
		private readonly string _wasmLoadPath;

		// TODO: Add Cache

		public ImmutablePoliciesFileStore(ILogger<ImmutablePoliciesFileStore> logger)
		{
			_logger = logger;

			// Set default load path
			_wasmLoadPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		}

		public void SetRelativePath(string path)
		{
			throw new NotImplementedException();
		}

		public void SetAbsolutePath(string path)
		{
			throw new NotImplementedException();
		}

		public async Task<(byte[], bool)> LoadPolicyAsync(string name, bool throwOnLoadError = false)
		{
			string fileName = Path.Combine(_wasmLoadPath, name + ".wasm");
			bool fileExists = File.Exists(fileName);
			
			if (!fileExists)
			{
				_logger.LogWarning("Policy {0} not found", fileName);

				if (throwOnLoadError)
					throw new FileNotFoundException(fileName);
				else
					return (default(byte[]), false);
			}

			try
			{
				var bytes = await File.ReadAllBytesAsync(fileName);
				return (bytes, true);
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());

				if (throwOnLoadError)
					throw;
				else
					return (default(byte[]), false);
			}
		}
	}
}
