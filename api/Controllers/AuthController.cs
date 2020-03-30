using System.Text;
using System.Security.Claims;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // ? if this attribute is commented out, validation attributes from models wont be applied. the methods also wont automatically infer paramaters are coming from request body.
    public class AuthController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IAuthRepository repo;
        private readonly IConfiguration config;

        public AuthController(
          DataContext context,
          IAuthRepository repo,
          IConfiguration config
        )
        {
            this.context = context;
            this.repo = repo;
            this.config = config;
        }


        // POST api/auth/register
        [Description("Register a new user")] // FIXME: why isn't this fuckign showing up on swagger? WTF
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        // ? In MVC architect, DTOs (data transfer objects) are simplified versions of an object (less properties) used to map to the main object and for being displayed at end of view.
        // ? thus, we're rendering user as DTO so that passwordHash and passwordSalt will be hidden
        {
            // convert username to lowercase so there are no duplicates
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            // check if username is taken
            if (await repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("Username already exists.");
            }

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        // POST api/auth/login
        [Description("User login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
            {
                return Unauthorized(); // ! dont return any hints as to whether user exists or not for security concerns
            }

            /* 
              * Validation steps:
                1. Server first looks into db to check if user exists as an entry and verifies password;
                2. Start building token:
                    a. build token with 2 claims: id & username
                    b. to make sure the tokens are valid when it comes back to the server from the client, the server needs to sign it with an encrypted key
                    c. then, create token descriptor to include claims, signing key, token expiration date
                    d. create a JWT token with the token descriptor
                3. send back the JWT token as an object to the client
            */

            // first, we want to build up a token to return to the users, with two bits of information included: user id and username, but as a JWT token.
            // the server can then take a look at this token and check its validity without having to look inside at the database.

            Claim[] claims = new[] // ? first, we want to gather the user id and username as Claims
            {
              new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()), // ? params: (1) type, (2) value - which needs to be string
              new Claim(ClaimTypes.Name, userFromRepo.Username.ToString()),
            };

            // we also need a security key for reading & decrypting the token in the future,
            // this key should be stored in the appsettings.json file, injected as IConfiguration in constructor, and encoded with UTF8 bytes:
            SymmetricSecurityKey key = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(
                config.GetSection("AppSettings:Token").Value
              )
            );

            // then, we need to hash the security key with an algorithm:
            SigningCredentials credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // then, create a security token descriptor, which will contain the claims, expiry date, and key for the token:
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credential
            };

            // then, add a token handler to pass in the token descriptor, which will create the JWT token that the server is going to pass back to the client:
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            // lastly, return the token as an object to the client
            return Ok(
              new
              {
                  token = tokenHandler.WriteToken(token)
              }
            );

            // * to decrypt the token and see that it is working as expected, 
            // * go to: https://jwt.io/

            /* 
              * How to use this token?
                - for all controllers or endpoints with the [Authorize] attribute, when they are called by the client side,
                  the token must be included inside the request's header in the following format:
                  {
                    "Authorization": "Bearer <Token>"
                  }
            */
        }

        [Description("Read all users")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await context.Users.ToListAsync();
            // return Ok(users.Select(user => user.Username));
            return Ok(users);
        }
    }
}