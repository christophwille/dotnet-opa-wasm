using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace AspNetAuthZwithOpa.Authorization
{
    public class OpaPolicyHandler : AuthorizationHandler<OpaPolicyRequirement>
    {
        private readonly SampleFactory _factory;
        private readonly ILogger<OpaPolicyHandler> _logger;

        public OpaPolicyHandler(SampleFactory factory, ILogger<OpaPolicyHandler> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OpaPolicyRequirement requirement)
        {
            string policyName = requirement.Policy;
            bool isPolicyAvailable = await _factory.EnsurePolicyWasmLoadedAsync(policyName);

            if (isPolicyAvailable)
            {
                using var opaPolicy = _factory.CreatePolicyInstance(policyName);

                opaPolicy.SetDataJson(@"{""world"": ""world""}");

                string input = @"{""message"": ""world""}";
                string output = opaPolicy.EvaluateJson(input);

                context.Succeed(requirement);
            }
            else
            {
                _logger.LogError($"Policy {policyName} not found, cannot evaluate");
            }
        }
    }
}
