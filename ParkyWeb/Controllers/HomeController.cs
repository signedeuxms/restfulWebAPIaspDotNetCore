using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        public HomeController(ILogger<HomeController> logger,
            INationalParkRepository nationalParkRepository, 
            ITrailRepository trailRepository)
        {
            _logger = logger;
            this._nationalParkRepository = nationalParkRepository;
            this._trailRepository = trailRepository;
        }

        public async Task<IActionResult> Index()
        {
            IndexVM nationalParksAndTrails = new IndexVM()
            {
                NationalParks = await this._nationalParkRepository.GetAllAsync(
                    SDetail.NationalParkAPIpath),
                Trails = await this._trailRepository.GetAllAsync(
                    SDetail.TrailAPIpath)
            };

            return View(nationalParksAndTrails);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
