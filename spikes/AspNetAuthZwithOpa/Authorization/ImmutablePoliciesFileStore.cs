using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace AspNetAuthZwithOpa.Authorization
{
	public class ImmutablePoliciesFileStore : IPoliciesStore
	{
		private readonly IMemoryCache _cache;
		private readonly ILogger<ImmutablePoliciesFileStore> _logger;
		private readonly string _wasmLoadPath;

		public ImmutablePoliciesFileStore(IMemoryCache cache, ILogger<ImmutablePoliciesFileStore> logger)
		{
			_cache = cache;
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

		public virtual string GenerateCacheKeyForPolicy(string name)
		{
			return "OPA_AuthZ_Policies_Cached_" + name;
		}

		public async Task<(byte[], bool)> LoadPolicyAsync(string name, bool throwOnLoadError = false)
		{
			string cacheKey = GenerateCacheKeyForPolicy(name);
			if (_cache.TryGetValue(cacheKey, out byte[] wasmBytes))
			{
				return (wasmBytes, true);
			}

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
				_cache.Set(cacheKey, bytes);
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
