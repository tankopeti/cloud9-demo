using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cloud9_2.Models;
using Cloud9_2.Data;

namespace Cloud9_2.Pages.Admin
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class LoginLogModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginLogModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
        public Dictionary<string, bool> ColumnVisibility { get; set; } = new();

        public async Task OnGetAsync()
        {
            await CleanupOldRecordsAsync();

            UserActivities = await _context.UserActivities
                .OrderByDescending(ua => ua.LoginTime)
                .Take(500)
                .ToListAsync();

            var allColumns = new List<string>
            {
                "UserId",
                "UserName",
                "LoginTime",
                "LogoutTime",
                "IsActive"
            };

            var userRoles = User.FindAll(ClaimTypes.Role)
                .Select(r => r.Value)
                .ToList();

            var visibilitySettings = await _context.ColumnVisibilities
                .Where(cv => cv.PageName == "/Admin/LoginLog" && userRoles.Contains(cv.RoleName))
                .ToListAsync();

            // Default: minden oszlop látszik
            ColumnVisibility = allColumns.ToDictionary(col => col, col => true);

            // Ha van adatbázisbeli beállítás, az felülírja a defaultot
            foreach (var setting in visibilitySettings)
            {
                if (allColumns.Contains(setting.ColumnName))
                {
                    ColumnVisibility[setting.ColumnName] = setting.IsVisible;
                }
            }
        }

        private async Task CleanupOldRecordsAsync()
        {
            const int maxRecords = 500;

            int totalRecords = await _context.UserActivities.CountAsync();

            if (totalRecords > maxRecords)
            {
                var recordsToDelete = await _context.UserActivities
                    .OrderBy(ua => ua.LoginTime)
                    .Take(totalRecords - maxRecords)
                    .ToListAsync();

                _context.UserActivities.RemoveRange(recordsToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}