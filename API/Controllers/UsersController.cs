using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        // instead of importing the entire db context, we are just injecting the UserRepository service.
        // for benefits of the repository pattern, see: https://www.c-sharpcorner.com/UploadFile/8a67c0/repository-pattern-and-generic-repository-pattern/
        public UsersController(IUserRepository userRepository, IMapper mapper, ILogger<UsersController> logger)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _logger = logger;
        }

        [Description("Get Users")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            return Ok(await _userRepository.GetMembersAsync());
        }

        [Description("Get One User")]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetMemberAsync(username);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [Description("Update user info")]
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            /*
                here, we are directly getting the username via the token because we don't trust the user/client
                to provide the correct one.

                we can access something called Claims from an http request, where we can extract and read the current
                user sending the request as a ClaimsPrincipal, to read more, see:
                https://docs.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal?view=net-5.0
            */
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUserNameAsync(username);

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }
    }
}