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

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(DataContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
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

            /* 
                when you use the 'using' keyword, C# will call the dispose() method inside
                the class being initialized, which releases the resources being used by this class.
                this is only available to classes that inherits from the IDisposable interface.
            */
            using var hmac = new HMACSHA512();

            // hash password and generate salt for hashed password so no users may have
            // the same hashed password even if passwords are identical.
            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(), // always use lowercase when storing emails & username!
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(e => e.UserName == userName.ToLower());
        }
    }
}