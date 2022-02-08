using AspNetAuthZwithOpa.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

        public async Task OnGetAsync([FromServices] SamplePolicy samplePolicy)
        {
            bool output = await samplePolicy.EvaluateAsync();
        }
    }
}
