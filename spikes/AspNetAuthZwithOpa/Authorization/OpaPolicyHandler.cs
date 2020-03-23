using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Opa.Wasm;
using Wasmtime;

namespace AspNetAuthZwithOpa.Authorization
{
	public class OpaPolicyHandler : AuthorizationHandler<OpaPolicyRequirement>
	{
		private readonly IPoliciesStore _policiesStore;
		private readonly Store _store;

		public OpaPolicyHandler(IPoliciesStore policiesStore)
		{
			_policiesStore = policiesStore;
			var engine = new Engine();
			_store = engine.CreateStore();
		}

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OpaPolicyRequirement requirement)
		{
			// Only shows the concept, you'd need to: (1) cache wasms, (2) json-in, json-out, (3) evaluate out-json
			//       json-in: a defined set of properties of the request that is then being evaluated by the OPA policy (treated as black box)
			// Might need a https://docs.microsoft.com/en-us/aspnet/core/security/authorization/iauthorizationpolicyprovider?view=aspnetcore-3.1

			string policyName = requirement.Policy;
			var (wasmBytes, succeeded) = await _policiesStore.LoadPolicyAsync(policyName);

			if (succeeded)
			{
				using var module = _store.CreateModule(policyName, wasmBytes);
				var opaPolicy = module.CreateOpaPolicy();

				opaPolicy.SetData(@"{""world"": ""world""}");

				string input = @"{""message"": ""world""}";
				string output = opaPolicy.Evaluate(input);

				context.Succeed(requirement);
			}
		}
	}
}
