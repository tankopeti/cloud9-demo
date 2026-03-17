using Cloud9_2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Cloud9_2.Pages.Admin;

[Authorize(Roles = "SuperAdmin,Admin")]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ApplicationDbContext _context;

    public IndexModel(
        ILogger<IndexModel> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public int TotalUsers { get; set; }
    public int OnlineUsers { get; set; }
    public int TotalRoles { get; set; }
    public int LoginLogCount { get; set; }

    public List<OnlineUserVm> RecentOnlineUsers { get; set; } = new();
    public List<LoginLogVm> RecentLoginLogs { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Ideiglenesen, amíg az Identity user/role típust pontosan be nem kötjük
        TotalUsers = 0;
        TotalRoles = 0;

        OnlineUsers = await _context.UserActivities
            .Where(x => x.IsActive)
            .Select(x => x.UserName)
            .Distinct()
            .CountAsync();

        LoginLogCount = await _context.UserActivities.CountAsync();

        var activeUsers = await _context.UserActivities
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.LoginTime)
            .Select(x => new OnlineUserVm
            {
                UserName = x.UserName,
                LastActivity = x.LoginTime
            })
            .Take(50)
            .ToListAsync();

        RecentOnlineUsers = activeUsers
            .GroupBy(x => x.UserName)
            .Select(g => g.OrderByDescending(x => x.LastActivity).First())
            .Take(5)
            .ToList();

        RecentLoginLogs = await _context.UserActivities
            .OrderByDescending(x => x.LoginTime)
            .Take(5)
            .Select(x => new LoginLogVm
            {
                UserName = x.UserName,
                LoginTime = x.LoginTime
            })
            .ToListAsync();
    }

    public class OnlineUserVm
    {
        public string UserName { get; set; } = string.Empty;
        public DateTime? LastActivity { get; set; }
    }

    public class LoginLogVm
    {
        public string UserName { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
    }
}