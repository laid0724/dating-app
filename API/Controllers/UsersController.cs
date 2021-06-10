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
using Microsoft.AspNetCore.Http;
using API.Extensions;
using API.Helpers;

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
        private readonly IPhotoService _photoService;
        private readonly ILogger<UsersController> _logger;

        // instead of importing the entire db context, we are just injecting the UserRepository service.
        // for benefits of the repository pattern, see: https://www.c-sharpcorner.com/UploadFile/8a67c0/repository-pattern-and-generic-repository-pattern/
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService, ILogger<UsersController> logger)
        {
            _mapper = mapper;
            _photoService = photoService;
            _userRepository = userRepository;
            _logger = logger;
        }

        [Description("Get Users")]
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var requestingUser = await GetAppUser();
            userParams.CurrentUserName = requestingUser.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = requestingUser.Gender == "male" ? "female" : "male";
            }

            var users = await _userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }

        [Description("Get One User")]
        [HttpGet("{username}", Name = "GetUser")]
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
            var user = await GetAppUser();

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [Description("Upload photo")]
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await GetAppUser();
            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                /* 
                    return a 201 Created response when we add resource to the server
                    - here, we assign the GetUser method with a Name attribute so its route can be accessed here
                        - we also pass in a route value of username so that the route parameter is supplied
                    
                    this way, the response from the server will come back with a "Location" property, indicating the url
                    of where the photo is stored - in this case, the user's profile url

                */
                return CreatedAtRoute(
                    "GetUser",
                    new { username = user.UserName },  // this supplies the route param with the username for this method
                    _mapper.Map<PhotoDto>(photo)
                );
            }

            return BadRequest("Problem adding photo");
        }

        [Description("Set user's main photo")]
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await GetAppUser();
            var photo = user.Photos.FirstOrDefault(e => e.Id == photoId);

            if (photo == null)
            {
                return NotFound("Photo does not exist.");
            }

            if (photo.IsMain)
            {
                return BadRequest("This is already your main photo.");
            }

            var currentMain = user.Photos.FirstOrDefault(e => e.IsMain);

            if (currentMain != null)
            {
                currentMain.IsMain = false;
            }
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to set main photo");
        }

        [Description("Delete user's photo")]
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await GetAppUser();
            var photo = user.Photos.FirstOrDefault(e => e.Id == photoId);

            if (photo == null)
            {
                return NotFound("Photo does not exist.");
            }

            if (photo.IsMain)
            {
                return BadRequest("Cannot delete your main photo.");
            }

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
            }

            user.Photos.Remove(photo);

            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to delete photo.");
        }

        // when method is declared public in a controller without REST attributes, swagger will throw ambiguous http error
        // use protected to solve this issue
        protected async Task<AppUser> GetAppUser()
        {
            /*
                here, we are directly getting the username via Claims because we don't trust the user/client
                to provide the correct one.

                we can access something called Claims from an http request, where we can extract and read the current
                user sending the request as a ClaimsPrincipal, to read more, see:
                https://docs.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal?view=net-5.0
            */
            return await _userRepository.GetUserByUserNameAsync(User.GetUsername());
        }
    }
}