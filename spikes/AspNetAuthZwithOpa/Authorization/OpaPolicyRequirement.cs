using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AspNetAuthZwithOpa.Authorization
{
	// https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-3.1
	public class OpaPolicyRequirement : IAuthorizationRequirement
	{
		public string Policy { get; }

		public OpaPolicyRequirement(string policy)
		{
			Policy = policy;
		}
	}
}
