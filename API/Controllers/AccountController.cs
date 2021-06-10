using System.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using API.DTOs;
using API.Interfaces;
using AutoMapper;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ILogger<AccountController> logger, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            // Note: this logic is not required once you use the [Required] attribute on the DTO clas.
            // if (String.IsNullOrWhiteSpace(registerDto.UserName) || String.IsNullOrWhiteSpace(registerDto.Password))
            // {
            //     return BadRequest("username and password is required");
            // }

            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("Username is taken");
            }

            // reverse map request to AppUser shape
            var user = _mapper.Map<AppUser>(registerDto);

            /* 
                when you use the 'using' keyword, C# will call the dispose() method inside
                the class being initialized, which releases the resources being used by this class.
                this is only available to classes that inherits from the IDisposable interface.
            */
            using var hmac = new HMACSHA512();

            // hash password and generate salt for hashed password so no users may have
            // the same hashed password even if passwords are identical.
            // note: each newly instantiated HMACSHA512 class generates a new random key
            user.UserName = registerDto.UserName.ToLower(); // always use lowercase when storing emails & username!
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
            });
        }

        /* 
            Token Authentication Mechanism
            1. Client sends usernaem + password to server
            2. Server validates credentials and sends back a JWT, which client will store in machine/browser.
            3. Then, client will send requests with JWT token in header, which server will verify as valid and
                process requests accordingly.

            JSON Web Token (JWT) Benefits:
            1. No session to manage, JWTs are self contained tokens.
            2. Portable - a single token can be used with multiple backends as long as they share the same signature/secret key to validate the token.
            3. No cookies required, thus mobile friendly 
            4. Performance - once a token is issued, there is no need to make a database request to verify a users authentication.
        */

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(e => e.Photos)
                .SingleOrDefaultAsync(user => user.UserName.ToLower() == loginDto.UserName.ToLower());

            if (user == null)
            {
                // best security practice is to not tell whether either username or password is wrong.
                return Unauthorized("Invalid username or password.");
            }

            // HMACSHA512 class takes an overload of a key that is generated via this class
            // we are using the user's salt value as a key to hash the password that is being sent in via
            // the loginDto object. 
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // if the hashed value is identical to the db user's PasswordHash, then it is the correct password.
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid username or password.");
                }
            }

            return Ok(new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(e => e.IsMain)?.Url,
                KnownAs = user.KnownAs,
            });
        }

        private async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(e => e.UserName == userName.ToLower());
        }
    }
}