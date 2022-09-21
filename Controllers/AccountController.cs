using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Seed.ViewModel;

namespace Autorization.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<AccountController> logger
    )
    {
        _userManager = userManager; 
        _signInManager = signInManager;
        _logger = logger;
    }

    public IActionResult Login(string returnUrl) => View(new LoginViewModel() { ReturnUrl = returnUrl});
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if(user is null) return BadRequest();

        var result = await _signInManager.PasswordSignInAsync(user,model.Password,false,false);
        return LocalRedirect($"{model.ReturnUrl}" ?? "/");
    }
    public IActionResult Register(string returnUrl) => View(new RegisterViewModel() { ReturnUrl = returnUrl});
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var user = new IdentityUser(model.Username);
        var result = await _userManager.CreateAsync(user, model.Password);

        if(!result.Succeeded) return BadRequest();

        return LocalRedirect($"/Account/Login?ReturnUrl={model.ReturnUrl}"); 
    }
}