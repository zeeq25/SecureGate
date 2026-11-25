using SecureGate.Data;
using SecureGate.Models;

namespace SecureGate.Services;

public class AuditService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _http;

    public AuditService(ApplicationDbContext db, IHttpContextAccessor http)
    {
        _db = db;
        _http = http;
    }

    public async Task LogAsync(string eventType, string? userName, string detail = "")
    {
        _db.AuditLogs.Add(new AuditLog
        {
            EventType = eventType,
            UserName = string.IsNullOrWhiteSpace(userName) ? "(unknown)" : userName,
            Detail = detail,
            IpAddress = _http.HttpContext?.Connection.RemoteIpAddress?.ToString()
        });
        await _db.SaveChangesAsync();
    }
}