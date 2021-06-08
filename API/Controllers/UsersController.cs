using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    // with [Authorize] in this location instead of individual methods,
    // ALL methods in this controller will need JWT token to be able to call this API
    // add "Authorization: Bearer <JWT token string>" in headers as key:value pair
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;

        // instead of importing the entire db context, we are just injecting the UserRepository service.
        // for benefits of the repository pattern, see: https://www.c-sharpcorner.com/UploadFile/8a67c0/repository-pattern-and-generic-repository-pattern/
        public UsersController(UserRepository userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [Description("Get Users")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return Ok(await _userRepository.GetUsersAsync());
        }

        [Description("Get One User")]
        [HttpGet("{username}")]
        public async Task<ActionResult<AppUser>> GetUser(string userName)
        {
            var user = await _userRepository.GetUserByUserNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}