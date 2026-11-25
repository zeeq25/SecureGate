namespace SecureGate.Models;

public class AuditLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string UserName { get; set; } = "";
    public string EventType { get; set; } = "";
    public string Detail { get; set; } = "";
    public string? IpAddress { get; set; }

    // Display helpers (not stored in the database)
    public string Label => EventType.Replace('_', ' ').ToLower();
    public string BadgeClass => EventType switch
    {
        "LOGIN_FAIL" or "LOCKOUT" or "TOTP_FAIL" or "RECOVERY_FAIL" => "danger",
        "LOGIN_OK" or "ENROLL" => "success",
        "RECOVERY_USED" or "MFA_RESET" => "warning",
        _ => "secondary"
    };
}