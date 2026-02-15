using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Cloud9_2.Pages.CRM.Sites
{
[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger) => _logger = logger;

    public int PageSize { get; set; } = 20;

    public Task OnGetAsync()
    {
        _logger.LogInformation("Sites Index loaded (API-only).");
        return Task.CompletedTask;
    }
}

}
