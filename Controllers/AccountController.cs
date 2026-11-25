using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureGate.Models;
using SecureGate.Services;

namespace SecureGate.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _users;
    private readonly SignInManager<IdentityUser> _signIn;
    private readonly AuditService _audit;

    public AccountController(UserManager<IdentityUser> users, SignInManager<IdentityUser> signIn, AuditService audit)
    {
        _users = users;
        _signIn = signIn;
        _audit = audit;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signIn.PasswordSignInAsync(
            model.UserName, model.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            await _audit.LogAsync("LOGIN_OK", model.UserName, "Session created");
            return RedirectToAction("Index", "Home");   // MFA code step comes later
        }

        if (result.IsLockedOut)
        {
            await _audit.LogAsync("LOCKOUT", model.UserName, "Account locked for 15 minutes");
            ModelState.AddModelError("", "Account locked after too many failed attempts. Try again in 15 minutes.");
        }
        else
        {
            await _audit.LogAsync("LOGIN_FAIL", model.UserName, "Wrong username or password");
            ModelState.AddModelError("", "Username or password is incorrect.");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
        var result = await _users.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _audit.LogAsync("REGISTER", model.UserName, "Account created");
            TempData["Msg"] = "Account created. Sign in to continue.";
            return RedirectToAction(nameof(Login));
        }

        foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var name = User.Identity?.Name;
        await _signIn.SignOutAsync();
        await _audit.LogAsync("LOGOUT", name, "Session destroyed");
        TempData["Msg"] = "You’re signed out.";
        return RedirectToAction(nameof(Login));
    }

    // Still static for now
    public IActionResult Enroll() => View();
    public IActionResult Verify() => View();
    public IActionResult RecoveryCodes() => View();
}