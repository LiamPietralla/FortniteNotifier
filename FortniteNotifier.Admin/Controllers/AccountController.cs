using FortniteNotifier.Admin.Helpers;
using FortniteNotifier.Admin.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Serilog;
using Serilog.Core;
using System.Security.Claims;

namespace FortniteNotifier.Admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly ConfigHelper _config;

        public AccountController(IConfiguration configuration)
        {
            _config = new(configuration);
        }

        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                Log.Information("Login Request");
                    
                LoginViewModel vm = new();

                // Store the redirect URL if set
                if (Request.Query["ReturnUrl"] != StringValues.Empty)
                {
                    vm.ReturnUrl = Request.Query["ReturnUrl"]!;
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating to the login page.");
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string password, string returnUrl)
        {
            try
            {
                Log.Information("Login Attempt");
                
                // Check if the password is correct            
                if (password == _config.LoginPassword)
                {
                    Log.Information("Login Attempt Successful");
                   
                    // Sign in the user
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "Admin")
                }, CookieAuthenticationDefaults.AuthenticationScheme)));

                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        // Redirect to the home page
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Redirect to specified url if local
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    // Create the view model and return to login
                    LoginViewModel vm = new()
                    {
                        Error = "Incorrect password"
                    };

                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while logging in.");
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogoutAsync()
        {
            try
            {
                // Sign the user out of cookie authentication
                await HttpContext.SignOutAsync();

                // Redirect to the home page
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while logging out.");
                return RedirectToAction("Index", "Error");
            }
        }
    }
}
