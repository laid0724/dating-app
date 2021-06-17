using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        public AdminController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ILogger<AdminController> logger, IPhotoService photoService, IMapper mapper)
        {
            _mapper = mapper;
            _photoService = photoService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }

        [Description("get users with roles")]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
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

        [Description("edit user roles")]
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult<IEnumerable<string>>> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Description("get photos for moderation")]
        [Authorize(Policy = "RequireModerateRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult<ICollection<PhotoForApprovalDto>>> GetPhotosForModeration()
        {
            return Ok(await _unitOfWork.PhotoRepository.GetUnapprovedPhotos());
        }

        [Description("approve photo")]
        [Authorize(Policy = "RequireModerateRole")]
        [HttpPost("photos-to-moderate")]
        public async Task<ActionResult> ApprovePhoto([FromBody] PhotoForApprovalDto photoForApprovalDto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(photoForApprovalDto.UserName);
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoForApprovalDto.Id);

            if (user == null) return NotFound("User not found");
            if (photo == null) return NotFound("Photo not found");

            photo.IsApproved = true;

            if (user.Photos.Any(p => !p.IsMain))
            {
                photo.IsMain = true;
            }

            if (_unitOfWork.HasChanges() && await _unitOfWork.Complete())
            {
                return Ok(_mapper.Map<PhotoForApprovalDto>(photo));
            }

            return BadRequest("Photo approval failed");
        }

        [Description("reject photo")]
        [Authorize(Policy = "RequireModerateRole")]
        [HttpDelete("photos-to-moderate/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);

            if (photo == null) return NotFound();

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
            }

            _unitOfWork.PhotoRepository.RemovePhoto(photo);

            if (_unitOfWork.HasChanges() && await _unitOfWork.Complete())
            {
                return NoContent();
            }

            return BadRequest("Photo rejection failed");
        }
    }
}