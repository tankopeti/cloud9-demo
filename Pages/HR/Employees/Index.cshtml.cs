using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloud9_2.Data;
using Cloud9_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Cloud9_2.Pages.HR.Employees
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ✅ nálad a típus neve Employees
        public IList<Cloud9_2.Models.Employees> Employees { get; set; } = new List<Cloud9_2.Models.Employees>();

        public int TotalRecords { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 50;

        public int TotalPages => PageSize > 0
            ? (int)Math.Ceiling((double)TotalRecords / PageSize)
            : 0;

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (CurrentPage < 1) CurrentPage = 1;
            if (PageSize < 10) PageSize = 50;
            if (PageSize > 200) PageSize = 200;

            // JS tölti a táblát, itt nem kell adatot lekérni
            await Task.CompletedTask;
            return Page();
        }
    }
}