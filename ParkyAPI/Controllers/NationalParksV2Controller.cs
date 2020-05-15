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
    [Route("api/v{version:apiVersion}/nationalparks")]
    [ApiVersion("2.0")]
    //[Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPIspecNationalPark")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksV2Controller : ControllerBase
    {
        private readonly INationalParkRepository _nationalParkRepository;
        private readonly IMapper _mapper;


        public NationalParksV2Controller(INationalParkRepository npRepo, IMapper mapper)
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
            var parks = this._nationalParkRepository.GetNationalParks().FirstOrDefault();
            /*var parksDTO = new List<NationalParkDTO>();
            foreach ( var park in parks)
            {
                parksDTO.Add(this._mapper.Map<NationalParkDTO>(park));
            }*/

            return Ok(this._mapper.Map<NationalParkDTO>(parks));
        }
    }
}