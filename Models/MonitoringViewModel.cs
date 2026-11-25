namespace SecureGate.Models;

public class MonitoringViewModel
{
    public int Accounts { get; set; }
    public int MfaEnrolled { get; set; }
    public int FailedLast24h { get; set; }
    public int Locked { get; set; }
    public string Filter { get; set; } = "all";
    public List<UserRow> Users { get; set; } = new();
    public List<AuditLog> Events { get; set; } = new();
}

public class UserRow
{
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public bool MfaEnabled { get; set; }
    public bool Locked { get; set; }
}