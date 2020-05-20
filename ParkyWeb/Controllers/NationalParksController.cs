using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    [Authorize]
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


        /*
         * UpdateInsert() is used to update and insert a national park.
         * Id: a nullable id
         */
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateInsert(int? Id)
        {
            NationalPark park = new NationalPark();

            if (Id == null)
            {
                // this instruction will be treu for insert and create
                return View(park);

            }

            // load the object for update
            park = await this._nationalRepository.GetAsync(SDetail.NationalParkAPIpath, 
                    Id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));

            if ( park == null)
            {
                return NotFound();
            }

            return View(park);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInsert(NationalPark nationalPark)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    byte[] picture = null;

                    using (var file = files[0].OpenReadStream())
                    {
                        using (var stream = new MemoryStream())
                        {
                            file.CopyTo(stream);
                            picture = stream.ToArray();
                        }
                    }

                    nationalPark.Picture = picture;
                }
                else
                {
                    var parkFromDatabase = await this._nationalRepository
                                .GetAsync(SDetail.NationalParkAPIpath, nationalPark.Id,
                                HttpContext.Session.GetString("JWToken"));
                    nationalPark.Picture = parkFromDatabase.Picture;
                }

                if ( nationalPark.Id == 0)
                {
                    await this._nationalRepository.CreateAsync(SDetail.NationalParkAPIpath,
                        nationalPark, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await this._nationalRepository
                        .UpdateAsync(SDetail.NationalParkAPIpath+nationalPark.Id, 
                                  nationalPark, HttpContext.Session.GetString("JWToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(nationalPark);
            }
        }


        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int Id)
        {
            var status = await this._nationalRepository
                                    .DeleteAsync(SDetail.NationalParkAPIpath, Id,
                                    HttpContext.Session.GetString("JWToken"));

            if (status)
            {
                return Json(new { success = true, message = "Delete successful" });
            }

            return Json(new { success = false, message = "Delete successful" });
        }


        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json( new { data = await this._nationalRepository
                                      .GetAllAsync(SDetail.NationalParkAPIpath,
                                      HttpContext.Session.GetString("JWToken"))
            });
        }
    }
}