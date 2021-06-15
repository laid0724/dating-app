using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager, ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [Description("get users with roles")]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("uses-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .Include(r => r.UserRoles)
                    .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    UserName = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        [Description("get photos for moderation")]
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
        {
            return Ok("Admins or moderators can see this");
        }


    }
}