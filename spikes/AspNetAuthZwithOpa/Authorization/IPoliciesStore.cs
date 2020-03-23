using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetAuthZwithOpa.Authorization
{
	public interface IPoliciesStore
	{
		Task<(byte[], bool)> LoadPolicyAsync(string name, bool throwOnLoadError = false);
	}
}
