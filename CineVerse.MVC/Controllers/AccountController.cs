using CineVerse.Application.Common;
using CineVerse.Domain.Entities;
using CineVerse.MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CineVerse.MVC.Controllers
{
    public class AccountController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager
        ): Controller
    {
        public IActionResult Register() => View();


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new AppUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                DoB = model.DoB,
                Email = model.Email,
                EmailConfirmed = true
            };

            var userResult = await userManager.CreateAsync(user , model.Password);
            if (userResult.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(user, Roles.User);
                if (roleResult.Succeeded)
                {
                    TempData["Success"] = "Your account has been created. You can now log in.";
                    return RedirectToAction(nameof(Login));
                }
                roleResult.Errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e.Description));
            }
            else
            {
                userResult.Errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e.Description));
            }
            return View(model);

        }

        public IActionResult Login(string? returnUrl)
        {
            if (returnUrl is not null)
                TempData["ReturnUrl"] = returnUrl;
            else
                TempData.Remove(returnUrl!);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByEmailAsync(model.Email);
            if(user is not null)
            {
                await signInManager.SignOutAsync();

                var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    await userManager.ResetAccessFailedCountAsync(user);
                    await userManager.SetLockoutEndDateAsync(user, null);

                    var returnUrl = TempData["ReturnUrl"]?.ToString();
                    if (returnUrl is not null && Url.IsLocalUrl(returnUrl))
                        return LocalRedirect(returnUrl);

                    if (await userManager.IsInRoleAsync(user, Roles.Admin) || await userManager.IsInRoleAsync(user, Roles.Editor))
                        return RedirectToAction("Index", "Home", new { area = "Administrator" });


                    return RedirectToAction("Index","Home");
                }
                else if (result.IsLockedOut)
                {
                    var lockoutEnd = await userManager.GetLockoutEndDateAsync(user);
                    var minutesLeft = Math.Ceiling((lockoutEnd!.Value - DateTimeOffset.UtcNow).TotalMinutes);

                    ModelState.AddModelError(string.Empty, $"Your account has been locked due to too many failed attempts. Please try again in {minutesLeft} minutes.");
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
            }

            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
