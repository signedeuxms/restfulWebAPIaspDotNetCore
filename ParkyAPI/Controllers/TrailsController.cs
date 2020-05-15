using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.DTOs;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/trails")]
    //[Route("api/Trails")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ParkyOpenAPIspecTrail")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository _trailRepository;
        private readonly IMapper _mapper;


        public TrailsController(ITrailRepository trailRepo, IMapper mapper)
        {
            this._trailRepository = trailRepo;
            this._mapper = mapper;
        }


        /// <summary>
        /// Get list of trails.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDTO>))]
        public IActionResult GetTrails()
        {
            var trails = this._trailRepository.GetTrails();
            var trailsDTO = new List<TrailDTO>();
            foreach (var trail in trails)
            {
                //trailsDTO.Add(this._mapper.Map<TrailDTO>(trail));
                var trailDTO = new TrailDTO()
                {
                    Id = trail.Id,
                    Name = trail.Name,
                    Distance = trail.Distance,
                    NationalParkId = trail.NationalParkId,
                    Difficulty = trail.Difficulty,
                    NationalParkDTO = new NationalParkDTO
                    {
                        Id = trail.NationalPark.Id,
                        Name = trail.NationalPark.Name,
                        State = trail.NationalPark.State,
                        Created = trail.NationalPark.Created,
                        Established = trail.NationalPark.Established
                    }
                };
                trailsDTO.Add(trailDTO);
            }

            return Ok(trailsDTO);
        }


        /// <summary>
        /// Get individual trail.
        /// </summary>
        /// <param name="trailID"> The Id of the trail. </param>
        /// <returns></returns>
        [HttpGet("{trailID:int}", Name = nameof(GetTrail))]
        [ProducesResponseType(200, Type = typeof(TrailDTO))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int trailID)
        {
            var trail = this._trailRepository.GetTrail(trailID);

            if (trail == null)
            {
                return NotFound();
            }

            //var trailDTO = this._mapper.Map<TrailDTO>(trail);
            var trailDTO = new TrailDTO()
            {
                Id = trail.Id,
                Name = trail.Name,
                Distance = trail.Distance,
                NationalParkId = trail.NationalParkId,
                Difficulty = trail.Difficulty,
                NationalParkDTO = new NationalParkDTO
                {
                    Id = trail.NationalPark.Id,
                    Name = trail.NationalPark.Name,
                    State = trail.NationalPark.State,
                    Created = trail.NationalPark.Created,
                    Established = trail.NationalPark.Established
                }
            };

            return Ok(trailDTO);
        }


        /// <summary>
        /// Get individual trail.
        /// </summary>
        /// <param name="nationalParkID"> The Id of the trail. </param>
        /// <returns></returns>
        [HttpGet("[action]/{nationalParkID:int}")]
        [ProducesResponseType(200, Type = typeof(TrailDTO))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrailInNationalPark(int nationalParkID)
        {
            var trails = this._trailRepository.GetTrailsInNationalPark(nationalParkID);
            var trailsDTO = new List<TrailDTO>();

            if (trails == null)
            {
                return NotFound();
            }

            foreach( var trail in trails)
            {
                trailsDTO.Add(this._mapper.Map<TrailDTO>(trail));
                //var trailDTO = this._mapper.Map<TrailDTO>(trail);
                /*var trailDTO = new TrailDTO()
                {
                    Id = trail.Id,
                    Name = trail.Name,
                    Distance = trail.Distance,
                    NationalParkId = trail.NationalParkId,
                    Difficulty = trail.Difficulty,
                    NationalParkDTO = new NationalParkDTO
                    {
                        Id = trail.NationalPark.Id,
                        Name = trail.NationalPark.Name,
                        State = trail.NationalPark.State,
                        Created = trail.NationalPark.Created,
                        Established = trail.NationalPark.Established
                    }
                };
                trailsDTO.Add(trailDTO);*/
            }

            return Ok(trailsDTO);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailCreateDTO trailCreateDTO)
        {
            if (trailCreateDTO == null)
            {
                return BadRequest(ModelState);
            }

            if (this._trailRepository.TrailExists(trailCreateDTO.Name))
            {
                ModelState.AddModelError("", "\n Trail exists!!!");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var trail = this._mapper.Map<Trail>(trailCreateDTO);
            /*var trail = new Trail()
            {
                //Id = trailCreateDTO.Id,
                Name = trailCreateDTO.Name,
                Distance = trailCreateDTO.Distance,
                NationalParkId = trailCreateDTO.NationalParkId,
                Difficulty = trailCreateDTO.Difficulty,
                //DateCreated = trailCreateDTO.DateCreated,
                NationalPark = new NationalPark
                {
                    Id = trailCreateDTO.NationalParkId,
                    Name = trailCreateDTO.Nat.Name,
                    State = trailCreateDTO.NationalPark.State,
                    Created = trailCreateDTO.NationalPark.Created,
                    Established = trailCreateDTO.NationalPark.Established
                }
            };*/

            if (!this._trailRepository.CreateTrail(trail))
            {
                ModelState.AddModelError("", $"\n Something went wrong when saving the" +
                    $" record {trail.Name} !!!");
                return StatusCode(500, ModelState);
            }

            // return Ok();
            return CreatedAtRoute(nameof(GetTrail),
                                    new { trailID = trail.Id }, trail);
        }


        [HttpPatch("{trailID:int}", Name = nameof(UpdateTrail))]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail(int trailID,
                             [FromBody] TrailUpdateDTO trailUpdateDTO)
        {
            if (trailUpdateDTO == null || trailID != trailUpdateDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var trail = this._mapper.Map<Trail>(trailUpdateDTO);

            if (!this._trailRepository.UpdateTrail(trail))
            {
                ModelState.AddModelError("", $"\n Something went wrong when updating the" +
                    $" record {trail.Name} !!!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{trailID:int}", Name = nameof(DeleteTrail))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailID)
        {
            if (!this._trailRepository.TrailExists(trailID))
            {
                return NotFound();
            }

            var trail = this._trailRepository.GetTrail(trailID);

            if (!this._trailRepository.DeleteTrail(trail))
            {
                ModelState.AddModelError("", $"\n Something went wrong when deleting the" +
                    $" record {trail.Name} !!!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}