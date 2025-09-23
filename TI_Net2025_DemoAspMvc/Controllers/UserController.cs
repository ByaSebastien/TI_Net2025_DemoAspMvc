using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using TI_Net2025_DemoAspMvc.Extensions;
using TI_Net2025_DemoAspMvc.Mappers;
using TI_Net2025_DemoAspMvc.Models;
using TI_Net2025_DemoAspMvc.Models.Dtos.User;
using TI_Net2025_DemoAspMvc.Models.Entities;
using TI_Net2025_DemoAspMvc.Repositories;
using TI_Net2025_DemoAspMvc.Services;

namespace TI_Net2025_DemoAspMvc.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly UserService _userService;

        public UserController(UserRepository userRepository, UserService userService)
        {
            _userService = userService;
            _userRepository = userRepository;
        }

        public ActionResult Register()
        {
            return View(new RegisterFormDto());
        }

        [HttpPost]
        public IActionResult Register([FromForm] RegisterFormDto form)
        {
            bool isValid = true;
            if(!ModelState.IsValid)
            {
                isValid = false;
            }

            try
            {
                _userService.Add(form.ToUser());
            }catch (Exception ex)
            {
                isValid = false;
                ModelState.AddModelError("email",ex.Message);
            }

            if (!isValid) { 

                form.Password = "";
                return View(form);
            }

            return RedirectToAction("Login","User");
        }

        public IActionResult Login()
        {
            return View(new LoginFormDto());
        }

        [HttpPost]
        public IActionResult Login([FromForm] LoginFormDto form)
        {
            if(!ModelState.IsValid)
            {
                form.Password = "";
                return View(form);
            }

            User? user = _userRepository.GetUserByUsernameOrEmail(form.Login);

            if(user == null)
            {
                ModelState.AddModelError<LoginFormDto>(m => m.Login, "Cet utilisateur n'existe pas");
                form.Password = "";
                return View(form);
            }

            if(!Argon2.Verify(user.Password, form.Password))
            {
                ModelState.AddModelError<LoginFormDto>(m => m.Password, "Mauvais mot de passe");
                form.Password = "";
                return View(form);
            }

            ClaimsPrincipal claims = new ClaimsPrincipal(
                new ClaimsIdentity([
                        new Claim(ClaimTypes.Sid, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role.ToString()) 
                    ],
                CookieAuthenticationDefaults.AuthenticationScheme)
            );

            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claims,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    //ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                });

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public IActionResult Logout()
        {

            Console.WriteLine($"Id : {User.GetId()}");
            Console.WriteLine($"Role : {User.GetRole().ToString()}");
            Console.WriteLine("Logged Out");

            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "User");
        }
    }
}
