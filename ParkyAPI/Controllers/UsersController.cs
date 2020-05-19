using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/Users")]
    //[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;


        public UsersController(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticationModel userModel)
        {
            var user = this._userRepository.Authenticate(userModel.Username, 
                            userModel.Password);

            if(user == null)
            {
                return BadRequest(new { message = "username or password is incorrect" });
            }

            return Ok(user);
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthenticationModel userModel)
        {
            bool ifUserNameUnique = this._userRepository.IsUniqueUser(userModel.Username);

            if(!ifUserNameUnique)
            {
                return BadRequest(new { message = "username already exists" });
            }

            var user = this._userRepository.Register(userModel.Username,
                                    userModel.Password);

            if(user == null)
            {
                return BadRequest(new { message = "error while registering" });
            }
            return Ok();
        }
    }
}