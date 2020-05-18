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

namespace ParkyWeb.Controllers
{
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
        public async Task<IActionResult> UpdateInsert(int? Id)
        {
            IEnumerable<NationalPark> nationalParks = await this._nationalParkRepository
                                        .GetAllAsync(SDetail.NationalParkAPIpath);

            TrailsVM trailVM = new TrailsVM()
            {
                NationalParkList = nationalParks.Select(park => new SelectListItem
                {
                    Text = park.Name,
                    Value = park.Id.ToString()
                })
            };

            if (Id == null)
            {
                // this instruction will be treu for insert and create
                return View(trailVM);

            }

            // load the object for update
            trailVM.Trail = await this._trailRepository.GetAsync(
                                SDetail.NationalParkAPIpath, Id.GetValueOrDefault());

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
                    await this._trailRepository.CreateAsync(SDetail.TrialAPIpath,
                        trailVM.Trail);
                }
                else
                {
                    await this._trailRepository
                        .UpdateAsync(SDetail.NationalParkAPIpath+ trailVM.Trail.Id,
                                        trailVM.Trail);
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(trailVM);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int Id)
        {
            var status = await this._trailRepository
                                    .DeleteAsync(SDetail.TrialAPIpath, Id);

            if (status)
            {
                return Json(new { success = true, message = "Delete successful" });
            }

            return Json(new { success = false, message = "Delete successful" });
        }


        public async Task<IActionResult> GetAlTrail()
        {
            return Json( new { data = await this._trailRepository
                                      .GetAllAsync(SDetail.TrialAPIpath)} );
        }
    }
}