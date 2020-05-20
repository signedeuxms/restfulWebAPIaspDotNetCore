using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModels;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INationalParkRepository _nationalParkRepository;
        private readonly ITrailRepository _trailRepository;
        private readonly IAccountRepository _accountRepository;

        public HomeController(ILogger<HomeController> logger,
            INationalParkRepository nationalParkRepository, 
            ITrailRepository trailRepository, IAccountRepository accountRepository)
        {
            _logger = logger;
            this._nationalParkRepository = nationalParkRepository;
            this._trailRepository = trailRepository;
            this._accountRepository = accountRepository;
        }

        public async Task<IActionResult> Index()
        {
            IndexVM nationalParksAndTrails = new IndexVM()
            {
                NationalParks = await this._nationalParkRepository.GetAllAsync(
                    SDetail.NationalParkAPIpath, 
                    HttpContext.Session.GetString("JWToken")),

                Trails = await this._trailRepository.GetAllAsync(
                    SDetail.TrailAPIpath, HttpContext.Session.GetString("JWToken"))
            };

            return View(nationalParksAndTrails);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            User user = new User();
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User account)
        {
            User user = await this._accountRepository.LoginAsync(
                SDetail.AccountAPIpath+"authenticate/", account);

            if(user.Token == null)
            {
                return View();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            // save token in a session
            HttpContext.Session.SetString("JWToken", user.Token);

            TempData["alert"] = "Welcome " + user.Username;

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User account)
        {
            bool userExist = await this._accountRepository.RegisterAsync(
                SDetail.AccountAPIpath + "register/", account);

            if (userExist == false)
            {
                return View();
            }

            TempData["alert"] = "Registration successful ";

            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();

        }


        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString("JWToken","");

            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
