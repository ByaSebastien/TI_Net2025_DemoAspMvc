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

namespace TI_Net2025_DemoAspMvc.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
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

            if(_userRepository.ExistByEmail(form.Email))
            {
                ModelState.AddModelError<RegisterFormDto>(f => f.Email, "Email already exist");
                isValid = false;
            }

            if(_userRepository.ExistByUsername(form.Username))
            {
                ModelState.AddModelError<RegisterFormDto>(f => f.Username, "Username already exist");
                isValid = false;
            }

            if (!isValid) { 

                form.Password = "";
                return View(form);
            }

            

            User user = form.ToUser();
            user.Password = Argon2.Hash(form.Password);
            user.Role = UserRole.User;

            _userRepository.Add(user);

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

            HttpContext.SignInAsync(claims);

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
