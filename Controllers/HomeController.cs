using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureGate.Data;
using SecureGate.Models;

namespace SecureGate.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<IdentityUser> users)
    {
        _logger = logger;
        _db = db;
        _users = users;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var name = User.Identity?.Name;
        var mine = await _db.AuditLogs
            .Where(a => a.UserName == name)
            .OrderByDescending(a => a.Id)
            .Take(8)
            .ToListAsync();
        return View(mine);
    }

    // Open to any signed-in user for now; we'll restrict it to Administrator/Analyst when roles are added
    [Authorize]
    public async Task<IActionResult> Monitoring(string filter = "all")
    {
        var failures = new[] { "LOGIN_FAIL", "TOTP_FAIL", "RECOVERY_FAIL", "LOCKOUT" };
        var logins = new[] { "LOGIN_OK", "LOGOUT" };
        var changes = new[] { "REGISTER", "ENROLL", "CODES_REGEN", "RECOVERY_USED", "MFA_RESET", "UNLOCK" };

        var q = _db.AuditLogs.AsQueryable();
        if (filter == "failures") q = q.Where(a => failures.Contains(a.EventType));
        else if (filter == "logins") q = q.Where(a => logins.Contains(a.EventType));
        else if (filter == "changes") q = q.Where(a => changes.Contains(a.EventType));

        var since = DateTime.UtcNow.AddHours(-24);
        var model = new MonitoringViewModel
        {
            Filter = filter,
            Accounts = await _users.Users.CountAsync(),
            MfaEnrolled = await _users.Users.CountAsync(u => u.TwoFactorEnabled),
            Locked = await _users.Users.CountAsync(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow),
            FailedLast24h = await _db.AuditLogs.CountAsync(a => failures.Contains(a.EventType) && a.Timestamp > since),
            Users = await _users.Users.Select(u => new UserRow
            {
                UserName = u.UserName ?? "",
                Email = u.Email ?? "",
                MfaEnabled = u.TwoFactorEnabled,
                Locked = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow
            }).ToListAsync(),
            Events = await q.OrderByDescending(a => a.Id).Take(40).ToListAsync()
        };
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}