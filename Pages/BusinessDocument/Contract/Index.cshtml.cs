using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cloud9_2.Pages.BusinessDocument.Contract
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public int PageSize { get; set; } = 20;

        public void OnGet()
        {
            // PageSize validálás
            if (int.TryParse(Request.Query["pageSize"], out var ps)
                && ps > 0 && ps <= 200)
            {
                PageSize = ps;
            }

            // opcionális: SortBy whitelist (hogy ne mehessen bármi be)
            var allowedSorts = new[]
            {
                "date_desc",
                "date_asc",
                "status",
                "partner"
            };

            if (!string.IsNullOrWhiteSpace(SortBy)
                && !allowedSorts.Contains(SortBy))
            {
                SortBy = null;
            }

            // StatusFilter whitelist (ha fix státusz kódokat használsz)
            var allowedStatuses = new[]
            {
                "DRAFT",
                "APPROVED",
                "SIGNED",
                "CANCELLED"
            };

            if (!string.IsNullOrWhiteSpace(StatusFilter)
                && !allowedStatuses.Contains(StatusFilter))
            {
                StatusFilter = null;
            }
        }
    }
}
