using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    // [Authorize(Roles = "Admin")] // this denotes that only user with a role of admin can access this api/method
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        // instead of importing the entire db context, we are just injecting the UnitOfWork service with access to individual repositories as its properties.
        // for benefits of the repository pattern, see: https://www.c-sharpcorner.com/UploadFile/8a67c0/repository-pattern-and-generic-repository-pattern/
        // see, e.g., unit of work pattern: https://www.c-sharpcorner.com/UploadFile/b1df45/unit-of-work-in-repository-pattern/
        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService, ILogger<UsersController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
            _logger = logger;
        }

        [Description("Get Users")]
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
            userParams.CurrentUserName = User.GetUsername();

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = gender == "male" ? "female" : "male";
            }

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }

        [Description("Get One User")]
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var apiCallerUsername = User.GetUsername();

            var user = await _unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser: apiCallerUsername == username);

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

            _unitOfWork.UserRepository.Update(user);

            if (await _unitOfWork.Complete()) return NoContent();

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
                PublicId = result.PublicId,
                IsApproved = false,
            };

            // if (user.Photos.Count == 0)
            // {
            //     photo.IsMain = true;
            // }

            user.Photos.Add(photo);

            if (await _unitOfWork.Complete())
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
            var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photoId);
            var photo = user.Photos.FirstOrDefault(e => e.Id == photoId);

            if (photo == null)
            {
                return NotFound("Photo does not exist.");
            }

            if (photo.IsMain)
            {
                return BadRequest("This is already your main photo.");
            }

            if (!photo.IsApproved)
            {
                return BadRequest("Photo is not approved yet");
            }

            var currentMain = user.Photos.FirstOrDefault(e => e.IsMain);

            if (currentMain != null)
            {
                currentMain.IsMain = false;
            }

            photo.IsMain = true;

            if (await _unitOfWork.Complete())
            {
                return NoContent();
            }

            return BadRequest("Failed to set main photo");
        }

        [Description("Delete user's photo")]
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photoId);
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

            if (await _unitOfWork.Complete())
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
            return await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());
        }
    }
}