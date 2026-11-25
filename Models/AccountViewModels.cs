using System.ComponentModel.DataAnnotations;

namespace SecureGate.Models;

public class LoginViewModel
{
    [Required]
    public string UserName { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

public class RegisterViewModel
{
    [Required, StringLength(20, MinimumLength = 3)]
    [RegularExpression(@"^[A-Za-z0-9_.-]+$", ErrorMessage = "Use letters, numbers, . _ - only.")]
    public string UserName { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";
}