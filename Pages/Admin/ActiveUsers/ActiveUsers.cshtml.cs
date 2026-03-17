using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cloud9_2.Data;
using Cloud9_2.Models;

namespace Cloud9_2.Pages.Admin
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ActiveUsersModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ActiveUsersModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<UserActivity> ActiveUsers { get; set; } = new();

        public async Task OnGetAsync()
        {
            var activeRows = await _context.UserActivities
                .Where(a => a.IsActive && !string.IsNullOrWhiteSpace(a.UserName))
                .OrderByDescending(a => a.LoginTime)
                .ToListAsync();

            ActiveUsers = activeRows
                .GroupBy(a => a.UserName.Trim().ToLower())
                .Select(g => g.First())
                .OrderByDescending(a => a.LoginTime)
                .ToList();
        }
    }
}