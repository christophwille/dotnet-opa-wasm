using AspNetAuthZwithOpa.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AspNetAuthZwithOpa.Pages
{
    [Authorize(Policy = "GuardedByOpa")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet([FromServices] SamplePolicy samplePolicy)
        {
            bool output = samplePolicy.Evaluate();
        }
    }
}
