using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Opa.Wasm;
using Wasmtime;

namespace AspNetAuthZwithOpa.Authorization
{
	public class OpaPolicyHandler : AuthorizationHandler<OpaPolicyRequirement>
	{
		private readonly IWebHostEnvironment _env;
		private readonly Store _store;
		private readonly string _wasmLoadPath;

		public OpaPolicyHandler(IWebHostEnvironment env)
		{
			_env = env;
			var engine = new Engine();
			_store = engine.CreateStore();

			// Yes, this isn't a nice way, but loading from disk on every request wouldn't be production-ready either
			_wasmLoadPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

		}
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OpaPolicyRequirement requirement)
		{
			// Only shows the concept, you'd need to: (1) cache wasms, (2) json-in, json-out, (3) evaluate out-json
			//       json-in: a defined set of properties of the request that is then being evaluated by the OPA policy (treated as black box)
			// Might need a https://docs.microsoft.com/en-us/aspnet/core/security/authorization/iauthorizationpolicyprovider?view=aspnetcore-3.1

			string policy = requirement.Policy;

			using var module = _store.CreateModule(System.IO.Path.Combine(_wasmLoadPath, policy + ".wasm"));
			var opaPolicy = module.CreateOpaPolicy();

			opaPolicy.SetData(@"{""world"": ""world""}");

			string input = @"{""message"": ""world""}";
			string output = opaPolicy.Evaluate(input);

			context.Succeed(requirement);

			return Task.CompletedTask;
		}
	}
}
