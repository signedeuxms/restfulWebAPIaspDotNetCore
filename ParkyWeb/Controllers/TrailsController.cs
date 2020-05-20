using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModels;
using ParkyWeb.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class TrailsController : Controller
    {
        private readonly ITrailRepository _trailRepository;
        private readonly INationalParkRepository _nationalParkRepository;


        public TrailsController(ITrailRepository trailRepository,
                                INationalParkRepository nationalRepository)
        {
            this._trailRepository = trailRepository;
            this._nationalParkRepository = nationalRepository;
        }


        public IActionResult Index()
        {
            return View(new Trail() { } );
        }


        /*
         * UpdateInsert() is used to update and insert a national park.
         * Id: a nullable id
         */
         [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateInsert(int? Id)
        {
            IEnumerable<NationalPark> nationalParks = await this._nationalParkRepository
                                        .GetAllAsync(SDetail.NationalParkAPIpath,
                                        HttpContext.Session.GetString("JWToken"));

            TrailsVM trailVM = new TrailsVM()
            {
                NationalParkList = nationalParks.Select(park => new SelectListItem
                {
                    Text = park.Name,
                    Value = park.Id.ToString()
                }),
                Trail = new Trail()
            };

            //var trailVM = await this.populateNationalParkDropDownm();

            if (Id == null)
            {
                // this instruction will be treu for insert and create
                return View(trailVM);

            }

            // load the object for update
            trailVM.Trail = await this._trailRepository.GetAsync(
                                SDetail.TrailAPIpath, Id.GetValueOrDefault(),
                                HttpContext.Session.GetString("JWToken"));

            if (trailVM.Trail == null)
            {
                return NotFound();
            }

            return View(trailVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInsert(TrailsVM trailVM)
        {
            if (ModelState.IsValid)
            {
                if (trailVM.Trail.Id == 0)
                {
                    await this._trailRepository.CreateAsync(SDetail.TrailAPIpath,
                        trailVM.Trail, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await this._trailRepository
                        .UpdateAsync(SDetail.TrailAPIpath + trailVM.Trail.Id,
                               trailVM.Trail, HttpContext.Session.GetString("JWToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> nationalParks = await this._nationalParkRepository
                                       .GetAllAsync(SDetail.NationalParkAPIpath,
                                       HttpContext.Session.GetString("JWToken"));

                TrailsVM trailVMa = new TrailsVM()
                {
                    NationalParkList = nationalParks.Select(park => new SelectListItem
                    {
                        Text = park.Name,
                        Value = park.Id.ToString()
                    }),
                    Trail = trailVM.Trail
                };

                //var trailVMa = await this.populateNationalParkDropDownm();

                return View(trailVMa);
            }
        }


        private async Task<TrailsVM> populateNationalParkDropDownm()
        {
            IEnumerable<NationalPark> nationalParks = await this._nationalParkRepository
                                        .GetAllAsync(SDetail.NationalParkAPIpath,
                                        HttpContext.Session.GetString("JWToken"));

            TrailsVM trailVM = new TrailsVM()
            {
                NationalParkList = nationalParks.Select(park => new SelectListItem
                {
                    Text = park.Name,
                    Value = park.Id.ToString()
                }),
                Trail = new Trail()
            };

            return trailVM;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int Id)
        {
            var status = await this._trailRepository
                                    .DeleteAsync(SDetail.TrailAPIpath, Id,
                                    HttpContext.Session.GetString("JWToken"));

            if (status)
            {
                return Json(new { success = true, message = "Delete successful" });
            }

            return Json(new { success = false, message = "Delete not successful" });
        }


        public async Task<IActionResult> GetAllTrail()
        {
            return Json( new { data = await this._trailRepository
                                      .GetAllAsync(SDetail.TrailAPIpath, 
                                      HttpContext.Session.GetString("JWToken"))
            } );
        }
    }
}