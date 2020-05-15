using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParkyAPI.Models;
using ParkyAPI.Models.DTOs;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ParkyOpenAPIspecNationalPark")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _nationalParkRepository;
        private readonly IMapper _mapper;


        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            this._nationalParkRepository = npRepo;
            this._mapper = mapper;
        }


        /// <summary>
        /// Get list of national parks.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDTO>))]
        public IActionResult GetNationalParks()
        {
            var parks = this._nationalParkRepository.GetNationalParks();
            var parksDTO = new List<NationalParkDTO>();
            foreach ( var park in parks)
            {
                parksDTO.Add(this._mapper.Map<NationalParkDTO>(park));
            }

            return Ok(parksDTO);
        }


        /// <summary>
        /// Get individual national park.
        /// </summary>
        /// <param name="nationalParkID"> The Id of the national park. </param>
        /// <returns></returns>
        [HttpGet("{nationalParkID:int}", Name = nameof(GetNationalPark))]
        [ProducesResponseType(200, Type = typeof(NationalParkDTO))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetNationalPark( int nationalParkID)
        {
            var park = this._nationalParkRepository.GetNationalPark(nationalParkID);

            if (park == null )
            {
                return NotFound();
            }

            var parkDTO = this._mapper.Map<NationalParkDTO>(park);
            /*var parkDTO = new NationalParkDTO()
            {
                Created = park.Created,
                Id = park.Id,
                Name = park.Name,
                State = park.State
            };*/
            return Ok(parkDTO);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NationalParkDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateNationalPark([FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null)
            {
                return BadRequest(ModelState);
            }

            if (this._nationalParkRepository.NationalParkExists(nationalParkDTO.Name))
            {
                ModelState.AddModelError( "", "\n National park exists!!!");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var park = this._mapper.Map<NationalPark>(nationalParkDTO);

            if (!this._nationalParkRepository.CreateNationalPark(park))
            {
                ModelState.AddModelError("", $"\n Something went wrong when saving the" +
                    $" record {park.Name} !!!");
                return StatusCode(500, ModelState);
            }

            // return Ok();
            return CreatedAtRoute(nameof(GetNationalPark), 
                                    new { nationalParkID = park.Id }, park);
        }


        [HttpPatch ("{nationalParkID:int}", Name = nameof(UpdateNationalPark))]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateNationalPark(int nationalParkID, 
                             [FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null ||  nationalParkID != nationalParkDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var park = this._mapper.Map<NationalPark>(nationalParkDTO);

            if (!this._nationalParkRepository.UpdateNationalPark(park))
            {
                ModelState.AddModelError("", $"\n Something went wrong when updating the" +
                    $" record {park.Name} !!!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{nationalParkID:int}", Name = nameof(DeleteNationalPark))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int nationalParkID)
        {
            if (!this._nationalParkRepository.NationalParkExists(nationalParkID))
            {
                return NotFound();
            }

            var park = this._nationalParkRepository.GetNationalPark(nationalParkID);

            if (!this._nationalParkRepository.DeleteNationalPark(park))
            {
                ModelState.AddModelError("", $"\n Something went wrong when deleting the" +
                    $" record {park.Name} !!!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}