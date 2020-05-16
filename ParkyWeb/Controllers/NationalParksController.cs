using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _nationalRepository;

        public NationalParksController(INationalParkRepository nationalRepository)
        {
            this._nationalRepository = nationalRepository;                              
        }

        public IActionResult Index()
        {
            return View(new NationalPark() { } );
        }


        public async Task<IActionResult> GetAllNationalPark()
        {
            var x = await this._nationalRepository
                                      .GetAllAsync(SDetail.NationalParkAPIpath);
            Console.WriteLine("\nSorry for the delay. . . .\n");
            var j = Json(new
            {
                data = await this._nationalRepository
                                      .GetAllAsync(SDetail.NationalParkAPIpath)
            });
            return Json( new { data = await this._nationalRepository
                                      .GetAllAsync(SDetail.NationalParkAPIpath)} );
        }
    }
}