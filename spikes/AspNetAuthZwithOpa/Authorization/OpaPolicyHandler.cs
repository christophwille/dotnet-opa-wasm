using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Opa.Wasm;

namespace AspNetAuthZwithOpa.Authorization
{
    public class OpaPolicyHandler : AuthorizationHandler<OpaPolicyRequirement>
    {
        private readonly IPoliciesStore _policiesStore;
        private readonly ILogger<OpaPolicyHandler> _logger;

        public OpaPolicyHandler(IPoliciesStore policiesStore, ILogger<OpaPolicyHandler> logger)
        {
            _policiesStore = policiesStore;
            _logger = logger;
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
                using var opaRuntime = new OpaRuntime();

                // TODO: This incurs the compilation penalty for wasm - use an object pool (single-threaded use only)
                using var module = opaRuntime.Load(policyName, wasmBytes);

                using var opaPolicy = new OpaPolicy(opaRuntime, module);

                opaPolicy.SetData(@"{""world"": ""world""}");

                string input = @"{""message"": ""world""}";
                string output = opaPolicy.Evaluate(input);

                context.Succeed(requirement);
            }
            else
            {
                _logger.LogError($"Policy {policyName} not found, cannot evaluate");
            }
        }
    }
}
