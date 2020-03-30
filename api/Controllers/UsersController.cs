using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;

        public UsersController(IDatingRepository repo, DataContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.repo = repo;
            this.context = context;
        }

        // GET api/Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await repo.GetUsers();

            // ? Automapper syntax: mapper.Map<destinationType>(sourceObj);
            var usersToReturn = mapper.Map<IEnumerable<UserForListDto>>(users); // ? here, we map the users to their DTOs as IEnumerable

            return Ok(usersToReturn);
        }

        // GET api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await repo.GetUser(id);

            // ? Automapper syntax: mapper.Map<destinationType>(sourceObj);
            var userToReturn = mapper.Map<UserForDetailedDto>(user); // ? here, we map the user to its DTO

            return Ok(userToReturn);
        }

        // // POST api/Users
        // [HttpPost]
        // public async Task<IActionResult> PostUsers([FromBody] string value) { }

        // // PUT api/Users/5
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutUsers(int id, [FromBody] string value) { }

        // // DELETE api/Users/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteUsers(int id) { }
    }
}